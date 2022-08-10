using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CameraSystem 
{
    [SerializeField] Camera _controlledCamera;
    [SerializeField] float _distanceFromObject;
    [SerializeField] Vector2 _cameraHeighLimits;
    [SerializeField] Vector2 _rotationInputSensitivity;
    [SerializeField] float _delay;
    [SerializeField] float _rotationSpeed;
    [SerializeField] float _cameraTilt;

    //Rotation factor is an angle that describe the camera rotation around the object.
    Vector2 _rotationFactor = new();
    float _currentRotationFactor = 0;
    Vector3 _finalFollowedPosition;
    GameObject _followedObject;



    //Interface
    public void Initialize(GameObject _target)
    {
        _followedObject = _target;
        _rotationFactor = new Vector2(0f, _cameraHeighLimits.x);
        _finalFollowedPosition = _followedObject.transform.position + (Vector3.up * _cameraTilt);
    }
    public Transform GetCameraTransform()
    {
        return _controlledCamera.transform;
    }
    public void RotateCamera(Vector2 _deltaRotation)
    {
        _rotationFactor.x += (_rotationInputSensitivity.x * _deltaRotation.x);
        _rotationFactor.y = Mathf.Clamp(_rotationFactor.y - (_rotationInputSensitivity.y * _deltaRotation.y), _cameraHeighLimits.x, _cameraHeighLimits.y);
    }
    
    public void Update()
    {
        TraslateCamera();
        _controlledCamera.transform.LookAt(_followedObject.transform.position);
    }

       
    //Internal Algorithms
    void TraslateCamera()
    {
        //Lerping the object position instead of the camera, because we want the camera to move only on its circle.
        _finalFollowedPosition = Vector3.Lerp(_finalFollowedPosition, _followedObject.transform.position, Time.fixedDeltaTime * _delay);
         
        _currentRotationFactor = _currentRotationFactor +  ((_rotationFactor.x - _currentRotationFactor) *  Time.fixedDeltaTime * _rotationSpeed);

        //Move the camera to the final Position on the circle.
        _controlledCamera.transform.position = _finalFollowedPosition
                                                + (Vector3.up * _rotationFactor.y) // Height
                                                + (_distanceFromObject * Vector3.forward * Mathf.Cos(_currentRotationFactor)) // r * cos(theta)
                                                + (_distanceFromObject * Vector3.right * Mathf.Sin(_currentRotationFactor));  // r * sin(theta)
    }
}
