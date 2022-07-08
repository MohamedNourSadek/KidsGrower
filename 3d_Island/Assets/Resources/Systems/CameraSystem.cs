using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CameraSystem 
{
    [SerializeField] Camera _controlledCamera;
    [SerializeField] float _distanceFromPlayer;
    [SerializeField] float _cameraHeight;
    [SerializeField] float _delay;
    [SerializeField] float _rotationInputSensitivity;
    [SerializeField] float _rotationSpeed;
    [SerializeField] float _cameraTilt;

    PlayerSystem _myPlayer;


    public void Initialize(PlayerSystem _player)
    {
        _myPlayer = _player;
        _finalPlayerPosition = _myPlayer.transform.position + (Vector3.up * _cameraTilt);
    }
    public Transform GetCameraTransform()
    {
        return _controlledCamera.transform;
    }


    //Rotation factor is an angle that describe the camera rotation around the player.
    public float _rotationFactor = 0;
    public float _currentRotationFactor = 0;

    Vector3 _finalPlayerPosition;
    public void TraslateCamera()
    {
        //Lerping the player position instead of the camera, because we want the camera to move only on its circle.
        _finalPlayerPosition = Vector3.Lerp(_finalPlayerPosition, _myPlayer.transform.position, Time.fixedDeltaTime * _delay);

        _currentRotationFactor = _currentRotationFactor +  ((_rotationFactor - _currentRotationFactor) *  Time.fixedDeltaTime * _rotationSpeed);

        //Move the camera to the final Position on the circle.
        _controlledCamera.transform.position = _finalPlayerPosition
                                                + (Vector3.up * _cameraHeight) // Height
                                                + (_distanceFromPlayer * Vector3.forward * Mathf.Cos(_currentRotationFactor)) // r * cos(theta)
                                                + (_distanceFromPlayer * Vector3.right * Mathf.Sin(_currentRotationFactor));  // r * sin(theta)
    }
    public void RotateCamera(float _deltaRotation)
    {
        _rotationFactor += (_rotationInputSensitivity * _deltaRotation);
    }

    public void Update()
    {
        TraslateCamera();
        _controlledCamera.transform.LookAt(_myPlayer.transform.position);
    }
}
