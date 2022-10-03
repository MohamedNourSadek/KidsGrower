using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] List<Transform> movementPoints = new List<Transform>();
    [SerializeField] LeanTweenType animationType = LeanTweenType.easeInOutCubic;

    int currentPoint = 0;
    int sign = -1;

    private void Start()
    {
        Explore();
    }
    void Explore()
    {
        Vector3 endPoint = movementPoints[currentPoint].position;

        if (currentPoint == movementPoints.Count - 1 || currentPoint == 0)
            sign *= -1;

        currentPoint += sign;

        float time = (endPoint - transform.position).magnitude;

        transform.LookAt(endPoint);

        int i = LeanTween.moveLocal(this.gameObject, endPoint, time / speed).setEase(animationType).id;

        LTDescr process = LeanTween.descr(i);

        process.setOnComplete(Explore);
    }
}
