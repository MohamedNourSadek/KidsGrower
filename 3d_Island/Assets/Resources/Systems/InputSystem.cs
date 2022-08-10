using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    List<IInputUser> _users = new();
    PlayerInputActions _inputActions;
    Vector2 _xyAxis;

    static InputSystem instance;

    void Awake()
    {
        instance = this;

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
    void Update()
    {
        MovementInput();
        RotateInput();
    }

    public static void SubscribeUser(IInputUser _user)
    {
        instance._users.Add(_user);
    }
    
    void PickInput(InputAction.CallbackContext obj)
    {
        foreach(IInputUser user in _users)
            user.PickInput();
    }
    void JumpInput(InputAction.CallbackContext context)
    {
        foreach (IInputUser user in _users)
            user.JumpInput();
    }
    void ThrowInput(InputAction.CallbackContext obj)
    {
        foreach (IInputUser user in _users)
            user.ThrowInput();
    }
    void PlantInput(InputAction.CallbackContext obj)
    {
        foreach (IInputUser user in _users)
            user.PlantInput();
    }
    void DashInput(InputAction.CallbackContext obj)
    {
        foreach (IInputUser user in _users)
            user.DashInput();
    }
    void PetInput(InputAction.CallbackContext obj)
    {
        foreach (IInputUser user in _users)
            user.PetInput();
    }
    void PressInput(InputAction.CallbackContext obj)
    {
        foreach (IInputUser user in _users)
            user.PressInput();
    }
    public static Vector2 GetMousePosition()
    {
        return instance._inputActions.Player.Hand.ReadValue<Vector2>();
    }
    void MovementInput()
    {
        _xyAxis = _inputActions.Player.Move.ReadValue<Vector2>();

        if (_xyAxis.magnitude > 0)
        {
            foreach (IInputUser user in _users)
                user.MoveInput(_xyAxis);
        }
    }
    void RotateInput()
    {
        Vector2 _deltaRotate = new Vector2();

        _deltaRotate.x = _inputActions.Player.RotateX.ReadValue<float>();
        _deltaRotate.y = _inputActions.Player.RotateY.ReadValue<float>();

        if (_deltaRotate.magnitude > 0)
        {
            foreach (IInputUser user in _users)
                user.RotateInput(_deltaRotate);
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
    public void RotateInput(Vector2 _deltaRotate);
}