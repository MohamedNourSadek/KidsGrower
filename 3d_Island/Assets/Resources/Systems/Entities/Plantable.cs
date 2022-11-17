using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public enum GroundTag { Ground, Farm}

public abstract class Plantable : Pickable
{
    [SerializeField] public float plantTime = 10;
    [SerializeField] public float plantDistance = 1f;
    [SerializeField] List<GroundTag> allowedToPlantOn = new();
    [SerializeField] VisualEffect growingProgress;
    [SerializeField] GameObject spawnEffectAsset;


    protected float plantedSince = 0f;
    protected bool planted = false;

    public override void Awake()
    {
        base.Awake();
        growingProgress.SetFloat("Progress", 0f);
    }

    public override void Pick(HandSystem _picker)
    {
        base.Pick(_picker);

        if (planted)
            CancelPlant();
    }
    public void Plant(Vector3 _plantLocation)
    {
        isPicked = false;
        planted = true;
        myBody.constraints = RigidbodyConstraints.FreezeAll;
        myBody.transform.rotation = Quaternion.identity;
        transform.position = _plantLocation;

        StartCoroutine(Planting());
    }
    protected virtual void CancelPlant()
    {
        plantedSince = 0;
        planted = false;
        myBody.constraints = RigidbodyConstraints.None;

        StopCoroutine(Planting());
    }
    protected IEnumerator Planting()
    {
        while ((plantedSince < plantTime) && !isPicked)
        {
            plantedSince += Time.fixedDeltaTime;

            PlantingUpdate();

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        
        if (!isPicked && (plantedSince >= plantTime))
        {
            Instantiate(spawnEffectAsset, this.transform.position, spawnEffectAsset.transform.rotation);

            OnPlantDone();
        }
        else
        {
            OnPlantCancel();
        }
    }
    protected IEnumerator DestroyMe(float _delay)
    {
        yield return new WaitForSecondsRealtime(_delay);
        Destroy(this.gameObject);
    }


    public bool IsOnPlatingGround(Vector3 _position)
    {
        RaycastHit _hit;

        Physics.Raycast(_position, Vector3.down, out _hit);

        if (_hit.collider)
            foreach (GroundTag _tag in allowedToPlantOn)
                if (_hit.collider.tag == _tag.ToString())
                    return true;

        return false;
    }
    protected abstract void OnPlantDone(); 
    protected virtual void OnPlantCancel()
    {
        growingProgress.SetFloat("Progress", 0f);
    }
    protected virtual void PlantingUpdate()
    {
        growingProgress.SetFloat("Progress", (plantedSince / plantTime));
    }
}
