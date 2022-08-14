using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Plantable
{
    [SerializeField] GameObject harvestPrefab;

    [SerializeField] GameObject small;
    [SerializeField] GameObject medium;
    [SerializeField] GameObject final;

    protected override void CancelPlant()
    {
        base.CancelPlant();

        SetModel(0);
    }
    protected override void OnPlantDone()
    {
        Instantiate(harvestPrefab, transform.position, Quaternion.identity);
        StartCoroutine(DestroyMe(0f));
    }
    protected override void PlantingUpdate()
    {
        if (plantedSince >= (plantTime / 1.5f))
            SetModel(2);
        else if (plantedSince >= (plantTime/2f))
            SetModel(1);
        else if (plantedSince >= (plantTime / 4f))
            SetModel(0);
    }

    void SetModel(int _level)
    {
        if(_level == 2)
        {
            final.SetActive(true);
            medium.SetActive(false);
            small.SetActive(false);
        }
        else if(_level == 1){
            final.SetActive(false);
            medium.SetActive(true);
            small.SetActive(false);
        }
        else if (_level == 0)
        {
            final.SetActive(false);
            medium.SetActive(false);
            small.SetActive(true);
        }
    }
}
