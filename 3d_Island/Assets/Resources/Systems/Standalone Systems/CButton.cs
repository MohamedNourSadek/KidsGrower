using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CButton : Button
{
    public static List<CButton> cButtons = new List<CButton>();

    protected override void Awake()
    {
        base.Awake();
        cButtons.Add(this);
        SoundManager.instance.InitializeButton(this);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();

        cButtons.Remove(this);
    }
}
