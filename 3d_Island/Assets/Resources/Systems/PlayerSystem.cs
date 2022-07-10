using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    //Editor Fields
    [SerializeField] Rigidbody _playerBody;
    [SerializeField] MovementSystem _movementSystem;
    [SerializeField] CameraSystem _myCamera;
    [SerializeField] HandSystem _handSystem;
    [SerializeField] UIController _uiController;

    InputSystem _inputSystem = new InputSystem();

    //Initialization and refreshable functions
    void Awake()
    {
        _inputSystem.Initialize(this);
        _movementSystem.Initialize(_playerBody, _myCamera.GetCameraTransform());
        _myCamera.Initialize(this.gameObject);
        _handSystem.Initialize(Pickable._allPickables);
    }

    void FixedUpdate()
    {
        _inputSystem.Update();
        _myCamera.Update();
        _movementSystem.Update();

        _handSystem.Update();
        UpdateUi();
    }
    void UpdateUi()
    {
        if (_handSystem._canDrop)
            _uiController.PickDropButton_SwitchMode(PickMode.Drop);
        else
            _uiController.PickDropButton_SwitchMode(PickMode.Pick);

        if (_handSystem._canPick || _handSystem._canDrop)
            _uiController.PickDropButton_Enable(true);
        else
            _uiController.PickDropButton_Enable(false);

        if (_handSystem._canThrow)
            _uiController.ThrowButton_Enable(true);
        else
            _uiController.ThrowButton_Enable(false);

        if (_handSystem._canPlant)
            _uiController.PlantButton_Enable(true);
        else
            _uiController.PlantButton_Enable(false);

        _uiController.JumpButton_Enable(_movementSystem.IsOnGroud());
    }

    ///(Movement-Input) Interface
    public void MoveInput(Vector2 _movementInput)
    {
        _movementSystem.PreformMove(_movementInput);
    }
    public void RotateInput(float _deltaRotation)
    {
        _myCamera.RotateCamera(_deltaRotation);
    }
    public void JumpInput()
    {
        _movementSystem.PreformJump();
    }
    public void PickInput()
    {
        if(_handSystem._canPick)
        {
            _handSystem.PickObject();
        }
        else if(_handSystem._canDrop)
        {
            _handSystem.DropObject();
        }
    }
    public void ThrowInput()
    {
        if(_handSystem._canThrow)
            _handSystem.ThrowObject(transform.forward);
    }
    public void PlantInput()
    {
        if(_handSystem._canPlant)
            _handSystem.PlantObject();
    }
}
