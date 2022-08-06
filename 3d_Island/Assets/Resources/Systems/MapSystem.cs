using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CustomizingState { Detecting, Moving}

public class MapSystem : MonoBehaviour, IInputUser
{
    [SerializeField] List<Transform> _explorationPoints = new();
    [SerializeField] CustomizingState _customizingState = CustomizingState.Detecting;

    public static MapSystem instance;
    readonly InputSystem _inputSystem = new();
    bool _customizing = false;
    CustomizableObject _lastdetected;

    void Awake()
    {
        instance = this;
        _inputSystem.Initialize(this);
    }
    RaycastHit CastFromMouse()
    {
        RaycastHit _hit;

        Vector2 _mouse2D = _inputSystem.GetMousePosition();
        Vector3 _mousePosition = new(_mouse2D.x, _mouse2D.y, 2f);

        Ray ray = Camera.main.ScreenPointToRay(_mousePosition, Camera.MonoOrStereoscopicEye.Mono);

        Physics.Raycast(ray, out _hit);

        return _hit;
    }


    public void SetCustomizing(bool state)
    {
        _customizing = state;
    }
    public Vector3 GetRandomExplorationPoint()
    {
        var _randomLocation = Random.Range(0, _explorationPoints.Count);
        return _explorationPoints[_randomLocation].transform.position;
    }


    //Input Functions interceptors
    public void PressInput()
    {
        if (_customizing)
        {
            if ((_customizingState == CustomizingState.Detecting))
            {
                RaycastHit _hit = CastFromMouse();

                if (_hit.collider.GetComponentInParent<CustomizableObject>())
                {
                    _lastdetected = _hit.collider.GetComponentInParent<CustomizableObject>();

                    _customizingState = CustomizingState.Moving;

                    UIController.instance.CustomizeLog("Selected object: " + _lastdetected.name, Color.yellow);
                }
                else
                {
                    UIController.instance.CustomizeLog("No Object Detected", Color.white);
                }
            }
            else if (_customizingState == CustomizingState.Moving)
            {
                RaycastHit _hit = CastFromMouse();

                if (_hit.collider.gameObject.CompareTag("Ground"))
                {
                    _lastdetected.transform.position = _hit.point;
                    _customizingState = CustomizingState.Detecting;

                    UIController.instance.CustomizeLog("", Color.white);
                }
                else
                {
                    _customizingState = CustomizingState.Detecting;
                    PressInput();
                }
            }
        }
    }
    public void PetInput(){}
    public void DashInput(){}
    public void PlantInput(){}
    public void ThrowInput(){}
    public void JumpInput(){}
    public void PickInput(){}
    public void MoveInput(Vector2 _movementInput){}
    public void RotateInput(float _deltaRotate){}
}
