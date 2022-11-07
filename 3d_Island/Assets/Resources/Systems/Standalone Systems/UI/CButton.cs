using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CButton : Button
{
    public static List<CButton> cButtons = new List<CButton>();
    public MenuAnimatioSettings animationSettings;

    protected override void Awake()
    {
        base.Awake();

        onClick.AddListener(OnClickAnimation);
        cButtons.Add(this);
        
        if(SoundManager.instance)
            SoundManager.instance.InitializeButton(this);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();

        cButtons.Remove(this);
    }
   
    void OnClickAnimation()
    {
        if (this.IsActive())
            StartCoroutine(OnPressAnimation());
    }
    IEnumerator OnPressAnimation()
    {
        this.gameObject.LeanScale(animationSettings.onScale - animationSettings.pressedIncrementScale, animationSettings.pressedAnimationTime).setEase(animationSettings.pressedAnimationCurve);

        yield return new WaitForSeconds(animationSettings.pressedAnimationTime);

        this.gameObject.LeanScale(animationSettings.onScale, animationSettings.pressedAnimationTime).setEase(animationSettings.pressedAnimationCurve);
    }

}
