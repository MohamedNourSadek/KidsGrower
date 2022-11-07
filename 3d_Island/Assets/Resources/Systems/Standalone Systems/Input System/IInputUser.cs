using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputUser
{
    public bool activeInput
    {
        get;
        set;
    }

    public void PetInput();
    public void DashInput();
    public void PlantInput();
    public void ThrowInput();
    public void JumpInput();
    public void PickInput();
    public void PressDownInput();
    public void PressUpInput();
    public void MoveInput(Vector2 movementInput);
    public void RotateInput(Vector2 deltaRotate);
}