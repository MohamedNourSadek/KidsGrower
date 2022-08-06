using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GroundTag { Ground, Farm}

public abstract class Plantable : Pickable
{
    [SerializeField] public float _plantTime = 30;
    [SerializeField] public float _plantDistance = 1f;
    [SerializeField] List<GroundTag> _allowedToPlantOn = new();
    
    protected float _plantedSince = 0f;
    protected bool _planted = false;


    public override void Pick(HandSystem _picker)
    {
        base.Pick(_picker);

        if (_planted)
            CancelPlant();
    }
    public void Plant(Vector3 _plantLocation)
    {
        _isPicked = false;
        _planted = true;
        _myBody.isKinematic = true;
        _myBody.transform.rotation = Quaternion.identity;
        transform.position = _plantLocation;

        StartCoroutine(Planting());
    }
    protected void CancelPlant()
    {
        _plantedSince = 0;
        _planted = false;

        UIController.instance.DestroyProgressBar(this.gameObject);
        StopCoroutine(Planting());
    }
    protected IEnumerator Planting()
    {
        UIController.instance.CreateProgressBar(this.gameObject, new Vector2(0f, _plantTime), this.transform);

        while ((_plantedSince < _plantTime) && !_isPicked)
        {
            _plantedSince += Time.fixedDeltaTime;
            UIController.instance.UpdateProgressBar(this.gameObject, _plantedSince);

            PlantingUpdate();

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        if (!_isPicked && (_plantedSince >= _plantTime))
        {
            UIController.instance.DestroyProgressBar(this.gameObject);
            OnPlantDone();
        }
    }
    protected IEnumerator DestroyMe()
    {
        yield return new WaitForSecondsRealtime(1f);
        Destroy(this.gameObject);
    }


    public bool IsOnPlatingGround(Vector3 position)
    {
        RaycastHit hit;

        Physics.Raycast(position, Vector3.down, out hit);

        if (hit.collider)
            foreach (GroundTag tag in _allowedToPlantOn)
                if (hit.collider.tag == tag.ToString())
                    return true;

        return false;
    }
    protected abstract void OnPlantDone(); 
    protected abstract void PlantingUpdate(); //Update while planting
}
