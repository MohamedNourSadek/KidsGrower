using System.Collections.Generic;
using UnityEngine;


public class BackgroundAnimation : MonoBehaviour
{
    [SerializeField] float speed = 1f;
    [SerializeField] List<DynamicBackgroundInfo> animationkeys;
    [SerializeField] LeanTweenType positionAnimationStyle;
    [SerializeField] LeanTweenType colorAnimationStyle;

    Vector2 screenCenterPoint;
    int currentBackgroundAnimationKey = 0;

    public void Awake()
    {
        screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        AnimateBackground();
    }
    void AnimateBackground()
    {
        int id = LeanTween.
            move(this.gameObject, screenCenterPoint + animationkeys[currentBackgroundAnimationKey].position, 1f / speed).
            setEase(positionAnimationStyle).id;

        LTDescr process = LeanTween.descr(id);
        process.setOnComplete(AnimateBackground);


        if (currentBackgroundAnimationKey == animationkeys.Count - 1)
            currentBackgroundAnimationKey = 0;
        else
            currentBackgroundAnimationKey++;
    }
}
