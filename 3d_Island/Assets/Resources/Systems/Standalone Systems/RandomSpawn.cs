using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    [SerializeField] Vector2 randomRange = new Vector2(120f, 240f);
    private void Awake()
    {
        StartCoroutine(SpawnEveryT());
    }

    IEnumerator SpawnEveryT()
    {
        while(true) 
        {
            float time = UnityEngine.Random.Range(randomRange.x, randomRange.y);

            yield return new WaitForSecondsRealtime(time);

            bool exists = false;

            List<GameObject> objs = new List<GameObject>();

            foreach(GameObject obj in objs)
            {
                if(obj.tag == "Tree" || obj.tag == "Rock")
                {
                    exists = true;
                }
            }

            if(exists == false)
            {
                GameManager.instance.SpawnTree(this.transform.position);
            }
        }
    }


}
