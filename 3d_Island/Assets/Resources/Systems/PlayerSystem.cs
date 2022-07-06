using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    [SerializeField] public Rigidbody _playerBody;
    [SerializeField] MovementSystem _movementSystem;
    [SerializeField] CameraSystem _myCamera;

    InputSystem _inputSystem;
    CameraSystem _cameraSystem;

    private void Awake()
    {
        _inputSystem = new InputSystem(this);
        _movementSystem._myPlayer = this;
        _myCamera._myPlayer = this;
    }

    private void Update()
    {
        _inputSystem.Update();
        _movementSystem.Update();
    }


    public void MoveInput(Vector2 _direction)
    {
        _movementSystem.PreformMove(_direction);
    }
    public void JumpInput()
    {
        _movementSystem.PreformJump();
    }
}
