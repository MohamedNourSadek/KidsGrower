using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.InputSystem.Composites;

[System.Serializable]
public class MenuPanel : MonoBehaviour
{
    [SerializeField] public string panalName;
    [SerializeField] public List<MenuPanelItem> menuPanelItems;
    [SerializeField] public MenuAnimatioSettings animationSettings;

    
    public bool active;
    public virtual void Initialize()
    {
        FillFunctions();
    }
    public virtual void FillFunctions()
    {
    }


    public virtual void OnActiveChange(bool _state)
    {

    }
    public bool IsActive()
    {
        return active;
    }
    public float GetActivationTime(bool _state)
    {
        active = _state;

        float _time;

        if (_state)
            _time = animationSettings.InAnimationTime;
        else
            _time = animationSettings.OutAnimationTime;

        ServicesProvider.instance.StartCoroutine(ActivatePanel_Coroutine(_state, _time));
        OnActiveChange(_state);
        return _time;
    }
    public Button GetButton(string name)
    {
        foreach(MenuPanelItem item in menuPanelItems)
        {
            if (item.name == name)
                return item.item.GetComponent<Button>();   
        }

        return null;
    }
    IEnumerator ActivatePanel_Coroutine(bool _state, float _time)
    {
        if (_state)
            gameObject.SetActive(_state);

        foreach (MenuPanelItem item in menuPanelItems)
            item.ActivateItem(_state);

        if (!_state)
        {
            yield return new WaitForSeconds(_time);
            gameObject.SetActive(_state);
        }

    }
}

