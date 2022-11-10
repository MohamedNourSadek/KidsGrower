using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIPopUp : MonoBehaviour 
{
    [SerializeField] public TextMeshProUGUI header;
    [SerializeField] public TextMeshProUGUI message;
    [SerializeField] public Button button;
    [SerializeField] MenuAnimatioSettings animationOnAwake;


    private void OnEnable()
    {
        gameObject.LeanScale(animationOnAwake.offScale, 0f);
        gameObject.LeanScale(animationOnAwake.onScale, animationOnAwake.InAnimationTime).setEase(animationOnAwake.InAnimationCurve);
    }

}
