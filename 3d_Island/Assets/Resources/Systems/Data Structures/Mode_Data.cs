using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mode_Data 
{
    public Mode_Data() { }
    public Mode_Data(modes modeName, float timeSinceStart)
    {
        this.modeName = modeName;
        this.timeSinceStart = timeSinceStart;
    }

    public modes modeName;
    public float timeSinceStart;
}
