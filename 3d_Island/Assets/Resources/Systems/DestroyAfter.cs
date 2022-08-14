using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField] float time = 3f;
    float timeSinceSpawned = 0f;


    private void FixedUpdate()
    {
        if(timeSinceSpawned >= time)
            Destroy(this.gameObject);
        else
            timeSinceSpawned += Time.fixedDeltaTime;
    }
}
