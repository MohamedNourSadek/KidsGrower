using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    [SerializeField] public Rigidbody _playerBody;
    [SerializeField] MovementSystem _movementSystem;
    [SerializeField] CameraSystem _myCamera;

    InputSystem _inputSystem = new InputSystem();

    private void Awake()
    {
        Cursor.visible = false;

        _inputSystem.Initialize(this);
        _movementSystem.Initialize(this);
        _myCamera.Initialize(this);
    }

    private void FixedUpdate()
    {
        _inputSystem.Update();
        _myCamera.Update();
        _movementSystem.Update();
    }

    
    public Transform GetCameraTransform()
    {
        return _myCamera.GetCameraTransform();
    }
    public void MoveInput(Vector2 _movementInput)
    {
        _movementSystem.PreformMove(_movementInput);
    }
    public void JumpInput()
    {
        _movementSystem.PreformJump();
    }
    public void RotateInput(float _deltaRotation)
    {
        _myCamera.RotateCamera(_deltaRotation);
    }
}
