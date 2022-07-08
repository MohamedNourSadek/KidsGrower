using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    [SerializeField] Rigidbody _playerBody;
    [SerializeField] MovementSystem _movementSystem;
    [SerializeField] CameraSystem _myCamera;
    [SerializeField] UIController _uiContoller;

    [Header("Pickable Parameters")]
    [SerializeField] float _minDistanceToPick = 1f;
    [SerializeField] float _throwForce = 20f;

    [Header("Objects References")]
    [SerializeField] GameObject _myHand;

    InputSystem _inputSystem = new InputSystem();

    List<Pickable> _allPickables = new List<Pickable>();
    List<Pickable> _nearPickables = new List<Pickable>();
    Pickable _objectInHand = new Pickable();
    

    //Initialization and refreshable functions
    private void Awake()
    {
        _inputSystem.Initialize(this);
        _movementSystem.Initialize(this);
        _myCamera.Initialize(this.gameObject);

        //Have a reference of all Pickables
        var _allPickablesArray = FindObjectsOfType<Pickable>();
        foreach (var _pickable in _allPickablesArray)
            _allPickables.Add(_pickable);
    }
    private void FixedUpdate()
    {
        _inputSystem.Update();
        _myCamera.Update();
        _movementSystem.Update();

        DetectNearPickables();
    }

    

    //Picking Pickables
    public void DetectNearPickables()
    {
        if (_objectInHand == null)
        {
            foreach (Pickable _pickable in _allPickables)
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

            _uiContoller.PickButton_Enable((_nearPickables.Count > 0));
            _uiContoller.ThrowButton_Enable(false);
        }
        else
        {
            foreach (Pickable _pickable in _allPickables)
                _pickable.PickablilityIndicator(false);
        }
    }
    public void PickObject()
    {
        _uiContoller.PickButton_SwitchMode(PickMode.Drop);
        _uiContoller.ThrowButton_Enable(true);

        _objectInHand = _nearPickables[0];
        _objectInHand.Pick(_myHand.transform);
    }
    public void DropObject()
    {
        _uiContoller.PickButton_SwitchMode(PickMode.Pick);
        _uiContoller.ThrowButton_Enable(false);

        _objectInHand.Drop();
        _objectInHand = null;
    }
    public void ThrowObject()
    {
        //Because Drop function removes the reference
        var _tempReference = _objectInHand;

        DropObject();

        _tempReference.GetComponent<Rigidbody>().AddForce(this.transform.forward * _throwForce, ForceMode.Impulse);

        Debug.Log("Thrown");
    }


    //Camera - Movement - Input Interface
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
    public void JumpInput()
    {
        _movementSystem.PreformJump();
    }
    public void RotateInput(float _deltaRotation)
    {
        _myCamera.RotateCamera(_deltaRotation);
    }
    public void PickInput()
    {
        if((_objectInHand == null) && _nearPickables.Count > 0)
        {
            PickObject();
        }
        else if(_objectInHand != null)
        {
            DropObject();
        }
    }
    public void ThrowInput()
    {
        ThrowObject();
    }
}
