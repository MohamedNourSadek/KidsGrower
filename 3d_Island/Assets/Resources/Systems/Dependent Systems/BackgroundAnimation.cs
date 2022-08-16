using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class BackgroundAnimation
{
    [SerializeField] float speed = 1f;
    [SerializeField] List<DynamicBackgroundInfo> animationkeys;
    [SerializeField] LeanTweenType positionAnimationStyle;
    [SerializeField] LeanTweenType colorAnimationStyle;
    [SerializeField] Image movableBackground;
    [SerializeField] Image frame;

    Vector2 screenCenterPoint;
    int currentBackgroundAnimationKey = 0;

    public void Initialize()
    {
        screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        AnimateBackground();
    }
    void AnimateBackground()
    {
        LeanTween.value(movableBackground.gameObject, movableBackground.color.a, animationkeys[currentBackgroundAnimationKey].alpha, 1f / speed).setOnUpdate((float val) => {

            Image r = movableBackground;
            Color c = r.color;
            c.a = val;
            r.color = c;
        }).
        setEase(colorAnimationStyle);

        LeanTween.value(frame.gameObject, frame.color, animationkeys[currentBackgroundAnimationKey].color, 1f / speed).setOnUpdate((Color val) => {

            Image r = frame;
            Color c = r.color;
            c = val;
            r.color = c;
        }).
        setEase(colorAnimationStyle);

        int _id = LeanTween.
            move(movableBackground.gameObject, screenCenterPoint + animationkeys[currentBackgroundAnimationKey].position, 1f / speed).
            setEase(positionAnimationStyle).id;

        LTDescr process = LeanTween.descr(_id);
        process.setOnComplete(AnimateBackground);


        if (currentBackgroundAnimationKey == animationkeys.Count - 1)
            currentBackgroundAnimationKey = 0;
        else
            currentBackgroundAnimationKey++;
    }
}
