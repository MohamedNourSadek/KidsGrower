using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class PanelsManager
{
    [SerializeField] string name;
    [SerializeField] bool updateNames;
    [SerializeField] bool overrideAnimations;
    [SerializeField] string defaultPanel;
    [SerializeField] MenuAnimatioSettings animationSettings;
    [SerializeField] public List<MenuPanel> menuPanels;
    string currentMenuPanel;


    public void Initialize()
    {
        //reset all first
        foreach (MenuPanel _panel in menuPanels)
        {
            _panel.menuObjectl.SetActive(false);

            if(overrideAnimations)
                _panel.animationSettings = animationSettings;
        }

        foreach (MenuPanel _panel in menuPanels)
        {
            foreach (var _item in _panel.menuPanelItems)
            {
                _item.Initialize(_panel.animationSettings);
            }
        }

        currentMenuPanel = defaultPanel;
        OpenMenuPanel(currentMenuPanel, false);
    }
    public void OnDrawGizmos()
    {
        if (updateNames)
        {
            updateNames = false;

            foreach (var panel in menuPanels)
            {
                panel.menuObjectl.name = panel.panalName;

                foreach (var item in panel.menuPanelItems)
                {
                    string name = "";

                    if (item.item.GetComponentInChildren<TextMeshProUGUI>())
                        name = item.item.GetComponentInChildren<TextMeshProUGUI>().text;
                    else if (item.item.GetComponentInChildren<Text>())
                        name = item.item.GetComponentInChildren<Text>().text;
                    else if(item.item.GetComponent<Text>())
                        name = item.item.GetComponent<Text>().text;


                    item.item.name = name;
                    item.name = name;
                }
            }
        }
    }

    public void OpenMenuPanel(string _menuPanelName, bool _exclusive)
    {
        ServicesProvider.instance.StartCoroutine(OpenMenuPanel_Coroutine(_menuPanelName, _exclusive));
    }
    public void CloseMenuPanel(string _menuPanelName)
    {
        ServicesProvider.instance.StartCoroutine(CloseMenuPanel_Coroutine(_menuPanelName));
    }
    public void ToggleMenuPanel(string _menuPanelName, bool _exculsive)
    {
        if (IsPanelActive(_menuPanelName))
        {
            CloseMenuPanel(_menuPanelName);
        }
        else
        {
            OpenMenuPanel(_menuPanelName, _exculsive);
        }
    }

    public static void OpenMenuPanel(string _menuPanelName_PlusMangerNum, List<PanelsManager> _managers, bool _exclusive)
    {
        string _menuName = PanelsManager.GetPanelName(_menuPanelName_PlusMangerNum);
        int _num = PanelsManager.GetManagerNumb(_menuPanelName_PlusMangerNum);

        _managers[_num].OpenMenuPanel(_menuName, _exclusive);
    }
    public static void TogglePanel(string _menuPanelName_PlusMangerNum, List<PanelsManager> _managers, bool _exclusive)
    {
        string _panelName = PanelsManager.GetPanelName(_menuPanelName_PlusMangerNum);
        int _num = PanelsManager.GetManagerNumb(_menuPanelName_PlusMangerNum);

        _managers[_num].ToggleMenuPanel(_panelName, _exclusive);
    }
    public static void CloseMenuPanel(string _menuPanelName_PlusMangerNum, List<PanelsManager> _managers)
    {
        string _menuName = PanelsManager.GetPanelName(_menuPanelName_PlusMangerNum);
        int _num = PanelsManager.GetManagerNumb(_menuPanelName_PlusMangerNum);

        _managers[_num].CloseMenuPanel(_menuName);
    }

    IEnumerator OpenMenuPanel_Coroutine(string _menuPanelName, bool exculsive)
    {
        //DeactivateFirst
        if(exculsive == false)
        {
            foreach (MenuPanel _menuPanel in menuPanels)
            {
                if (_menuPanel.IsActive() && (_menuPanel.panalName != _menuPanelName.ToString()))
                {
                    float _time = _menuPanel.ActivatePanel(false);
                    yield return new WaitForSeconds(_time);
                }
            }
        }

        //Then activate second
        foreach (MenuPanel _menuPanel in menuPanels)
        {
            if (_menuPanel.IsActive() == false && _menuPanel.panalName == _menuPanelName.ToString())
            {
                float _time = _menuPanel.ActivatePanel(true);
                yield return new WaitForSeconds(_time);
            }
        }

    }
    IEnumerator CloseMenuPanel_Coroutine(string _menuPanelName)
    {
        foreach (MenuPanel _menuPanel in menuPanels)
        {
            if (_menuPanel.IsActive() == true && _menuPanel.panalName == _menuPanelName.ToString())
            {
                float _time = _menuPanel.ActivatePanel(false);
                yield return new WaitForSeconds(_time);
            }
        }
    }
    bool IsPanelActive(string _panelName)
    {
        foreach (MenuPanel _menuPanel in menuPanels)
        {
            if (_menuPanel.IsActive() == true && _menuPanel.panalName == _panelName.ToString())
            {
                return true;
            }
        }

        return false;

    }
    static string GetPanelName(string _menuPanelName_PlusNum)
    {
        string _menuName = _menuPanelName_PlusNum.Substring(0, _menuPanelName_PlusNum.Length - 1);

        return _menuName;
    }
    static int GetManagerNumb(string _menuPanelName_PlusNum)
    {
        int _num = System.Int32.Parse(_menuPanelName_PlusNum[_menuPanelName_PlusNum.Length - 1].ToString());

        return _num;
    }
}


