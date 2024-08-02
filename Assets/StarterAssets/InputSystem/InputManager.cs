using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    public PlayerManager _playerManager;
    [SerializeField] private PlayerInput PlayerInput;
    [SerializeField] private float _runningStaminaCost = 2;
    PlayerAcitons _acitons;

    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    public bool Run { get; private set; }
    public bool Jump { get; set; }

    [Header("Movement Settings")]
    public bool analogMovement;

    private InputActionMap _currentMap;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _runAction;
    private InputAction _crouchAction;
    private InputAction _jumpAction;

    private void Awake()
    {
        HideCursor();
        _currentMap = PlayerInput.currentActionMap;
        _moveAction = _currentMap.FindAction("Move");
        _lookAction = _currentMap.FindAction("Look");
        _runAction = _currentMap.FindAction("Run");
        _jumpAction = _currentMap.FindAction("Jump");

        _moveAction.performed += onMove;
        _lookAction.performed += onLook;
        _runAction.performed += onRun;
        _jumpAction.performed += onJump;

        _moveAction.canceled += onMove;
        _lookAction.canceled += onLook;
        _runAction.canceled += onRun;
        _jumpAction.canceled += onJump;
    }

    private void HideCursor()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void onMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }
    private void onLook(InputAction.CallbackContext context)
    {
        Look = context.ReadValue<Vector2>();
    }
    private void onRun(InputAction.CallbackContext context)
    {
        Run = context.ReadValueAsButton();
    }

    private void onJump(InputAction.CallbackContext context)
    {
        Jump = context.ReadValueAsButton();
    }

    private void Update()
    {
        RunnigControl();
    }

    private void OnEnable()
    {
        _currentMap.Enable();
    }

    private void OnDisable()
    {
        _currentMap.Disable();
    }

    public Vector2 GetMouseDelta()
    {
        return _acitons.Player.Look.ReadValue<Vector2>();
    }

    public void RunnigControl()
    {

        if (_playerManager == null)
        {
            return;
        }

        if (Run && _playerManager._characterNetworkManager.currentStamina.Value > 1)
        {
            _playerManager._characterNetworkManager.isRunning.Value = true;
            _playerManager._characterNetworkManager.currentStamina.Value -= _runningStaminaCost * Time.deltaTime;
        }
        else
        {
            _playerManager._characterNetworkManager.isRunning.Value = false;
        }

        if (_playerManager._characterNetworkManager.isRunning.Value)
        {
            Run = true;
        }else
        { Run = false; }  
    }
}
