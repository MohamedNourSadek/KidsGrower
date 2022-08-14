using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CameraSystem 
{
    [SerializeField] Camera controlledCamera;
    [SerializeField] float distanceFromObject;
    [SerializeField] Vector2 cameraHeighLimits;
    [SerializeField] Vector2 rotationInputSensitivity;
    [SerializeField] float delay;
    [SerializeField] float rotationSpeed;
    [SerializeField] float cameraTilt;

    //Rotation factor is an angle that describe the camera rotation around the object.
    Vector2 rotationFactor = new();
    float currentRotationFactor = 0;
    Vector3 finalFollowedPosition;
    GameObject followedObject;


    //Interface
    public void Initialize(GameObject _target)
    {
        followedObject = _target;
        rotationFactor = new Vector2(0f, cameraHeighLimits.x);
        finalFollowedPosition = followedObject.transform.position + (Vector3.up * cameraTilt);
    }
    public Transform GetCameraTransform()
    {
        return controlledCamera.transform;
    }
    public void RotateCamera(Vector2 _deltaRotation)
    {
        rotationFactor.x += (rotationInputSensitivity.x * _deltaRotation.x);
        rotationFactor.y = Mathf.Clamp(rotationFactor.y - (rotationInputSensitivity.y * _deltaRotation.y), cameraHeighLimits.x, cameraHeighLimits.y);
    }
    public void Update()
    {
        TraslateCamera();
        controlledCamera.transform.LookAt(followedObject.transform.position);
    }

       
    //Internal Algorithms
    void TraslateCamera()
    {
        //Lerping the object position instead of the camera, because we want the camera to move only on its circle.
        finalFollowedPosition = Vector3.Lerp(finalFollowedPosition, followedObject.transform.position, Time.fixedDeltaTime * delay);
         
        currentRotationFactor = currentRotationFactor +  ((rotationFactor.x - currentRotationFactor) *  Time.fixedDeltaTime * rotationSpeed);

        //Move the camera to the final Position on the circle.
        controlledCamera.transform.position = finalFollowedPosition
                                                + (Vector3.up * rotationFactor.y) // Height
                                                + (distanceFromObject * Vector3.forward * Mathf.Cos(currentRotationFactor)) // r * cos(theta)
                                                + (distanceFromObject * Vector3.right * Mathf.Sin(currentRotationFactor));  // r * sin(theta)
    }
}
