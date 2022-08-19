using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floating : MonoBehaviour
{
    [SerializeField] List<Vector3> animationKeys;
    [SerializeField] float speed;
    [SerializeField] LeanTweenType animationCurve;

    Vector3 initialPosition;
    int animationKey = 0;

    private void Awake()
    {
        initialPosition = this.gameObject.transform.position;
        Animate();
    }

    void Animate()
    {
        int _id = LeanTween.move(this.gameObject, initialPosition + animationKeys[animationKey], 1f / speed).setEase(animationCurve).id;

        LTDescr process = LeanTween.descr(_id);
        process.setOnComplete(Animate);

        if (animationKey == animationKeys.Count - 1)
            animationKey = 0;
        else
            animationKey++;
    }

}
