using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class MovementSystem
{

    [SerializeField] GroundDetector _groundDetector;

    [Header("Movement Parameters")]
    [SerializeField] float _acceleration = 1250;
    [SerializeField] float _maxSpeed = 3;
    [SerializeField] float _rotationSpeed = 6;

    [Header("Jump Parameters")]
    [SerializeField] float _jumpForce = 13000;

    [Header("Dash parameters")]
    [SerializeField] float _dashRechargeTime = 1f;
    [SerializeField] float _dashForce = 13000;


    Rigidbody _body;
    Transform _lookDirection;

    Quaternion _finalAngle;
    bool _onGround = true;
    float _timeSinceLastDash = 0f;
    bool _dashedMidAir = false;
    
    //Interface to the controller
    public void Initialize(Rigidbody _body, Transform _lookdireciton)
    {
        _groundDetector.Initialize();
        this._body = _body;
        _lookDirection = _lookdireciton;
    }
    public void Update()
    {
        RotatePlayer();

        _onGround = _groundDetector.IsOnGroud(_body);

        if (_onGround)
            _dashedMidAir = false;

        _timeSinceLastDash += Time.fixedDeltaTime;
    }
    public void PreformMove(Vector2 _movementInput)
    {
        if(_body.velocity.magnitude <= _maxSpeed)
        {
            Vector3 _forwardAxis = new Vector3(_lookDirection.forward.x, 0f, _lookDirection.forward.z);
            Vector3 _rightAxis = Vector3.Cross(_forwardAxis, Vector3.up);

            _body.AddForce( 1 * _acceleration * _movementInput.y * _forwardAxis.normalized );
            _body.AddForce(-1 * _acceleration * _movementInput.x * _rightAxis.normalized );
        }

        _finalAngle = Quaternion.Euler(0f, _lookDirection.rotation.eulerAngles.y +  AdditionalMath.AngleFromY(_movementInput), 0f);
    }
    public void PreformJump()
    {
        if(_onGround)
        {
            _body.AddForce(Vector2.up * _jumpForce);
        }

    }
    public void PerformDash()
    {
        bool _canDash = false;

        if(IsDashable())
        {
            _timeSinceLastDash = 0f;
            _dashedMidAir = true;
            _canDash = true;
        }

        if(_canDash)
            _body.AddForce(_body.transform.forward * _dashForce);
    }

    public bool IsDashable()
    {
        return (_onGround && _timeSinceLastDash >= _dashRechargeTime) ||
               (!_onGround && !_dashedMidAir);
    }
    public bool IsOnGround()
    {
        return _onGround;
    }

    void RotatePlayer()
    {
        _body.transform.rotation = Quaternion.Lerp(_body.transform.rotation, _finalAngle, Time.fixedDeltaTime * _rotationSpeed);
    }
}
