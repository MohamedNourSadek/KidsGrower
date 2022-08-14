using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServicesProvider : MonoBehaviour
{
    public static ServicesProvider instance;
    void Awake()
    {
        instance = this;       
    }
}
