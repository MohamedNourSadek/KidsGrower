using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour, IController, IDetectable, IInputUser
{
    //Editor Fields
    [SerializeField] Rigidbody _playerBody;
    [SerializeField] MovementSystem _movementSystem;
    [SerializeField] CameraSystem _myCamera;
    [SerializeField] HandSystem _handSystem;
    [SerializeField] DetectorSystem _detector;
    InputSystem _inputSystem = new();
    InventorySystem _inventorySystem;
   
    //Initialization and refreshable functions
    void Awake()
    {
        _inventorySystem = new InventorySystem(this);
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
            UIController.instance.PickDropButton_SwitchMode(PickMode.Drop);
        else if (_handSystem._canPick)
            UIController.instance.PickDropButton_SwitchMode(PickMode.Pick);
        else if (_handSystem._detector.GetDetectable("Tree")._detectionStatus == DetectionStatus.VeryNear)
            UIController.instance.PickDropButton_SwitchMode(PickMode.Shake);
        else
            UIController.instance.PickDropButton_SwitchMode(PickMode.Pick);


        bool _canShake = (!_handSystem._canPick
                       && !_handSystem._canDrop
                       && (_handSystem._detector.GetDetectable("Tree")._detectionStatus == DetectionStatus.VeryNear));

        if (_handSystem._canPick || _handSystem._canDrop || _canShake)
            UIController.instance.PickDropButton_Enable(true);
        else
            UIController.instance.PickDropButton_Enable(false);

        if (_handSystem._canThrow)
            UIController.instance.ThrowButton_Enable(true);
        else
            UIController.instance.ThrowButton_Enable(false);

        if (_handSystem._canPlant)
            UIController.instance.PlantButton_Enable(true);
        else
            UIController.instance.PlantButton_Enable(false);

        UIController.instance.JumpButton_Enable(_movementSystem.IsOnGround());
        UIController.instance.DashButton_Enable(_movementSystem.IsDashable());
        UIController.instance.PetButton_Enable(_handSystem._canPet);
    }


    //Hand controller Interface implementations
    public Rigidbody GetBody()
    {
        return _playerBody;
    }
    public void StartCoroutine_Custom(IEnumerator routine)
    {
        base.StartCoroutine(routine);
    }


    ///(Movement-Input-Hand) Interface
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
            if(InventorySystem.IsStorable(_handSystem.GetNearest()))
            {
                _inventorySystem.Add((_handSystem.GetNearest()).GetComponent<IInventoryItem>());
            }
            else
            {
                _handSystem.PickObject();
            }
        }
        else if(_handSystem._canDrop)
        {
            _handSystem.DropObject();
        }
        else if(_handSystem._detector.GetDetectable("Tree")._detectionStatus == DetectionStatus.VeryNear)
        {
            ((TreeSystem)(_handSystem._detector.DetectableInRange("Tree"))).Shake();
        }
    }
    public void ThrowInput()
    {
        if(_handSystem._canThrow)
            _handSystem.ThrowObject(this.transform.position + (this.transform.forward));
    }
    public void PlantInput()
    {
        if (_handSystem._canPlant)
            _handSystem.PlantObject();
    }
    public void DashInput()
    {
        _movementSystem.PerformDash();
    }
    public void PetInput()
    {
        if(_handSystem._canPet)
            _handSystem.PetObject();
    }
    public void PressInput() { }
}
