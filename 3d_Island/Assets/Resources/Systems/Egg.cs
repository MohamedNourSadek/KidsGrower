using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Pickable
{
    [SerializeField] NPC _babyNpcPrefab;
    [SerializeField] public float _hatchTime = 30;

    float _plantedSince = 0f;

    bool _planted = false;

    public override void Pick(Transform handPosition)
    {
        base.Pick(handPosition);

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
        Instantiate(_babyNpcPrefab.gameObject, this.transform.position, _babyNpcPrefab.transform.rotation);
        DestroyImmediate(this.gameObject);
    }

}
