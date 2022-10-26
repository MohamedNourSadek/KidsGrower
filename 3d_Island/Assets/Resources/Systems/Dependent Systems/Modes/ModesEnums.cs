using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum modes { FreeMode, UnderPopulation}

public class ModesEnums
{
    public static modes GetEnumFromString(string modeName)
    {
        foreach (var mode in Enum.GetValues(typeof(modes)))
        {
            if(mode.ToString() == modeName)
            {
                return ((modes)mode);
            }
        }

        return (modes.FreeMode);
    }

}