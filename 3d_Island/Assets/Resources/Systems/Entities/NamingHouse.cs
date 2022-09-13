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
                    Debug.Log("Changing the name");
                }
            }
        }

    }

    public void ExitNaming()
    {
        if (playerSystem != null)
            GameManager.instance.SetPlaying(true);
    }

     void Update()
    {
        if(Input.GetKeyDown("x"))
        {
            ExitNaming();
        }
    }
}
