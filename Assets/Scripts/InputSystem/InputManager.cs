using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystem
{
    public class InputManager : Singleton<InputManager>
    {
        [SerializeField] private PlayerAcitons _playerActions;
        [SerializeField] private PlayerInput _playerInput;
        
        public Vector2 Movement { get; private set; }
        public Vector2 Look { get; private set; }

        private InputActionMap _currentmap;
        private InputAction _movementActions;
        private InputAction _lookActions;

        private void Awake()
        {
            _playerActions = new PlayerAcitons();
            _currentmap = _playerInput.currentActionMap;
            _movementActions = _currentmap.FindAction("Movement");
            _lookActions = _currentmap.FindAction("Look");

            _movementActions.performed += onMove;
            _lookActions.performed += onLook;
            
            _movementActions.canceled += onMove;
            _lookActions.canceled += onLook;

        }

        private void onLook(InputAction.CallbackContext obj)
        {
            Look = obj.ReadValue<Vector2>();
        }

        private void onMove(InputAction.CallbackContext obj)
        {
            Movement = obj.ReadValue<Vector2>();
        }

        private void OnEnable()
        {
            _currentmap.Enable();
        }

        private void OnDisable()
        {
            _currentmap.Disable();
        }

        public Vector2 GetMouseDelta()
        {
            return _playerActions.Player.Look.ReadValue<Vector2>();
        }
    }
}