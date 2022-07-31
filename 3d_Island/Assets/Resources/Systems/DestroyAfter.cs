using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    [SerializeField] float _time = 3f;
    float _timeSinceSpawned = 0f;


    private void Update()
    {
        if(_timeSinceSpawned >= _time)
            Destroy(this.gameObject);
        else
            _timeSinceSpawned += Time.deltaTime;
    }
}
