using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIElement_NPC : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    static float visibilityCutoff = 10f;

    void Update()
    {
        if(PlayerSystem.instance)
        {
            if(Distance(PlayerSystem.instance.transform.position) > visibilityCutoff)
            {
                levelText.gameObject.SetActive(false);
            }
            else
            {
                levelText.gameObject.SetActive(true);
            }
        }

    }
    float Distance(Vector3 player)
    {
        return (this.transform.position - player).magnitude;
    }
}

 