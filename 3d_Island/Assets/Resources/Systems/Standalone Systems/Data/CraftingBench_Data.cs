using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBench_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();

    public static List<CraftingBench_Data> GameToDate(CraftingBench[] craftingBenches)
    {
        List<CraftingBench_Data> list = new List<CraftingBench_Data>();

        foreach (CraftingBench craftingBench in craftingBenches)
            list.Add(craftingBench.GetData());

        return list;
    }
}
