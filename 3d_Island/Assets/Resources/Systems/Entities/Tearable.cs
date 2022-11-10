using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tearable : MonoBehaviour
{
    [SerializeField] protected Animator animator;

    protected bool tearingDown;
    protected int tearDownTime = 1;
    protected int tearingDownCount = 0;
    protected int maxTearDownCount = 3;

    //Interface
    public void TearDown()
    {
        if (tearingDown == false)
            StartCoroutine(TearingDown());
    }
    public virtual void TearEffects()
    {
        animator.SetTrigger("Shake");

        if (SoundManager.instance != null)
            SoundManager.instance.PlayRockShake(this.gameObject);
    }
    public virtual void SpawnResource()
    {

    }

    //Algorithm
    IEnumerator TearingDown()
    {
        TearEffects();

        tearingDown = true;
        tearingDownCount++;
        yield return new WaitForSecondsRealtime(tearDownTime);
        tearingDown = false;

        SpawnResource();

        if (tearingDownCount >= maxTearDownCount)
            Destroy(this.gameObject);
    }
}
