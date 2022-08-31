using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextAnimation : MonoBehaviour
{
    [SerializeField] List<TextAnimationKey> keys = new List<TextAnimationKey>();

    TextMeshProUGUI text;

    private void OnEnable()
    {
        text = GetComponent<TextMeshProUGUI>();

        StartCoroutine(animator());
    }

    int i = 0;
    IEnumerator animator()
    {
        while(true)
        {
            text.text = keys[i].text;
            yield return new WaitForSecondsRealtime(keys[i].time);

            if (i >= keys.Count - 1)
            {
                i = 0;
            }
            else
            {
                i++;
            }
        }
    }

}
