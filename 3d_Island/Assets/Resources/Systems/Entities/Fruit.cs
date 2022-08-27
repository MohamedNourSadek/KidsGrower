using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : Pickable
{
    [SerializeField] int totalEnergy = 150;
    [SerializeField] GroundDetector ground;

    int currentEnergy;

    private void Awake()
    {
        currentEnergy = totalEnergy;
    }


    public void LoadData(Fruit_Data fruit_data)
    {
        transform.position = fruit_data.position.GetVector();
    }
    public Fruit_Data GetData()
    {
        Fruit_Data fruit_data = new Fruit_Data();

        fruit_data.position = new nVector3(transform.position);

        return fruit_data;
    }

    public bool OnGround()
    {
        return ground.IsOnGroud(myBody);
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
