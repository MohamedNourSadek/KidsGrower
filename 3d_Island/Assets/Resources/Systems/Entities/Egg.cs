using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Plantable, ISavable
{
    [SerializeField] NPC babyNpcPrefab;
    [SerializeField] MeshRenderer myMesh;
    [SerializeField] float rottenThrusHold = 0.5f;


    float rottenness = 0f;


    public void LoadData(SaveStructure saveData)
    {
        Egg_Data egg_data = (Egg_Data)saveData;
        transform.position = egg_data.position.GetVector();
        transform.rotation = egg_data.rotation.GetQuaternion();
        rottenness = egg_data.rottenness;
        
        SetRottenness(rottenness);
    }
    public Egg_Data GetData()
    {
        Egg_Data egg_data = new Egg_Data();

        egg_data.position = new nVector3(transform.position);
        egg_data.rottenness = rottenness;
        egg_data.rotation = new nQuaternion(transform.rotation);
        return egg_data;
    }


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
            UIGame.instance.ShowRepeatingMessage("Rotten", this.gameObject.transform, 1f, 3f, new ConditionChecker(true));
            StartCoroutine(DestroyMe(0f));
        }
    }
}
