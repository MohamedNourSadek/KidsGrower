using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoRoutineProvider : MonoBehaviour
{
    public static CoRoutineProvider instance;
    private void Awake()
    {
        instance = this;
    }
}
