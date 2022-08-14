using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MapSystem : MonoBehaviour
{
    [SerializeField] List<Transform> explorationPoints = new();

    public static MapSystem instance;
    void Start()
    {
        instance = this;
    }

    public Vector3 GetRandomExplorationPoint()
    {
        var _randomLocation = Random.Range(0, explorationPoints.Count);
        return explorationPoints[_randomLocation].transform.position;
    }

}
