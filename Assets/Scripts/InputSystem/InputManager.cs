using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private PlayerAcitons _playeractions;

    private void Awake()
    {
        _playeractions = new PlayerAcitons();
    }

    private void OnEnable()
    {
        _playeractions.Enable();
    }

    private void OnDisable()
    {
        _playeractions.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return _playeractions.Player.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return _playeractions.Player.Look.ReadValue<Vector2>();
    }
}
