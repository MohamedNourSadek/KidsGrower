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

            BoxCaster boxCaster = ServicesProvider.instance.CreateBoxCaster(this.transform.position);
            yield return new WaitForSeconds(1f);
            List<GameObject> objs = boxCaster.GetObjectsInRange();

            if(ObjExists(objs) == false)
            {
                SpawnRandomObj();
            }

            Destroy(boxCaster.gameObject);
        }
    }
    bool ObjExists(List<GameObject> objs)
    {
        bool exists = false;

        foreach (GameObject obj in objs)
        {
            if (obj != null)
            {
                if (obj.tag == "Tree" || obj.tag == "Rock")
                {
                    exists = true;
                }
            }
        }

        return exists;
    }
    void SpawnRandomObj()
    {
        float random = UnityEngine.Random.Range(0f, 1f);

        if (random > 0.5f)
            GameManager.instance.SpawnRock(this.transform.position);
        else
            GameManager.instance.SpawnTree(this.transform.position);
    }
}
