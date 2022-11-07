using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tearable : MonoBehaviour
{
    protected bool tearingDown;
    protected int tearDownTime = 1;
    protected int tearingDownCount = 0;
    protected int maxTearDownCount = 3;

    public virtual void Shake()
    {

    }
    public void TearDown()
    {
        if (tearingDown == false)
            StartCoroutine(TearingDown());
    }
    IEnumerator TearingDown()
    {
        Shake();

        tearingDown = true;
        tearingDownCount++;
        yield return new WaitForSecondsRealtime(tearDownTime);
        tearingDown = false;

        GameManager.instance.SpawnWoodPack(this.transform.position + Vector3.up);

        if (tearingDownCount >= maxTearDownCount)
            Destroy(this.gameObject);
    }
}
