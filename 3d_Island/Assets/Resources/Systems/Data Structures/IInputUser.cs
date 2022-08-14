using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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