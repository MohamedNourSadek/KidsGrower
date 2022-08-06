using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Plantable
{
    [SerializeField] NPC _babyNpcPrefab;
    [SerializeField] MeshRenderer myMesh;
    [SerializeField] float RottenThrusHold = 0.5f;


    float _rottenness = 0f;

    public void SetRottenness(float rottenness)
    {
        _rottenness = rottenness;
        myMesh.material.color = Color.Lerp(myMesh.material.color, Color.black, _rottenness);
    }
    protected override void OnPlantDone()
    {
        if(_rottenness <= RottenThrusHold)
        {
            Instantiate(_babyNpcPrefab.gameObject, this.transform.position, _babyNpcPrefab.transform.rotation);
            DestroyImmediate(this.gameObject);
        }
        else
        {
            UIController.instance.RepeatMessage("Rotten", this.gameObject.transform, 1f, 3f, new ConditionChecker(true));
            StartCoroutine(DestroyMe(0.5f));
        }
    }
    protected override void PlantingUpdate(){}
}
