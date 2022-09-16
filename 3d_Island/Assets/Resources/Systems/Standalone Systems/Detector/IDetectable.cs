using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDetectable
{
    public string tag
    {
        get;
        set;
    }
    public GameObject GetGameObject();
}
