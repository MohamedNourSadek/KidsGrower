using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tearable : MonoBehaviour
{
    [SerializeField] protected Animator animator;

    public int tearDownTime = 1;
    protected int tearingDownCount = 0;
    protected int maxTearDownCount = 3;

    //Interface
    public void TearDown()
    {
        TearEffects();
        tearingDownCount++;
        SpawnResource();

        if (tearingDownCount >= maxTearDownCount)
            Destroy(this.gameObject);
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
}
