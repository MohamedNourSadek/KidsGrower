using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leg : MonoBehaviour
{
    LegSystem legSystem;

    public void Initialize(LegSystem legSystem)
    {
        this.legSystem = legSystem;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (legSystem != null)
            if (collider.tag == "Ground")
                legSystem.PlayWalk();
    }
}
