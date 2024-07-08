using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

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
    [SerializeField] private float _rotationSpeed = 1.5f;
    [SerializeField] private float _runSpeedOffset = 2.0f;


    [SerializeField] private Vector2 _defaulPositionRange = new Vector2(-4, 4);


    [SerializeField] NetworkVariable<Vector3> _networkPositionDirection = new NetworkVariable<Vector3>();
    [SerializeField] NetworkVariable<Vector3> _networkRotationDirection = new NetworkVariable<Vector3>();


    [SerializeField] private NetworkVariable<PlayerState> _networkPlayerState = new NetworkVariable<PlayerState>();

    private CharacterController _controller;
    private Animator _anim;

    // client caches positions
    private Vector3 _oldInputPosition = Vector3.zero;
    private Vector3 _oldInputRotation = Vector3.zero;
    //private PlayerState _oldPlayerState = PlayerState.Idle;


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
    }
    private void Start()
    {
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(_defaulPositionRange.x, _defaulPositionRange.y), 0, Random.Range(_defaulPositionRange.x, _defaulPositionRange.y));
            
            CameraController.Instance.FallowPLayer(transform.Find("PlayerCameraRoot"));
        }
        
    }

    private void Update()
    {
        if (IsClient && IsOwner) 
        {
            ClientInput();
        }

        ClientMoveAndRotate();
        ClientVisuals();
        
    }

    private void ClientInput()
    {
        // left & right rotation
        Vector3 inputRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);

        // forward & backward direction
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float forwardInput = Input.GetAxis("Vertical");
        Vector3 inputPosition = direction * forwardInput;

        // change animation states
        if (forwardInput == 0)
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        else if (!ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        else if (ActiveRunningActionKey() && forwardInput > 0 && forwardInput <= 1)
        {
            inputPosition = direction * _runSpeedOffset;
            UpdatePlayerStateServerRpc(PlayerState.Run);
        }
        else if (forwardInput < 0)
            UpdatePlayerStateServerRpc(PlayerState.ReverseWalk);

        if (Input.GetMouseButtonDown(0))
        {
            UpdatePlayerStateServerRpc(PlayerState.Attack);
        }


        // let server know about position and rotation client changes
        if (_oldInputPosition != inputPosition ||
            _oldInputRotation != inputRotation)
        {
            _oldInputPosition = inputPosition;
            _oldInputRotation = inputRotation;
            UpdateClientPositionAndRotationServerRpc(inputPosition * _walkSpeed, inputRotation * _rotationSpeed);
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

    [ServerRpc]
    public void UpdateClientPositionAndRotationServerRpc(Vector3 newPosition, Vector3 newRotation)
    {
        _networkPositionDirection.Value = newPosition;
        _networkRotationDirection.Value = newRotation;
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState state)
    {
        _networkPlayerState.Value = state;
    }

}
