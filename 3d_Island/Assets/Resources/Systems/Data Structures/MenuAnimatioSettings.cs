using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MenuAnimatioSettings
{
    [SerializeField] public Vector3 offScale = new Vector3(1, 0, 0);
    [SerializeField] public Vector3 onScale = new Vector3(1, 1, 1);
    [SerializeField] public float InAnimationTime = 0.3f;
    [SerializeField] public float OutAnimationTime = 0.3f;
    [SerializeField] public LeanTweenType InAnimationCurve = LeanTweenType.easeSpring;
    [SerializeField] public LeanTweenType OutAnimationCurve = LeanTweenType.easeSpring;

}