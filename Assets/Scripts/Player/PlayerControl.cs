
using Camera;
using InputSystem;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkTransport))]
public class PlayerControl : NetworkBehaviour
{

    public enum PlayerState 
    {
        Idle,
        Walk,
        ReverseWalk,
        Run,
        Attack
    }

    [SerializeField] private float _walkSpeed = 3.5f;
    //[SerializeField] private float _rotationSpeed = 1.5f;
    //[SerializeField] private float _runSpeedOffset = 2.0f;
    [SerializeField] private float gravity = 20.0F;
    [SerializeField] private Transform _cameraTransform;
    
    [SerializeField] private Vector2 _defaulPositionRange = new Vector2(-4, 4);
    [SerializeField] NetworkVariable<Vector3> _networkPositionDirection = new NetworkVariable<Vector3>();
    [SerializeField] NetworkVariable<Vector3> _networkRotationDirection = new NetworkVariable<Vector3>();
    [SerializeField] private NetworkVariable<PlayerState> _networkPlayerState = new NetworkVariable<PlayerState>();
    [SerializeField] private NetworkVariable<float> _networkPlayerHealth = new NetworkVariable<float>(100);
    [SerializeField] private float _minMeleeDistance = 3.0f;
    [SerializeField] private GameObject _melee;

    private CharacterController _controller;
    private Animator _anim;

    // client caches positions
    private Vector3 _moveDirection = Vector3.zero;
    private Vector3 _oldMoveDirection = Vector3.zero;

    private Rigidbody rb;
    private InputManager _inputManager;
    private bool hasAnim;
    private int _xVelHash;
    private int _yVelHash;


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        _cameraTransform = UnityEngine.Camera.main.transform;
       
    }
    private void Start()
    {
        if (IsClient && IsOwner)
        {
            hasAnim = TryGetComponent<Animator>(out _anim);
            rb = GetComponent<Rigidbody>();
            _inputManager = GetComponent<InputManager>();

            _xVelHash = Animator.StringToHash("horizontal");
            _yVelHash = Animator.StringToHash("vertical");
            
            transform.position = new Vector3(Random.Range(_defaulPositionRange.x, _defaulPositionRange.y), 0,
                Random.Range(_defaulPositionRange.x, _defaulPositionRange.y));
            
            CameraManager.Instance.FallowPLayer(transform.Find("PlayerCameraRoot"));
        }
        
    }

    private void Update()
    {
        if (IsClient && IsOwner) 
        {
            Cursor.visible = false;
            ClientInput();
        }

        ClientMoveAndRotate();
        ClientVisuals();
        
    }

    private void FixedUpdate()
    {
        if (IsClient && IsOwner)
        {
            if (_networkPlayerState.Value == PlayerState.Attack && ActiveMeleeActionKey())
            {
                CheckMeleeAttack( _melee.transform, Vector3 .up);
            }
        }
    }

    private void CheckMeleeAttack(Transform melee, Vector3 aimDirection)
    {
        RaycastHit _hit;

        int _layermask = LayerMask.GetMask("Player");

        if (Physics.Raycast(melee.position, melee.transform.TransformDirection(aimDirection), out _hit,
                _minMeleeDistance, _layermask))
        {
            Debug.DrawRay(melee.position,melee.transform.TransformDirection(aimDirection)*_minMeleeDistance,Color.yellow);

            var _playerHit = _hit.transform.GetComponent<NetworkObject>();
            if (_playerHit != null)
            {
                UpdateHealthServerRpc(10, _playerHit.OwnerClientId);
            }
        }
        else
        {
            Debug.DrawRay(melee.position,melee.transform.TransformDirection(aimDirection)*_minMeleeDistance,Color.red);
        }
    }
    
    private void ClientInput()
    {
        _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _moveDirection = transform.TransformDirection(_moveDirection);
        _moveDirection *= _walkSpeed;
        _moveDirection.y -= gravity * Time.deltaTime;
        _moveDirection = _cameraTransform.forward * _moveDirection.z + _cameraTransform.right * _moveDirection.x;
        _moveDirection.y = 0f;
        _controller.Move(_moveDirection * Time.deltaTime);
        
        
        // forward & backward direction
        // Vector3 direction = transform.TransformDirection(Vector3.forward);
         float forwardInput = Input.GetAxis("Vertical");
        // Vector3 inputPosition = direction * forwardInput;

        // change animation states
        if (forwardInput == 0)
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        else if (!ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        else if (ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
        {
            //inputPosition = direction * _runSpeedOffset;
            UpdatePlayerStateServerRpc(PlayerState.Run);
        }
        else if (forwardInput < 0)
            UpdatePlayerStateServerRpc(PlayerState.ReverseWalk);

        if (ActiveMeleeActionKey() && forwardInput == 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.Attack);
            return;
        }

        if (_oldMoveDirection != _moveDirection)
        {
            _oldMoveDirection = _moveDirection;
            UpdateClientPositionAndRotationServerRpc(_moveDirection);
        }
    }

    private void ClientMoveAndRotate()
    {
        if (_networkPositionDirection.Value != Vector3.zero)
        {
            _controller.SimpleMove(_networkPositionDirection.Value);
        }
        if (_networkRotationDirection.Value != Vector3.zero)
        {
            transform.Rotate(_networkRotationDirection.Value, Space.World);
        }
    }
    private void ClientVisuals()
    {
        if (_networkPlayerState.Value == PlayerState.Walk)
        {
            _anim.SetFloat("Walk", 1);
        } else if (_networkPlayerState.Value == PlayerState.ReverseWalk)
        {
            _anim.SetFloat("Walk", -1);
        } else if(_networkPlayerState.Value == PlayerState.Run)
        {
            _anim.SetFloat("Walk", 1);
        }
        else
        {
            _anim.SetFloat("Walk", 0);
        }

        if (_networkPlayerState.Value == PlayerState.Attack) 
        {

            _anim.SetTrigger("Attack");
        }

    }

    private static bool ActiveRunningActionKey()
    {
        return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    private static bool ActiveMeleeActionKey()
    {
        return Input.GetKey(KeyCode.Space);
    }

    [ServerRpc]
    public void UpdateClientPositionAndRotationServerRpc(Vector3 newPosition)
    {
        _networkPositionDirection.Value = newPosition;
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState state)
    {
        _networkPlayerState.Value = state;
    }

    [ServerRpc]
    public void UpdateHealthServerRpc(int damage, ulong clientId)
    {
        var clientWithDamage = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject
            .GetComponent<PlayerControl>();

        if (clientWithDamage != null && clientWithDamage._networkPlayerHealth.Value > 0)
        {
            clientWithDamage._networkPlayerHealth.Value -= damage;
        }

        NotifyHealthChangedClientRpc(damage, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientId }
            }
        });
    }

    [ClientRpc]
    public void NotifyHealthChangedClientRpc(int Damage, ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner) return;
        
        Debug.Log(($"Client got melee {Damage} "));
    }

}
