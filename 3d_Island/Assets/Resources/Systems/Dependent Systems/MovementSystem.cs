using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class MovementSystem
{
    [SerializeField] public GroundDetector groundDetector;

    [Header("Movement Parameters")]
    [SerializeField] float acceleration;
    [SerializeField] float maxSpeed;
    [SerializeField] float rotationSpeed;

    [Header("Jump Parameters")]
    [SerializeField] float jumpForce;

    [Header("Dash parameters")]
    [SerializeField] float dashRechargeTime;
    [SerializeField] float dashForce;

    Rigidbody body;
    Transform lookDirection;

    Quaternion finalAngle;
    bool onGround = true;
    float timeSinceLastDash = 0f;
    bool dashedMidAir = false;
    

    public void Initialize(Rigidbody _body, Transform _lookdireciton)
    {
        groundDetector.Initialize(_body);
        this.body = _body;
        lookDirection = _lookdireciton;
    }
    public void Update()
    {
        RotatePlayer();

        onGround = groundDetector.IsOnLayer(GroundLayers.Ground);

        if (onGround)
            dashedMidAir = false;

        timeSinceLastDash += Time.fixedDeltaTime;
    }


    //Interface
    public void PreformMove(Vector2 _movementInput)
    {
        if(body.velocity.magnitude <= maxSpeed)
        {
            Vector3 _forwardAxis = new Vector3(lookDirection.forward.x, 0f, lookDirection.forward.z);
            Vector3 _rightAxis = Vector3.Cross(_forwardAxis, Vector3.up);

            body.AddForce( 1 * acceleration * _movementInput.y * _forwardAxis.normalized );
            body.AddForce(-1 * acceleration * _movementInput.x * _rightAxis.normalized );
        }

        finalAngle = Quaternion.Euler(0f, lookDirection.rotation.eulerAngles.y +  AdditionalMath.AngleFromY(_movementInput), 0f);
    }
    public void PreformJump()
    {
        if(onGround)
        {
            body.AddForce(Vector2.up * jumpForce);
        }

    }
    public void PerformDash()
    {
        bool _canDash = false;
        if(IsDashable())
        {
            timeSinceLastDash = 0f;
            dashedMidAir = true;
            _canDash = true;
        }

        if(_canDash && body)
            body.AddForce(body.transform.forward * dashForce);
    }
    public bool IsDashable()
    {
        return (onGround && timeSinceLastDash >= dashRechargeTime) ||
               (!onGround && !dashedMidAir);
    }
    public bool IsOnGround()
    {
        return onGround;
    }
    public float GetSpeedRatio()
    {
       return Mathf.Clamp01((body.velocity.magnitude / maxSpeed));
    }


    //Internal Algorithms
    void RotatePlayer()
    {
        body.transform.rotation = Quaternion.Lerp(body.transform.rotation, finalAngle, Time.fixedDeltaTime * rotationSpeed);
    }
}
