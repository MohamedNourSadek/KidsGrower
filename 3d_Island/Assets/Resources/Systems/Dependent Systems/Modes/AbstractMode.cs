using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if(data.firstStart == true)
        {
            data.timeSinceStart = 0f;
            OnFirstLoad();
        }
        else
        {
            var trees = ServicesProvider.FindObjectsOfType<TreeSystem>();
            var rocks = ServicesProvider.FindObjectsOfType<Rock>();

            foreach (TreeSystem tree in trees)
               ServicesProvider.instance.DestroyObject(tree.gameObject);
            foreach (Rock rock in rocks)
                ServicesProvider.instance.DestroyObject(rock.gameObject);

            GameManager.instance.SetPlaying(true);
            GameManager.instance.SetBlur(false);
        }


    }
    protected virtual void OnFirstLoad()
    {
        data.firstStart = false;
    }
}
