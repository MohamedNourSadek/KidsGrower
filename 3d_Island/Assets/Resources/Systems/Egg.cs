using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : Pickable
{
    [SerializeField] NPC _babyNpcPrefab;
    [SerializeField] public float _hatchTime = 30;

    float _plantedSince = 0f;

    public override void Pick(Transform handPosition)
    {
        base.Pick(handPosition);
        CancelPlant();
    }
    public void Plant(Vector3 _plantLocation)
    {
        _isPicked = false;
        _myBody.isKinematic = true;
        _myBody.transform.rotation = Quaternion.identity;
        transform.position = _plantLocation;

        StartCoroutine(Laying());
    }


    IEnumerator Laying()
    {
        while ((_plantedSince < _hatchTime) && !_isPicked)
        {
            _plantedSince += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        if(!_isPicked && (_plantedSince >= _hatchTime))
            Hatch();
    }
    void CancelPlant()
    {
        _plantedSince = 0;
        StopCoroutine(Laying());
    }
    void Hatch()
    {
        Instantiate(_babyNpcPrefab.gameObject, this.transform.position, _babyNpcPrefab.transform.rotation);
        DestroyImmediate(this.gameObject);
    }

}
