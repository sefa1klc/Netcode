using UnityEngine;

using StarterAssets;
using Camera;
using Cinemachine;
using UnityEngine.EventSystems;





#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using Unity.Netcode;


[RequireComponent(typeof(CharacterController))]

[RequireComponent(typeof(NetworkObject))]
public class FirstPersonController : NetworkBehaviour
{
    [Header("Player")]
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    [Range(0.0f, 0.3f)] public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;
    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Space(10)]
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;

    [Header("Player Grounded")]
    public bool Grounded = true;
    public float GroundedOffset = -0.14f;
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;

    [Header("Camera")]
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;
    public bool LockCameraPosition = false;
    [SerializeField] private float RotationSpeed = 10.0f;

    private float _cameraYaw;
    private float _cameraPitch;
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;
    [SerializeField] private Transform _CameraRoot;
    [SerializeField] private CinemachineVirtualCamera VCamera;
    [SerializeField] private float MouseSensitivity = 21.9f;
    private float _xRotation;


    private Vector3 _moveDirection = Vector3.zero;
    private Vector3 _oldMoveDirection = Vector3.zero;

    private int _xVelHash;
    private int _yVelHash;


    [SerializeField] private NetworkVariable<Vector3> _networkPositionDirection = new NetworkVariable<Vector3>();

    private Animator _animator;
    private CharacterController _controller;
    private InputManager _input;
    private GameObject _mainCamera;

    private const float _threshold = 0.01f;

    private bool _hasAnimator;



    public override void OnNetworkSpawn()
    {
        _controller = GetComponent<CharacterController>();
        _input = InputManager.Instance;
        _hasAnimator = TryGetComponent(out _animator);
        VCamera = UnityEngine.Camera.main.GetComponent<CinemachineVirtualCamera>();

        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        if (IsOwner)
        {
            _CameraRoot = transform.Find("PlayerCameraRoot");
            CameraManager.Instance.FallowPLayer(_CameraRoot);
        }
        

        AssignAnimationIDs();

        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private void Update()
    {
        if (IsOwner)
        {
            JumpAndGravity();
            GroundedCheck();
            HandleInput();
        }
    }

    private void LateUpdate()
    {
        SetupPlayerCamera();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        _xVelHash = Animator.StringToHash("X-Vel");
        _yVelHash = Animator.StringToHash("Y-Vel");
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(
            transform.position.x, 
            transform.position.y - GroundedOffset, 
            transform.position.z);

        Grounded = Physics.CheckSphere(spherePosition, 
            GroundedRadius, 
            GroundLayers, 
            QueryTriggerInteraction.Ignore);

        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }



    private void HandleInput()
    {
        float targetSpeed = _input.Run ? SprintSpeed : MoveSpeed;

        if (_input.Move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.Move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // Calculate the movement direction
        Vector3 moveDirection = new Vector3(_input.Move.x, 0.0f, _input.Move.y).normalized;
        Vector3 targetDirection = moveDirection.x * transform.right + moveDirection.z * transform.forward;

       

        // Move the character
        Vector3 movement = targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime;
        _controller.Move(movement);

        if (_hasAnimator)
        {
            float xVel = moveDirection.x * _speed;
            float yVel = moveDirection.z * _speed;

            _animator.SetFloat("X-Vel", xVel);
            _animator.SetFloat("Y-Vel", yVel);
        }

        if (_oldMoveDirection != moveDirection)
        {
            _oldMoveDirection = moveDirection;
            UpdateClientPositionAndRotationServerRpc(moveDirection);
        }
    }


    private void JumpAndGravity()
    {
        if (Grounded)
        {
            _fallTimeoutDelta = FallTimeout;

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }

            if (_input.Jump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            _input.Jump = false;
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime;
        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }
    }

    [ClientRpc]
    private void UpdateClientPositionAndRotationClientRpc(Vector3 position, Vector3 rotation)
    {
        if (!IsOwner)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateClientPositionAndRotationServerRpc(Vector3 newPosition)
    {
        _networkPositionDirection.Value = newPosition;
    }


    private void SetupPlayerCamera()
    {
        if (!_hasAnimator || _CameraRoot == null) return;

        var Mouse_X = _input.Look.x;
        var Mouse_Y = _input.Look.y;

        if (VCamera != null)
        {
            Transform virtualCameraTransform = VCamera.transform;
            virtualCameraTransform.position = _CameraRoot.position;

            _xRotation -= Mouse_Y * MouseSensitivity * Time.smoothDeltaTime;
            _xRotation = Mathf.Clamp(_xRotation, TopClamp, BottomClamp);

            Quaternion targetRotation = Quaternion.Euler(0, virtualCameraTransform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

            virtualCameraTransform.localRotation = Quaternion.Euler(_xRotation, 0, 0);

            // Karakterin yatay dönüþünü yap
            transform.Rotate(Vector3.up * Mouse_X * MouseSensitivity * Time.smoothDeltaTime);
        }
    }
}