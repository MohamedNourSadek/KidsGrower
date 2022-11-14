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


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (collider.GetComponentInParent<PlayerSystem>() != null)
            {
                GameManager.instance.SetPlaying(false);
                GameManager.instance.SetBlur(true);
                UIGame.instance.OpenMenuPanel("Crafting Panel0");
            }
        }
    }
}
