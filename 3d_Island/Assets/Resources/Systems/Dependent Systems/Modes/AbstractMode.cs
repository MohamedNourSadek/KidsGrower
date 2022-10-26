using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractMode
{
    protected Mode_Data data;

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
        OnLoad();
    }
    protected virtual void OnLoad()
    {
        GameManager.instance.SetPlaying(true);
        GameManager.instance.SetBlur(false);
    }
    protected virtual void OnFirstLoad()
    {
    }
}
