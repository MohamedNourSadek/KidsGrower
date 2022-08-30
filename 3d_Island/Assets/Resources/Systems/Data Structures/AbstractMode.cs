using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMode
{
    static float startPeriod = 3f;

    protected Mode_Data data;
    protected UIController controller;
    protected GameManager gameManager;

    public AbstractMode(Mode_Data data)
    {
        this.data = data;

        Initialize();
    }
    public Mode_Data GetModeData()
    {
        return data;
    }
    public void Update()
    {
        data.timeSinceStart += Time.deltaTime;
    }


    protected virtual void Initialize()
    {
        controller = MonoBehaviour.FindObjectOfType<UIController>();
        gameManager = MonoBehaviour.FindObjectOfType<GameManager>();

        OnLoad();

    }
    protected virtual void OnLoad()
    {
        gameManager.SetPlaying(true);
        gameManager.SetBlur(false);
    }
    protected virtual void OnStart()
    {
    }

}
