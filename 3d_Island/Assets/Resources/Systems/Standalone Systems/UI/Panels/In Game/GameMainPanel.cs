using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameMainPanel : MenuPanel
{
    public override void FillFunctions()
    {
        base.FillFunctions();

        GetButton("Play").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Game0")));
        GetButton("Play").onClick.AddListener(new UnityAction(() => GameManager.instance.SetBlur(false)));
        GetButton("Play").onClick.AddListener(new UnityAction(() => GameManager.instance.SetPlaying(true)));

        GetButton("Save").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Game0")));
        GetButton("Save").onClick.AddListener(new UnityAction(() => GameManager.instance.SetPlaying(true)));
        GetButton("Save").onClick.AddListener(new UnityAction(() => GameManager.instance.Save()));

        GetButton("Customize Map").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Customize0")));

        GetButton("Settings").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Settings0")));

        GetButton("Quit").onClick.AddListener(new UnityAction(() => GameManager.instance.OpenMainMenu()));
    }


}
