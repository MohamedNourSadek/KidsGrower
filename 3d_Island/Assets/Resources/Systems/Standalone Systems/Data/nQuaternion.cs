using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nQuaternion 
{
    public float x;
    public float y;
    public float z;
    public float w;

    public nQuaternion()
    {

    }
    public nQuaternion(int _x, int _y, int _z, int _w)
    {
        x = _x;
        y = _y;
        z = _z;
        w = _w;
    }
    public nQuaternion(Quaternion quaternion)
    {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }
    public Quaternion GetQuaternion()
    {
        return new Quaternion(x, y, z, w);  
    }
}
