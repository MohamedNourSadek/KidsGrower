using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystem : MonoBehaviour
{
    List<IInputUser> Subscribers = new();
    PlayerInputActions inputActions;

    static InputSystem instance;

    void Awake()
    {
        instance = this;

        inputActions = new PlayerInputActions();
        inputActions.Enable();
        inputActions.Player.Jump.performed += JumpInput;
        inputActions.Player.Pick.performed += PickInput;
        inputActions.Player.Throw.performed += ThrowInput;
        inputActions.Player.Plant.performed += PlantInput;
        inputActions.Player.Dash.performed += DashInput;
        inputActions.Player.Pet.performed += PetInput;
        inputActions.Player.Press.performed += PressInput;
    }
    void FixedUpdate()
    {
        MovementInput();
        RotateInput();
    }

    public static void SubscribeUser(IInputUser _user)
    {
        instance.Subscribers.Add(_user);
    }
    public static Vector2 GetMousePosition()
    {
        return instance.inputActions.Player.Hand.ReadValue<Vector2>();
    }


    void PickInput(InputAction.CallbackContext obj)
    {
        foreach(IInputUser _sub in Subscribers)
            _sub.PickInput();
    }
    void JumpInput(InputAction.CallbackContext context)
    {
        foreach (IInputUser _sub in Subscribers)
            _sub.JumpInput();
    }
    void ThrowInput(InputAction.CallbackContext obj)
    {
        foreach (IInputUser _sub in Subscribers)
            _sub.ThrowInput();
    }
    void PlantInput(InputAction.CallbackContext obj)
    {
        foreach (IInputUser _sub in Subscribers)
            _sub.PlantInput();
    }
    void DashInput(InputAction.CallbackContext obj)
    {
        foreach (IInputUser _sub in Subscribers)
            _sub.DashInput();
    }
    void PetInput(InputAction.CallbackContext obj)
    {
        foreach (IInputUser _sub in Subscribers)
            _sub.PetInput();
    }
    void PressInput(InputAction.CallbackContext obj)
    {
        foreach (IInputUser _sub in Subscribers)
            _sub.PressInput();
    }
    void MovementInput()
    {
        Vector2 _xyAxis = inputActions.Player.Move.ReadValue<Vector2>();

        if (_xyAxis.magnitude > 0)
        {
            foreach (IInputUser _sub in Subscribers)
                _sub.MoveInput(_xyAxis);
        }
    }
    void RotateInput()
    {
        Vector2 _deltaRotate = new Vector2();

        _deltaRotate.x = inputActions.Player.RotateX.ReadValue<float>();
        _deltaRotate.y = inputActions.Player.RotateY.ReadValue<float>();

        if (_deltaRotate.magnitude > 0)
        {
            foreach (IInputUser _sub in Subscribers)
                _sub.RotateInput(_deltaRotate);
        }
    }
}

