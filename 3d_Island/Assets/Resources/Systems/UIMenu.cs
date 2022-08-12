using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DynamicBackgroundInfo
{
    public Vector2 position;

    public float alpha;
    public Color32 color;
}

public class UIMenu : MonoBehaviour
{ 
    [HeaderAttribute("Movable Background")]
    [SerializeField] Image _movableBackground;
    [SerializeField] Image _frame;
    public LeanTweenType _positionAnimationStyle;
    public LeanTweenType _colorAnimationStyle;
    [SerializeField] List<DynamicBackgroundInfo> _animationkeys;
    [SerializeField] float _speed = 1f;


    Vector2 screenCenterPoint;
    int current = 0;


    private void Awake()
    {
        screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        StartMovableLoop();
    }
    void StartMovableLoop()
    {
        LeanTween.value(_movableBackground.gameObject, _movableBackground.color.a, _animationkeys[current].alpha, 1f / _speed).setOnUpdate((float val) => {

            Image r = _movableBackground;
            Color c = r.color;
            c.a = val;
            r.color = c;
        }).
        setEase(_colorAnimationStyle);

        LeanTween.value(_frame.gameObject, _frame.color, _animationkeys[current].color, 1f / _speed).setOnUpdate((Color val) => {

            Image r = _frame;
            Color c = r.color;
            c = val;
            r.color = c;
        }).
        setEase(_colorAnimationStyle);  

        int id = LeanTween.
            move(_movableBackground.gameObject, screenCenterPoint + _animationkeys[current].position, 1f / _speed).
            setEase(_positionAnimationStyle).id;
        
        LTDescr process = LeanTween.descr(id);
        process.setOnComplete(StartMovableLoop);


        if (current == _animationkeys.Count - 1)
            current = 0;
        else
            current++;
    }
}
