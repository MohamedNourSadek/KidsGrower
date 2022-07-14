using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Test : MonoBehaviour
{
    [SerializeField] GameObject Target;
    NavMeshAgent myAgent;

    private void Awake()
    {
        myAgent = GetComponent<NavMeshAgent>();
    }


    private NavMeshPath path;
    private float elapsed = 0.0f;
    
    void Start()
    {
        path = new NavMeshPath();
        elapsed = 0.0f;
    }

    private void OnDrawGizmos()
    {
        myAgent.destination = Target.transform.position;

        if (elapsed > 1.0f)
        {
            elapsed -= 1.0f;
            NavMesh.CalculatePath(transform.position, Target.transform.position, NavMesh.AllAreas, path);
        }

        for (int i = 0; i < path.corners.Length - 1; i++)
            Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
    }



}
