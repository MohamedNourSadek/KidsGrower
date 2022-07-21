using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour, IHandController
{
    //Editor Fields
    [SerializeField] Rigidbody _playerBody;
    [SerializeField] MovementSystem _movementSystem;
    [SerializeField] CameraSystem _myCamera;
    [SerializeField] HandSystem _handSystem;
    [SerializeField] UIController _uiController;
    [SerializeField] DetectorSystem _detector;
    readonly InputSystem _inputSystem = new();

    //Initialization and refreshable functions
    void Awake()
    {
        _inputSystem.Initialize(this);
        _movementSystem.Initialize(_playerBody, _myCamera.GetCameraTransform());
        _myCamera.Initialize(this.gameObject);
        _handSystem.Initialize(_detector, this);
    }
    void FixedUpdate()
    {
        _inputSystem.Update();
        _myCamera.Update();
        _movementSystem.Update();
        _detector.Update();
        _handSystem.Update();

        UpdateUi();
    }
    void UpdateUi()
    {
        if (_handSystem._canDrop)
            _uiController.PickDropButton_SwitchMode(PickMode.Drop);
        else if (_handSystem._canPick)
            _uiController.PickDropButton_SwitchMode(PickMode.Pick);
        else if (_handSystem._detector._treeDetectionStatus == TreeDetectionStatus.VeryNear)
            _uiController.PickDropButton_SwitchMode(PickMode.Shake);
        else
            _uiController.PickDropButton_SwitchMode(PickMode.Pick);


        bool _canShake = (!_handSystem._canPick
                       && !_handSystem._canDrop
                       && (_handSystem._detector._treeDetectionStatus == TreeDetectionStatus.VeryNear));



        if (_handSystem._canPick || _handSystem._canDrop || _canShake)
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


        _uiController.JumpButton_Enable(_movementSystem.IsOnGround());
        _uiController.DashButton_Enable(_movementSystem.IsDashable());
        _uiController.PetButton_Enable(_handSystem._canPet);
    }

    //Hand Interface implementations
    public Rigidbody GetBody()
    {
        return _playerBody;
    }
    public void startCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
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
        else if(_handSystem._detector._treeDetectionStatus == TreeDetectionStatus.VeryNear)
        {
            _handSystem._detector.TreeInRange().Shake();
        }
    }
    public void ThrowInput()
    {
        if(_handSystem._canThrow)
            _handSystem.ThrowObject(this.transform.position + (this.transform.forward));
    }
    public void PlantInput()
    {
        if(_handSystem._canPlant)
            _handSystem.PlantObject();
    }
    public void DashInput()
    {
        _movementSystem.PerformDash();
    }
    public void PetInput()
    {
        if(_handSystem._canPet)
        {
            _handSystem.PetObject();
        }
    }


}
