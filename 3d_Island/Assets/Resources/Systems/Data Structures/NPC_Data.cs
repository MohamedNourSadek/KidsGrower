using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Data 
{
    public nVector3 position = new nVector3();
    public nQuaternion rotation = new nQuaternion();
    public float bornSince = 0;
    public float xp = 0;

    public void SpawnWithData(GameObject npcAsset)
    {
        ServicesProvider.Instantiate(npcAsset).GetComponent<NPC>().LoadData(this);
    }
    public static List<NPC_Data> GameToDate(NPC[] npcs)
    {
        List<NPC_Data> list = new List<NPC_Data>(); 

        foreach (NPC npc in npcs)
            list.Add(npc.GetData());

        return list;
    }
}
