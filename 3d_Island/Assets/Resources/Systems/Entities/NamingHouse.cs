using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamingHouse : MonoBehaviour
{
    PlayerSystem playerSystem;

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
                    UIController.instance.EditNPCStats(true);
                }
            }
        }
    }
    public void OnFinish()
    {
        GameManager.instance.SetPlaying(true);
        UIController.instance.EditNPCStats(false);
        UIController.instance.GetNPCStatsUI().name.text = UIController.instance.GetNewName();
        playerSystem.getNPCInHand().ChangeName( UIController.instance.GetNewName());
    }
}
