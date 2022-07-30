using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Pickable
{
    [SerializeField] NPC _babyNpcPrefab;
    [SerializeField] public float _hatchTime = 30;
    [SerializeField] MeshRenderer myMesh;
    [SerializeField] float RottenThrusHold = 0.5f;

    float _plantedSince = 0f;
    bool _planted = false;
    float _rottenness = 0f;

    public void SetRottenness(float rottenness)
    {
        _rottenness = rottenness;
        
        ChangeMaterial();
    }

    public override void Pick(HandSystem _picker)
    {
        base.Pick(_picker);

        if(_planted)
            CancelPlant();

    }
    public void Plant(Vector3 _plantLocation)
    {
        _isPicked = false;
        _planted = true;
        _myBody.isKinematic = true;
        _myBody.transform.rotation = Quaternion.identity;
        transform.position = _plantLocation;

        StartCoroutine(Laying());
    }


    void ChangeMaterial()
    {
        myMesh.material.color = Color.Lerp(myMesh.material.color, Color.black, _rottenness);
    }
    IEnumerator Laying()
    {
        UIController.uIController.CreateProgressBar(this.gameObject, new Vector2(0f,_hatchTime), this.transform);

        while ((_plantedSince < _hatchTime) && !_isPicked)
        {
            _plantedSince += Time.fixedDeltaTime;
            UIController.uIController.UpdateProgressBar(this.gameObject, _plantedSince);
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        if(!_isPicked && (_plantedSince >= _hatchTime))
        {
            UIController.uIController.DestroyProgressBar(this.gameObject);
            Hatch();
        }
    }
    void CancelPlant()
    {
        _plantedSince = 0;
        _planted = false;

        UIController.uIController.DestroyProgressBar(this.gameObject);
        StopCoroutine(Laying());
    }
    void Hatch()
    {
        if(_rottenness <= RottenThrusHold)
        {
            Instantiate(_babyNpcPrefab.gameObject, this.transform.position, _babyNpcPrefab.transform.rotation);
            DestroyImmediate(this.gameObject);
        }
        else
        {
            UIController.uIController.RepeatMessage("Rotten", this.gameObject.transform, 1f, 3f, new ConditionChecker(true));
            StartCoroutine(DestroyMe());
        }

    }

    IEnumerator DestroyMe()
    {
        yield return new WaitForSecondsRealtime(1f);
        Destroy(this.gameObject);
    }

}
