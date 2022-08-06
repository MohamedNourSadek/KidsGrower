using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : Plantable
{
    protected override void OnPlantDone()
    {
        StartCoroutine(DestroyMe());
    }
    protected override void PlantingUpdate(){}
}
