using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum FacialExpressions { Idel, Smile, Angry, Blink}

[System.Serializable]
public class FacialControl
{
    [SerializeField] Animator animator;
    [SerializeField] Vector2 timeBetweenBlinks = new Vector2(.25f, 2f);

    public void Initialize()
    {
        ServicesProvider.instance.StartCoroutine(BlinkPeriodically());  
    }
    public void PlayExpression(FacialExpressions facial)
    {
        animator.SetTrigger(facial.ToString());
    }
    IEnumerator BlinkPeriodically()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(timeBetweenBlinks.x, timeBetweenBlinks.y));
            PlayExpression(FacialExpressions.Blink);
        }
    }
}
