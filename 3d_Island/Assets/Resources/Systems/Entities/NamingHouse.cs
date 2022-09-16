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
                    UIGame.instance.EditNPCStats(true);
                }
            }
        }
    }
    public void OnFinish()
    {
        GameManager.instance.SetPlaying(true);
        UIGame.instance.EditNPCStats(false);
        UIGame.instance.GetNPCStatsUI().name.text = UIGame.instance.GetUiName();
        playerSystem.getNPCInHand().ChangeName( UIGame.instance.GetUiName());
    }
}
