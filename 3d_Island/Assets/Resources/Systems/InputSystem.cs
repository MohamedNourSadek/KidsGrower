using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem
{
    PlayerSystem _myPlayer;
    PlayerInputActions _inputActions;
    Vector2 _xyAxis;
    float _xyRotateAxis;

    public void Initialize(PlayerSystem _player)
    {
        _myPlayer = _player;
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _inputActions.Player.Jump.performed += JumpInput;
    }

    public void Update()
    {
        MovementInput();
        RotateInput();
    }


    
    void JumpInput(InputAction.CallbackContext context)
    {
        _myPlayer.JumpInput();
    }
    void RotateInput()
    {
        _xyRotateAxis = _inputActions.Player.Rotate.ReadValue<float>();

        if (_xyRotateAxis != 0)
        {
            _myPlayer.RotateInput(_xyRotateAxis);
        }
    }
    void MovementInput()
    {
        _xyAxis = _inputActions.Player.Move.ReadValue<Vector2>();

        if (_xyAxis.magnitude > 0)
        {
            _myPlayer.MoveInput(_xyAxis);
        }
    }
}
