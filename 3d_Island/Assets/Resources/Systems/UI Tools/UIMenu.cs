using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

public enum MenuPanelNames { Main, Setting, GameModes }

public class UIMenu : MonoBehaviour
{
    [SerializeField] PanelsManager panelsManager;

    [HeaderAttribute("Movable Background")]
    [SerializeField] float speed = 1f;
    [SerializeField] List<DynamicBackgroundInfo> animationkeys;
    [SerializeField] LeanTweenType positionAnimationStyle;
    [SerializeField] LeanTweenType colorAnimationStyle;
    [SerializeField] Image movableBackground;
    [SerializeField] Image frame;

    public void OpenScreen(int _menu)
    {
        panelsManager.OpenMenuPanel((MenuPanelNames)_menu);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void OpenGame()
    {
        SceneManager.LoadSceneAsync(1);
    }


    Vector2 screenCenterPoint;
    void Start()
    {
        screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        AnimateBackground();
        panelsManager.Initialize();
    }

    int currentBackgroundAnimationKey = 0;
    void AnimateBackground()
    {
        LeanTween.value(movableBackground.gameObject, movableBackground.color.a, animationkeys[currentBackgroundAnimationKey].alpha, 1f / speed).setOnUpdate((float val) => {

            Image r = movableBackground;
            Color c = r.color;
            c.a = val;
            r.color = c;
        }).
        setEase(colorAnimationStyle);

        LeanTween.value(frame.gameObject, frame.color, animationkeys[currentBackgroundAnimationKey].color, 1f / speed).setOnUpdate((Color val) => {

            Image r = frame;
            Color c = r.color;
            c = val;
            r.color = c;
        }).
        setEase(colorAnimationStyle);  

        int _id = LeanTween.
            move(movableBackground.gameObject, screenCenterPoint + animationkeys[currentBackgroundAnimationKey].position, 1f / speed).
            setEase(positionAnimationStyle).id;
        
        LTDescr process = LeanTween.descr(_id);
        process.setOnComplete(AnimateBackground);


        if (currentBackgroundAnimationKey == animationkeys.Count - 1)
            currentBackgroundAnimationKey = 0;
        else
            currentBackgroundAnimationKey++;
    }

    private void OnDrawGizmos()
    {
        panelsManager.OnDrawGizmos();
    }
}



[System.Serializable]
public class PanelsManager
{
    [SerializeField] bool updateNames;
    [SerializeField] MenuPanelNames defaultPanel;
    [SerializeField] public List<MenuPanel> menuPanels;

    MenuPanelNames currentMenuPanel;

    public void OnDrawGizmos()
    {
        if (updateNames)
        {
            updateNames = false;

            foreach (var panel in menuPanels)
            {
                foreach (var item in panel.menuPanelItems)
                {
                    string name = item.item.gameObject.GetComponentInChildren<TextMeshProUGUI>().text;

                    item.item.name = name;
                    item.name = name;
                }
            }
        }
    }
    public void Initialize()
    {
        //reset all first
        foreach (MenuPanel _panel in menuPanels)
        {
            _panel.menuObjectl.SetActive(false);

            foreach(var _item in _panel.menuPanelItems)
            {
                _item.item.gameObject.SetActive(false);
            }
        }

        
        foreach(MenuPanel _panel in menuPanels)
        {
            foreach(var _item in _panel.menuPanelItems)
            {
                _item.Initialize();
            }
        }

        currentMenuPanel = defaultPanel;
        OpenMenuPanel(currentMenuPanel);
    }

    public void OpenMenuPanel(MenuPanelNames _menuPanelName)
    {

        ServicesProvider.instance.StartCoroutine(OpenMenuPanel_Coroutine(_menuPanelName));

    }

    IEnumerator OpenMenuPanel_Coroutine(MenuPanelNames _menuPanelName)
    {
        //DeactivateFirst
        foreach (MenuPanel _menuPanel in menuPanels)
        {
            if (_menuPanel.IsActive() && (_menuPanel.panalName != _menuPanelName))
            {
                float _time = _menuPanel.ActivatePanel(false);
                yield return new WaitForSeconds(_time);
            }
        }

        //Then activate second
        foreach (MenuPanel _menuPanel in menuPanels)
        {
            if (_menuPanel.IsActive() == false && _menuPanel.panalName == _menuPanelName)
            {
                float _time = _menuPanel.ActivatePanel(true);
                yield return new WaitForSeconds(_time);
            }
        }

    }
}

