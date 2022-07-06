using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class MovementSystem
{
    [SerializeField] LayerMask _groundLayer;

    [Header("Movement Parameters")]
    [SerializeField] float _speed;
    [Header("Jump Parameters Parameters")]
    [SerializeField] float _jumpForce;
    [SerializeField] float _onGroundThreshold;


    [System.NonSerialized] public PlayerSystem _myPlayer;
    bool _onGround = true;


    public void PreformMove(Vector2 _movementDirection)
    {


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
