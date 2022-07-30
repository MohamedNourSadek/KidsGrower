using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapSystem : MonoBehaviour
{
    public static List<Transform> ExplorationPoints = new List<Transform>();

    [SerializeField] List<Transform> _explorationPoints = new List<Transform>();


    private void Awake()
    {
        ExplorationPoints = _explorationPoints;
    }

    public static Vector3 GetRandomExplorationPoint()
    {
        var _randomLocation = Random.Range(0, MapSystem.ExplorationPoints.Count);
        return MapSystem.ExplorationPoints[_randomLocation].transform.position;
    }

   
}
