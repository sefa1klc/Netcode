
using UnityEngine;
using Unity.Netcode;
using Camera;
using Cinemachine;

[RequireComponent(typeof(NetworkObject))]
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

    [SerializeField] private float MouseSensitivity = 21.9f;
    [SerializeField] private float _walkSpeed = 7.0f;  
    [SerializeField] private float _runSpeedOffset = 4.0f;  
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Vector2 _defaultPositionRange = new Vector2(-4, 4);
    [SerializeField] private NetworkVariable<Vector3> _networkPositionDirection = new NetworkVariable<Vector3>();
    [SerializeField] private NetworkVariable<Vector3> _networkRotationDirection = new NetworkVariable<Vector3>();
    [SerializeField] private NetworkVariable<PlayerState> _networkPlayerState = new NetworkVariable<PlayerState>();
    [SerializeField] private NetworkVariable<float> _networkPlayerHealth = new NetworkVariable<float>(100);
    [SerializeField] private float _minMeleeDistance = 3.0f;
    [SerializeField] private GameObject _melee;


    [SerializeField] private float UpperLimit = -40f;
    [SerializeField] private float BottomLimit = 70f;
    [SerializeField] private float RotationSpeed = 10.0f;
    [SerializeField] private Transform CameraRoot;
    [SerializeField] private CinemachineVirtualCamera VCamera;


    private Vector3 _moveDirection = Vector3.zero;
    private Vector3 _oldMoveDirection = Vector3.zero;

    private Rigidbody _rb;
    private Animator _anim;
    private InputManager2 _inputManager;
    private int _xVelHash;
    private int _yVelHash;
    private int _crouchHash;

    private float _xRotation;
    private bool _isCrouching;

    private Vector3 _currentVelocity = Vector3.zero;

    private bool _hasAnimator;

    private void Awake()
    {
        _hasAnimator = TryGetComponent<Animator>(out _anim);
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        _inputManager = GetComponent<InputManager2>();
        _cameraTransform = UnityEngine.Camera.main.transform;
        VCamera = UnityEngine.Camera.main.GetComponent<CinemachineVirtualCamera>();

        // Rigidbody'nin drag ve angularDrag de�erlerini kontrol edelim
        _rb.drag = 0f;
        _rb.angularDrag = 0.05f;
    }

    private void Start()
    {
        if (IsClient && IsOwner)
        {
            _xVelHash = Animator.StringToHash("X-Vel");
            _yVelHash = Animator.StringToHash("Y-Vel");
            _crouchHash = Animator.StringToHash("Crouch");

            transform.position = new Vector3(Random.Range(_defaultPositionRange.x, _defaultPositionRange.y), 0,
                Random.Range(_defaultPositionRange.x, _defaultPositionRange.y));

            CameraRoot = transform.Find("Skeleton/Hips/Spine/Chest/UpperChest/Neck/Head/Left_Eye/CameraRoot");
            CameraManager.Instance.FallowPLayer(CameraRoot);
        }
    }

    private void Update()
    {
        if (IsClient && IsOwner)
        {
            Cursor.visible = false;
            HandleInput();
            HandleCrouch();
        }

        MoveAndRotate();
    }

    private void FixedUpdate()
    {
        if (IsClient && IsOwner)
        {
            if (_networkPlayerState.Value == PlayerState.Attack && ActiveMeleeActionKey())
            {
                PerformMeleeAttack(_melee.transform, Vector3.up);
            }
        }
    }

    private void LateUpdate()
    {
        CamMovements(); 
    }

    private void PerformMeleeAttack(Transform melee, Vector3 aimDirection)
    {
        RaycastHit hit;
        int layermask = LayerMask.GetMask("Player");

        if (Physics.Raycast(melee.position, melee.transform.TransformDirection(aimDirection), out hit, _minMeleeDistance, layermask))
        {
            Debug.DrawRay(melee.position, melee.transform.TransformDirection(aimDirection) * _minMeleeDistance, Color.yellow);

            var playerHit = hit.transform.GetComponent<PlayerControl>();
            if (playerHit != null)
            {
                UpdateHealthServerRpc(10, playerHit.OwnerClientId);
            }
        }
        else
        {
            Debug.DrawRay(melee.position, melee.transform.TransformDirection(aimDirection) * _minMeleeDistance, Color.red);
        }
    }

    private void HandleInput()
    {
        float targetSpeed = _inputManager.Run ? _runSpeedOffset : _walkSpeed;

        Vector3 moveDirection = new Vector3(_inputManager.Move.x, 0f, _inputManager.Move.y).normalized;
        Vector3 worldMoveDirection = _cameraTransform.TransformDirection(moveDirection);

        Vector3 targetVelocity = worldMoveDirection * targetSpeed;

        _rb.velocity = new Vector3(targetVelocity.x, _rb.velocity.y, targetVelocity.z);

        float currentXVel = Mathf.Lerp(_anim.GetFloat(_xVelHash), _inputManager.Move.x * targetSpeed, 0.1f);
        float currentYVel = Mathf.Lerp(_anim.GetFloat(_yVelHash), _inputManager.Move.y * targetSpeed, 0.1f);


        _anim.SetFloat(_xVelHash, currentXVel);
        _anim.SetFloat(_yVelHash, currentYVel);



        //if (currentYVel == 0 && currentXVel == 0)    
        //{
        //    UpdatePlayerStateServerRpc(PlayerState.Idle);
        //}
        //else if (!_inputManager.Run && currentYVel > 0)
        //{
        //    UpdatePlayerStateServerRpc(PlayerState.Walk);
        //}
        //else if (_inputManager.Run && currentYVel > 5)
        //{
        //    UpdatePlayerStateServerRpc(PlayerState.Run);
        //}
        //else if (currentYVel < 0)
        //{
        //    UpdatePlayerStateServerRpc(PlayerState.ReverseWalk);
        //}


        if (_oldMoveDirection != moveDirection)
        {
            _oldMoveDirection = moveDirection;
            UpdateClientPositionAndRotationServerRpc(moveDirection);
        }
    }

    private void HandleCrouch() => _anim.SetBool(_crouchHash, _inputManager.Crouch);

    private void CamMovements()
    {
        if (!_hasAnimator || CameraRoot == null) return;

        var Mouse_X = _inputManager.Look.x;
        var Mouse_Y = _inputManager.Look.y;

        if (VCamera != null)
        {
            Transform virtualCameraTransform = VCamera.transform;
            virtualCameraTransform.position = CameraRoot.position;

            _xRotation -= Mouse_Y * MouseSensitivity * Time.smoothDeltaTie;
            _xRotation = Mathf.Clamp(_xRotation, UpperLimit, BottomLimit);

            Quaternion targetRotation = Quaternion.Euler(0, virtualCameraTransform.eulerAngles.y, 0);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, targetRotation, RotationSpeed * Time.deltaTime));

            virtualCameraTransform.localRotation = Quaternion.Euler(_xRotation, 0, 0);
        }
    }



    private void MoveAndRotate()
    {
        if (_networkPositionDirection.Value != Vector3.zero)
        {
            _rb.velocity = _networkPositionDirection.Value;
        }

        if (_networkRotationDirection.Value != Vector3.zero)
        {
            Quaternion deltaRotation = Quaternion.Euler(_networkRotationDirection.Value * Time.fixedDeltaTime);
            _rb.MoveRotation(_rb.rotation * deltaRotation);
        }
    }

    private bool ActiveMeleeActionKey()
    {
        return Input.GetKey(KeyCode.Space);
    }

    [ServerRpc]
    private void UpdateClientPositionAndRotationServerRpc(Vector3 newPosition)
    {
        _networkPositionDirection.Value = newPosition;
    }

    //[ServerRpc]
    //private void UpdatePlayerStateServerRpc(PlayerState state)
    //{
    //    _networkPlayerState.Value = state;
    //}

    [ServerRpc]
    private void UpdateHealthServerRpc(int damage, ulong clientId)
    {
        var clientWithDamage = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerControl>();

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
    private void NotifyHealthChangedClientRpc(int damage, ClientRpcParams clientRpcParams = default)
    {
        if (IsOwner) return;

        Debug.Log($"Client received {damage} damage");
    }
}


