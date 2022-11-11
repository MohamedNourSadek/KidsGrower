using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothCreator : MonoBehaviour, ISavable
{
    public void LoadData(SaveStructure saveData)
    {
        ClothCreator_Data clothCreator_Data = (ClothCreator_Data)saveData;
        transform.position = clothCreator_Data.position.GetVector();
        transform.rotation = clothCreator_Data.rotation.GetQuaternion();
    }
    public ClothCreator_Data GetData()
    {
        ClothCreator_Data clothCreator_Data = new ClothCreator_Data();

        clothCreator_Data.position = new nVector3(transform.position);
        clothCreator_Data.rotation = new nQuaternion(transform.rotation);

        return clothCreator_Data;
    }
}
