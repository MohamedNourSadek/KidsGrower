using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem
{
    IInputUser _myUser;
    PlayerInputActions _inputActions;
    Vector2 _xyAxis;

    public void Initialize(IInputUser _user)
    {
        _myUser = _user;
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _inputActions.Player.Jump.performed += JumpInput;
        _inputActions.Player.Pick.performed += PickInput;
        _inputActions.Player.Throw.performed += ThrowInput;
        _inputActions.Player.Plant.performed += PlantInput;
        _inputActions.Player.Dash.performed += DashInput;
        _inputActions.Player.Pet.performed += PetInput;
        _inputActions.Player.Press.performed += PressInput;
    }
    public void Update()
    {
        MovementInput();
        RotateInput();
    }


    void PickInput(InputAction.CallbackContext obj)
    {
        _myUser.PickInput();
    }
    void JumpInput(InputAction.CallbackContext context)
    {
        _myUser.JumpInput();
    }
    void ThrowInput(InputAction.CallbackContext obj)
    {
        _myUser.ThrowInput();
    }
    void PlantInput(InputAction.CallbackContext obj)
    {
        _myUser.PlantInput();
    }
    void DashInput(InputAction.CallbackContext obj)
    {
        _myUser.DashInput();
    }
    void PetInput(InputAction.CallbackContext obj)
    {
        _myUser.PetInput();
    }
    void PressInput(InputAction.CallbackContext obj)
    {
        _myUser.PressInput();
    }
    public Vector2 GetMousePosition()
    {
        return _inputActions.Player.Hand.ReadValue<Vector2>();
    }


    void MovementInput()
    {
        _xyAxis = _inputActions.Player.Move.ReadValue<Vector2>();

        if (_xyAxis.magnitude > 0)
        {
            _myUser.MoveInput(_xyAxis);
        }
    }
    void RotateInput()
    {
        float _deltaRotate = _inputActions.Player.Rotate.ReadValue<float>();

        if (Mathf.Abs(_deltaRotate) > 0)
        {
            _myUser.RotateInput(_deltaRotate);
        }
    }
}

public interface IInputUser
{
    public void PetInput();
    public void DashInput();
    public void PlantInput();
    public void ThrowInput();
    public void JumpInput();
    public void PickInput();
    public void PressInput();
    public void MoveInput(Vector2 _movementInput);
    public void RotateInput(float _deltaRotate);
}