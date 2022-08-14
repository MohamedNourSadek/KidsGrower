using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Plantable
{
    [SerializeField] NPC babyNpcPrefab;
    [SerializeField] MeshRenderer myMesh;
    [SerializeField] float rottenThrusHold = 0.5f;


    float rottenness = 0f;

    public void SetRottenness(float _rottenness)
    {
        this.rottenness = _rottenness;
        myMesh.material.color = Color.Lerp(myMesh.material.color, Color.black, this.rottenness);
    }
    protected override void OnPlantDone()
    {
        if(rottenness <= rottenThrusHold)
        {
            Instantiate(babyNpcPrefab.gameObject, this.transform.position, babyNpcPrefab.transform.rotation);
            DestroyImmediate(this.gameObject);
        }
        else
        {
            UIController.instance.RepeatMessage("Rotten", this.gameObject.transform, 1f, 3f, new ConditionChecker(true));
            StartCoroutine(DestroyMe(0f));
        }
    }
    protected override void PlantingUpdate(){}
}
