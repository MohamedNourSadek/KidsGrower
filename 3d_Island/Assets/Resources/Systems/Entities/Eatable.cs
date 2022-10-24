using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eatable : Pickable
{
    [SerializeField] int totalValue = 150;
    
    public int currentValue;

    private void Awake()
    {
        currentValue = totalValue;
    }


    public int GetMore()
    {
        if (currentValue > 0f)
        {
            currentValue -= 1;
            return 1;
        }
        else
        {
            if (this)
                Destroy(this.gameObject);
            return 0;
        }
    }
    public bool HasMore()
    {
        return currentValue > 0 ? true : false;
    }
    public virtual void ApplyEffect(NPC holder)
    {
    }
}
