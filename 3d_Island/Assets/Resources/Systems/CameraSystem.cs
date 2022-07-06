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
    [SerializeField] float _rotationSpeed;
    PlayerSystem _myPlayer;

    public void Initialize(PlayerSystem _player)
    {
        _myPlayer = _player;
    }
    public Vector3 GetCameraPosition()
    {
        return _controlledCamera.transform.position;
    }


    public void MoveCamera(Vector3 _newPlayerPosition)
    {
        Vector3 _direction = (_myPlayer.transform.position - _controlledCamera.transform.position).normalized;
        Vector3 _xzDirection = new Vector3(_direction.x, 0f, _direction.z);

        Vector3 _finalPosition = _newPlayerPosition - (_myPlayer.transform.forward * _distanceFromPlayer) +
                                                (Vector3.up * _cameraHeight);


        _controlledCamera.transform.position = Vector3.Lerp(_controlledCamera.transform.position, _finalPosition, Time.fixedDeltaTime/_delay);

    }

    public void RotatePerform(float _delta)
    {

    }

    public void Update()
    {

        Debug.Log("Moving");
        MoveCamera(_myPlayer.transform.position);
    }
}
