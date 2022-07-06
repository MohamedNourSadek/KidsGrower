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
    [SerializeField] float _MaxSpeed = 4;
    [Header("Jump Parameters Parameters")]
    [SerializeField] float _jumpForce;
    [SerializeField] float _onGroundThreshold;


    PlayerSystem _myPlayer;
    bool _onGround = true;

    public void Initialize(PlayerSystem _player)
    {
        _myPlayer = _player;
    }
    public void PreformMove(Vector2 _movementDirection)
    {
        if(_myPlayer._playerBody.velocity.magnitude <= _MaxSpeed)
        {
            Vector3 direction = (_myPlayer.transform.position - _myPlayer.GetCameraPosition());
            Vector3 _forwardAxis = new Vector3(direction.x, 0f, direction.z);
            Vector3 _rightAxis = Vector3.Cross(_forwardAxis, Vector3.up);

            _myPlayer._playerBody.AddForce( 1 * _acceleration * _movementDirection.y * _forwardAxis.normalized );
            _myPlayer._playerBody.AddForce(-1 * _acceleration * _movementDirection.x * _rightAxis.normalized );
        }
    }
    public void PreformJump()
    {
        if(_onGround)
        {
            _myPlayer._playerBody.AddForce(Vector2.up * _jumpForce);
        }

    }



    public void Update()
    {
        _onGround = DetectGround();
    }
    bool DetectGround()
    {
        RaycastHit _ray;
        Physics.Raycast(_myPlayer.transform.position + Vector3.up, Vector2.down, out _ray, _onGroundThreshold, _groundLayer);
        return (_ray.point.magnitude > 0);
    }
}
