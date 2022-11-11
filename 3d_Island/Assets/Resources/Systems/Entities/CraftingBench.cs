using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingBench : MonoBehaviour, ISavable
{
    public void LoadData(SaveStructure saveData)
    {
        CraftingBench_Data nameHouse_data = (CraftingBench_Data)saveData;
        transform.position = nameHouse_data.position.GetVector();
        transform.rotation = nameHouse_data.rotation.GetQuaternion();
    }
    public CraftingBench_Data GetData()
    {
        CraftingBench_Data namingHouse = new CraftingBench_Data();

        namingHouse.position = new nVector3(transform.position);
        namingHouse.rotation = new nQuaternion(transform.rotation);

        return namingHouse;
    }
}
