using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Plantable
{
    [SerializeField] float growingSpeed;

    [SerializeField] GameObject harvestPrefab;
    [SerializeField] GameObject model;

    Vector3 initialScale;
    private void Awake()
    {
        initialScale = model.transform.localScale;
    }

    public void LoadData(Seed_Data seed_Data)
    {
        transform.position = seed_Data.position.GetVector();
    }
    public Seed_Data GetData()
    {
        Seed_Data seed_data = new Seed_Data();

        seed_data.position = new nVector3(transform.position);

        return seed_data;
    }


    protected override void CancelPlant()
    {
        base.CancelPlant();

        model.transform.localScale = initialScale;
    }
    protected override void OnPlantDone()
    {
        Instantiate(harvestPrefab, transform.position, Quaternion.identity);
        StartCoroutine(DestroyMe(0f));
    }
    protected override void PlantingUpdate()
    {
        base.PlantingUpdate();

        model.transform.localScale = model.transform.localScale + (growingSpeed * Time.fixedDeltaTime * new Vector3(0, 1, 0));

    }
}
