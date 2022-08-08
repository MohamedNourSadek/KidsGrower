using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : Pickable
{
    [SerializeField] int currentEnergy;
    [SerializeField] int totalEnergy = 150;

    [SerializeField] GroundDetector _ground;
    
    private void Awake()
    {
        currentEnergy = totalEnergy;
    }

    public bool OnGround()
    {
        return _ground.IsOnGroud(_myBody);
    }
    public int GetEnergy()
    {
        if (currentEnergy > 0f)
        {
            currentEnergy -= 1;
            return 1;
        }
        else
        {
            if(this)
                Destroy(this.gameObject);
            return 0;
        }
    }
    public bool HasEnergy()
    {
        return currentEnergy > 0 ? true : false;
    }


}
