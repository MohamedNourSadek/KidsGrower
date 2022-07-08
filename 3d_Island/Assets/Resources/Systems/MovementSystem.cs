using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class MovementSystem
{
    [SerializeField] LayerMask _groundLayer;

    [Header("Movement Parameters")]
    [SerializeField] float _acceleration;
    [SerializeField] float _maxSpeed;
    [SerializeField] float _rotationSpeed;

    [Header("Jump Parameters Parameters")]
    [SerializeField] float _jumpForce;
    [SerializeField] float _onGroundThreshold;


    PlayerSystem _myPlayer;
    Quaternion _finalAngle;
    bool _onGround = true;


    public void Initialize(PlayerSystem _player)
    {
        _myPlayer = _player;
    }
    public void PreformMove(Vector2 _movementInput)
    {
        if(_myPlayer.GetBody().velocity.magnitude <= _maxSpeed)
        {
            Vector3 _forwardAxis = new Vector3(_myPlayer.GetCameraTransform().forward.x, 0f, _myPlayer.GetCameraTransform().forward.z);
            Vector3 _rightAxis = Vector3.Cross(_forwardAxis, Vector3.up);

            _myPlayer.GetBody().AddForce( 1 * _acceleration * _movementInput.y * _forwardAxis.normalized );
            _myPlayer.GetBody().AddForce(-1 * _acceleration * _movementInput.x * _rightAxis.normalized );
        }

        _finalAngle = Quaternion.Euler(0f, _myPlayer.GetCameraTransform().rotation.eulerAngles.y +  AdditionalMath.AngleFromY(_movementInput), 0f);
    }
    public void PreformJump()
    {
        if(_onGround)
        {
            _myPlayer.GetBody().AddForce(Vector2.up * _jumpForce);
        }

    }
    public void Update()
    {
        RotatePlayer();
        _onGround = DetectGround();
    }



    void RotatePlayer()
    {
        _myPlayer.transform.rotation = Quaternion.Lerp(_myPlayer.transform.rotation, _finalAngle, Time.fixedDeltaTime * _rotationSpeed);
    }
    bool DetectGround()
    {
        RaycastHit _ray;
        Physics.Raycast(_myPlayer.transform.position + Vector3.up, Vector2.down, out _ray, _onGroundThreshold, _groundLayer);
        return (_ray.point.magnitude > 0);
    }
}
