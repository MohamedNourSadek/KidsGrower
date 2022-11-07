using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamingHouse : MonoBehaviour, ISavable
{
    PlayerSystem playerSystem;


    public void LoadData(SaveStructure saveData)
    {
        NamingHouse_Data nameHouse_data = (NamingHouse_Data)saveData;
        transform.position = nameHouse_data.position.GetVector();
        transform.rotation = nameHouse_data.rotation.GetQuaternion();
    }
    public NamingHouse_Data GetData()
    {
        NamingHouse_Data namingHouse = new NamingHouse_Data();

        namingHouse.position = new nVector3(transform.position);
        namingHouse.rotation = new nQuaternion(transform.rotation);

        return namingHouse;
    }



    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if(collider.GetComponentInParent<PlayerSystem>() != null)
            {
                playerSystem = collider.GetComponentInParent<PlayerSystem>();

                if(playerSystem.GotNpcInHand())
                {
                    GameManager.instance.SetPlaying(false);
                    UIGame.instance.EditNPCStats(true);
                    GameManager.instance.OnNamingDone += OnFinish;
                }
            }
        }
    }
    public void OnFinish()
    {
        GameManager.instance.SetPlaying(true);
        UIGame.instance.EditNPCStats(false);
        UIGame.instance.GetNPCStatsUI().name.text = UIGame.instance.GetUiName();
        playerSystem.GetNPCInHand().ChangeName( UIGame.instance.GetUiName(), false);

        try
        {
            GameManager.instance.OnNamingDone -= OnFinish;
        }
        catch
        {
            Debug.Log("Error");
        }
    }
}
