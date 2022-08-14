using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MenuPanel
{
    [SerializeField] public MenuPanelNames panalName;
    [SerializeField] public GameObject menuObjectl;
    [SerializeField] public List<MenuPanelButton> menuPanelItems;
    [SerializeField] MenuAnimatioSettings animationSettings;

    bool active;

    public bool IsActive()
    {
        return active;
    }

    public float ActivatePanel(bool _state)
    {
        active = _state;

        float _time = 0;

        if (_state)
            _time = animationSettings.InAnimationTime;
        else
            _time = animationSettings.OutAnimationTime;

        ServicesProvider.instance.StartCoroutine(ActivatePanel_Coroutine(_state, _time));

        return _time;
    }
    IEnumerator ActivatePanel_Coroutine(bool _state, float _time)
    {
        if (_state)
            menuObjectl.SetActive(_state);

        foreach (MenuPanelButton item in menuPanelItems)
            item.ActivateItem(_state, animationSettings);

        if (!_state)
        {
            yield return new WaitForSeconds(_time);
            menuObjectl.SetActive(_state);
        }

    }
}

