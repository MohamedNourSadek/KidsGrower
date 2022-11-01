using UnityEngine;

public interface IController
{
    public Rigidbody GetBody();
    public GroundDetector GetGroundDetector();
}