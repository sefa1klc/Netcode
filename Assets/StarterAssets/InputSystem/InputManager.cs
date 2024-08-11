using System;
using Unity.Netcode;
using Unity.Services.Qos.V2.Models;
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
    public bool Actions { get; set; }
    public bool RT_Input { get; set; }
    public bool hold_RT_Input { get; set; }

    [Header("Movement Settings")]
    public bool analogMovement;


    private InputActionMap _currentMap;
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _runAction;
    private InputAction _crouchAction;
    private InputAction _jumpAction;
    private InputAction _RB_Action;
    private InputAction _RT_Action;
    private InputAction hold_RT_Action;



    private void Awake()
    {
        HideCursor();
        _currentMap = PlayerInput.currentActionMap;
        _moveAction = _currentMap.FindAction("Move");
        _lookAction = _currentMap.FindAction("Look");
        _runAction = _currentMap.FindAction("Run");
        _jumpAction = _currentMap.FindAction("Jump");
        _RB_Action = _currentMap.FindAction("RB");
        _RT_Action = _currentMap.FindAction("RT");
        hold_RT_Action = _currentMap.FindAction("Hold RT");

        _moveAction.performed += onMove;
        _lookAction.performed += onLook;
        _runAction.performed += onRun;
        _jumpAction.performed += onJump;
        _RB_Action.performed += onAction;
        _RT_Action.performed += onRT;
        hold_RT_Action.performed += onHoldRT;

        _moveAction.canceled += onMove;
        _lookAction.canceled += onLook;
        _runAction.canceled += onRun;
        _jumpAction.canceled += onJump;
        _RB_Action.canceled += onAction;
        _RT_Action.canceled += onRT;
        hold_RT_Action.canceled += onHoldRT;
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

    private void onAction(InputAction.CallbackContext context)
    {
        Actions = context.ReadValueAsButton();
    }

    private void onRT(InputAction.CallbackContext context)
    {
        RT_Input = context.ReadValueAsButton();
    }

    private void onHoldRT(InputAction.CallbackContext context)
    {
        hold_RT_Input = context.ReadValueAsButton();
    }

    private void Update()
    {
        RunnigControl();
        HandleAcitonInput();
        handleRTInput();
        handleChargedRTInput();
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

        if (Run && _playerManager._playerNetworkManager.currentStamina.Value > 1)
        {
            _playerManager._playerNetworkManager.isRunning.Value = true;
            _playerManager._playerNetworkManager.currentStamina.Value -= _runningStaminaCost * Time.deltaTime;
        }
        else
        {
            _playerManager._playerNetworkManager.isRunning.Value = false;
        }

        if (_playerManager._playerNetworkManager.isRunning.Value)
        {
            Run = true;
        }else
        { Run = false; }  
    }

    public void HandleAcitonInput()
    {
        if (Actions)
        {
            Actions = false;

            _playerManager._playerNetworkManager.SetCharacterActionHand(true);

            _playerManager._playerCombatManager.PerformWeaonBasedAction(_playerManager._playerInventoryManager._currentRightHandWeapon._RB_Actions,
                _playerManager._playerInventoryManager._currentRightHandWeapon);
        }
    }

    private void handleRTInput()
    {
        if (RT_Input)
        {
            RT_Input = false;

            _playerManager._playerNetworkManager.SetCharacterActionHand(true);

            _playerManager._playerCombatManager.PerformWeaonBasedAction(_playerManager._playerInventoryManager._currentRightHandWeapon._RT_Action,
                _playerManager._playerInventoryManager._currentRightHandWeapon);
        }
    }

    private void handleChargedRTInput()
    {
        if (_playerManager._isPerformingAction)
        {
            if (_playerManager._playerNetworkManager._isUsingRightHand.Value)
            {
                _playerManager._playerNetworkManager.isChargingAttack.Value = hold_RT_Input;
            } 
        }
    }
}
