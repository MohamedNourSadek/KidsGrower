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
        _inputSystem.Initialize(this);
        _movementSystem.Initialize(this);
        _myCamera.Initialize(this);
    }
    private void Update()
    {
        _inputSystem.Update();
        _myCamera.Update();
        _movementSystem.Update();
    }

    public Vector3 GetCameraPosition()
    {
        return _myCamera.GetCameraPosition();
    }
    public void MoveInput(Vector2 _direction)
    {
        _movementSystem.PreformMove(_direction);
    }
    public void JumpInput()
    {
        _movementSystem.PreformJump();
    }
    public void RotateInput(float _deltaSlide)
    {
        _myCamera.RotatePerform(_deltaSlide);
    }
}
