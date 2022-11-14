using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class CraftingPanel : MenuPanel, ICreator
{
    [SerializeField] CreationSystem creationSystem;

    //Menu Panel Interface
    public override void Initialize()
    {
        base.Initialize();
        creationSystem.Initialize(this);
    }
    public override void FillFunctions()
    {
        base.FillFunctions();

        GetButton("Back").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Game0")));
        GetButton("Back").onClick.AddListener(new UnityAction(() => GameManager.instance.SetPlaying(true)));
        GetButton("Back").onClick.AddListener(new UnityAction(() => GameManager.instance.SetBlur(false)));
    }
    public void OnCreatePress()
    {
        creationSystem.CreateAndStore();
    }
}
