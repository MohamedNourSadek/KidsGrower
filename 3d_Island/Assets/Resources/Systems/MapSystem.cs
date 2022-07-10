using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSystem : MonoBehaviour
{
    public static List<Transform> ExplorationPoints = new List<Transform>();

    [SerializeField] List<Transform> _explorationPoints = new List<Transform>();


    private void Awake()
    {
        ExplorationPoints = _explorationPoints;
    }
}
