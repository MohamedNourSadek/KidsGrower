using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    //Editor Fields
    [SerializeField] Rigidbody _playerBody;
    [SerializeField] MovementSystem _movementSystem;
    [SerializeField] CameraSystem _myCamera;
    [SerializeField] UIController _uiController;

    [Header("Pickable Parameters")]
    [SerializeField] float _minDistanceToPick = 1f;
    [SerializeField] float _throwForce = 20f;
    [SerializeField] float _plantDistance = 1f;

    [Header("Objects References")]
    [SerializeField] GameObject _myHand;


    //Variables for Algorithms
    InputSystem _inputSystem = new InputSystem();
    List<Pickable> _nearPickables = new List<Pickable>();
    Pickable _objectInHand = new Pickable();
    bool _canPick;
    bool _canDrop;
    bool _canThrow;
    bool _canPlant;

    public List<Pickable> pickables = new List<Pickable>();

    //Initialization and refreshable functions
    void Awake()
    {
        _inputSystem.Initialize(this);
        _movementSystem.Initialize(this);
        _myCamera.Initialize(this.gameObject);
    }

    void FixedUpdate()
    {
        _inputSystem.Update();
        _myCamera.Update();
        _movementSystem.Update();

        DetectInteractability();
        UpdateUi();

        pickables = Pickable._allPickables;
    }
    void UpdateUi()
    {
        if (_canDrop)
            _uiController.PickDropButton_SwitchMode(PickMode.Drop);
        else
            _uiController.PickDropButton_SwitchMode(PickMode.Pick);

        if (_canPick || _canDrop)
            _uiController.PickDropButton_Enable(true);
        else
            _uiController.PickDropButton_Enable(false);

        if (_canThrow)
            _uiController.ThrowButton_Enable(true);
        else
            _uiController.ThrowButton_Enable(false);

        if (_canPlant)
            _uiController.PlantButton_Enable(true);
        else
            _uiController.PlantButton_Enable(false);

    }


    //Picking Pickables
    void DetectInteractability()
    {
        //Nested inside a try catch in case the list changed while using it.
        try
        {
            if (_objectInHand == null)
            {
                foreach (Pickable _pickable in Pickable._allPickables)
                {
                    bool _isNear = false;
                    bool _alreadyExists = false;

                    if ((_pickable.transform.position - this.transform.position).magnitude <= _minDistanceToPick)
                        _isNear = true;

                    if (_nearPickables.Contains(_pickable))
                        _alreadyExists = true;

                    if (!_pickable.IsPicked() && _isNear && !_alreadyExists)
                    {
                        _nearPickables.Add(_pickable);
                        _pickable.PickablilityIndicator(true);
                    }
                    else if (!_pickable.IsPicked() && !_isNear && _alreadyExists)
                    {
                        _nearPickables.Remove(_pickable);
                        _pickable.PickablilityIndicator(false);
                    }
                }


                _canPick = _nearPickables.Count > 0;
                _canThrow = false;
                _canPlant = false;

            }
            else
            {
                foreach (Pickable _pickable in Pickable._allPickables)
                    _pickable.PickablilityIndicator(false);

                _canPick = false;
                _canDrop = true;
                _canThrow = true;
            }
        }
        catch 
        {

        }
    }
    void PickObject()
    {
        _objectInHand = _nearPickables[0];
        _objectInHand.Pick(_myHand.transform);

        _canPick = false;
        _canDrop = true;
        _canThrow = true;

        if(_objectInHand.GetType() == typeof(Egg))
        {
            _canPlant = true;
        }

    }
    void DropObject()
    {
        _canDrop = false;
        _canThrow = false;

        _objectInHand.Drop();
        _objectInHand = null;
    }
    void ThrowObject()
    {
        //Because Drop function removes the reference
        var _tempReference = _objectInHand;

        DropObject();

        _tempReference.GetComponent<Rigidbody>().AddForce(this.transform.forward * _throwForce, ForceMode.Impulse);
    }
    void PlantObject()
    {
        var _tempReference = (Egg)_objectInHand;
        DropObject();

        _tempReference.Plant(this.transform.position + (this.transform.forward * _plantDistance));
    }


    ///(Camera/Movement-Input) Interface
    public Transform GetCameraTransform()
    {
        return _myCamera.GetCameraTransform();
    }
    public Rigidbody GetBody()
    {
        return _playerBody;
    }
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
        if(_canPick)
        {
            PickObject();
        }
        else if(_canDrop)
        {
            DropObject();
        }
    }
    public void ThrowInput()
    {
        if(_canThrow)
            ThrowObject();
    }
    public void PlantInput()
    {
        if(_canPlant)
            PlantObject();
    }
}
