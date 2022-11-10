using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcGhost : MonoBehaviour
{
    [SerializeField] float speed = 0.2f;
    [SerializeField] float arrivalDistance = 1f;

    bool moving = false;
    Vector3 destination = new Vector3();


    void Awake()
    {
        destination = MapSystem.instance.GetRandomExplorationPoint();
        moving = true;

        StartCoroutine(TakeDecision());
    }
    IEnumerator TakeDecision()
    {
        while(true)
        {
            if (moving)
            {
                this.transform.position = Vector3.Lerp(this.transform.position, destination, speed * Time.fixedDeltaTime);

                this.transform.LookAt(destination);

                if ((this.transform.position - destination).magnitude <= arrivalDistance)
                    moving = false;
            }
            else
            {
                destination = MapSystem.instance.GetRandomExplorationPoint();
                moving = true;
            }

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        

    }
}
