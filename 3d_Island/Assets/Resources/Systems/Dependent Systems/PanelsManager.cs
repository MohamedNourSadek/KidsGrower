using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PanelsManager
{
    [SerializeField] bool updateNames;
    [SerializeField] public List<MenuPanel> menuPanels;

    string currentMenuPanel;
    public void Initialize(string defaultEnum)
    {
        //reset all first
        foreach (MenuPanel _panel in menuPanels)
        {
            _panel.menuObjectl.SetActive(false);

            foreach (var _item in _panel.menuPanelItems)
            {
                _item.item.gameObject.SetActive(false);
            }
        }


        foreach (MenuPanel _panel in menuPanels)
        {
            foreach (var _item in _panel.menuPanelItems)
            {
                _item.Initialize();
            }
        }

        currentMenuPanel = defaultEnum;
        OpenMenuPanel(currentMenuPanel);
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
    public void OpenMenuPanel(string _menuPanelName)
    {
        ServicesProvider.instance.StartCoroutine(OpenMenuPanel_Coroutine(_menuPanelName));
    }
    IEnumerator OpenMenuPanel_Coroutine(string _menuPanelName)
    {
        //DeactivateFirst
        foreach (MenuPanel _menuPanel in menuPanels)
        {
            if (_menuPanel.IsActive() && (_menuPanel.panalName != _menuPanelName.ToString()))
            {
                float _time = _menuPanel.ActivatePanel(false);
                yield return new WaitForSeconds(_time);
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

}


