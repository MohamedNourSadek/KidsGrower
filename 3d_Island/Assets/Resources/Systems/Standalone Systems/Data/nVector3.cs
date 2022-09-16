using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nVector3
{
    public float x;
    public float y;
    public float z;
    
    public nVector3()
    {

    }
    public nVector3(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }
    public nVector3(Vector3 position)
    {
        x = position.x;
        y = position.y;
        z = position.z;
    }
    public Vector3 GetVector()
    {
        return new Vector3(x, y, z);
    }

}
