using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Plantable
{
    [SerializeField] GameObject _harvestPrefab;

    [SerializeField] GameObject _small;
    [SerializeField] GameObject _medium;
    [SerializeField] GameObject _final;

    protected override void CancelPlant()
    {
        base.CancelPlant();

        SetModel(0);
    }
    protected override void OnPlantDone()
    {
        Instantiate(_harvestPrefab, transform.position, Quaternion.identity);
        StartCoroutine(DestroyMe(0f));
    }
    protected override void PlantingUpdate()
    {
        if (_plantedSince >= (_plantTime / 1.5f))
            SetModel(2);
        else if (_plantedSince >= (_plantTime/2f))
            SetModel(1);
        else if (_plantedSince >= (_plantTime / 4f))
            SetModel(0);
    }

    void SetModel(int _level)
    {
        if(_level == 2)
        {
            _final.SetActive(true);
            _medium.SetActive(false);
            _small.SetActive(false);
        }
        else if(_level == 1){
            _final.SetActive(false);
            _medium.SetActive(true);
            _small.SetActive(false);
        }
        else if (_level == 0)
        {
            _final.SetActive(false);
            _medium.SetActive(false);
            _small.SetActive(true);
        }
    }
}
