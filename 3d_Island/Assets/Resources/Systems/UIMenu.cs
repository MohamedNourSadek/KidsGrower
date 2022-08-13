using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;


public class UIMenu : MonoBehaviour
{
    [SerializeField] PanelsManager _panelsManager;

    [HeaderAttribute("Movable Background")]
    [SerializeField] float _speed = 1f;
    [SerializeField] List<DynamicBackgroundInfo> _animationkeys;
    [SerializeField] LeanTweenType _positionAnimationStyle;
    [SerializeField] LeanTweenType _colorAnimationStyle;
    [SerializeField] Image _movableBackground;
    [SerializeField] Image _frame;

    public void OpenScreen(int menu)
    {
        _panelsManager.OpenMenuPanel((MenuPanelNames)menu);
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
        _panelsManager.Initialize();
    }

    int currentBackgroundAnimationKey = 0;
    void AnimateBackground()
    {
        LeanTween.value(_movableBackground.gameObject, _movableBackground.color.a, _animationkeys[currentBackgroundAnimationKey].alpha, 1f / _speed).setOnUpdate((float val) => {

            Image r = _movableBackground;
            Color c = r.color;
            c.a = val;
            r.color = c;
        }).
        setEase(_colorAnimationStyle);

        LeanTween.value(_frame.gameObject, _frame.color, _animationkeys[currentBackgroundAnimationKey].color, 1f / _speed).setOnUpdate((Color val) => {

            Image r = _frame;
            Color c = r.color;
            c = val;
            r.color = c;
        }).
        setEase(_colorAnimationStyle);  

        int id = LeanTween.
            move(_movableBackground.gameObject, screenCenterPoint + _animationkeys[currentBackgroundAnimationKey].position, 1f / _speed).
            setEase(_positionAnimationStyle).id;
        
        LTDescr process = LeanTween.descr(id);
        process.setOnComplete(AnimateBackground);


        if (currentBackgroundAnimationKey == _animationkeys.Count - 1)
            currentBackgroundAnimationKey = 0;
        else
            currentBackgroundAnimationKey++;
    }

    private void OnDrawGizmos()
    {
        _panelsManager.OnDrawGizmos();
    }
}


public enum MenuPanelNames { Main, Setting, GameModes}

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
                foreach (var item in panel._menuPanelItems)
                {
                    string name = item._item.gameObject.GetComponentInChildren<TextMeshProUGUI>().text;

                    item._item.name = name;
                    item._name = name;
                }
            }
        }
    }
    public void Initialize()
    {
        //reset all first
        foreach (MenuPanel panel in menuPanels)
        {
            panel._menuObjectl.SetActive(false);

            foreach(var item in panel._menuPanelItems)
            {
                item._item.gameObject.SetActive(false);
            }
        }

        
        foreach(MenuPanel panel in menuPanels)
        {
            foreach(var item in panel._menuPanelItems)
            {
                item.Initialize();
            }
        }

        currentMenuPanel = defaultPanel;
        OpenMenuPanel(currentMenuPanel);
    }

    public void OpenMenuPanel(MenuPanelNames menuPanelName)
    {

        CoRoutineProvider.instance.StartCoroutine(OpenMenuPanel_Coroutine(menuPanelName));

    }

    IEnumerator OpenMenuPanel_Coroutine(MenuPanelNames menuPanelName)
    {
        //DeactivateFirst
        foreach (MenuPanel menuPanel in menuPanels)
        {
            if (menuPanel.IsActive() && (menuPanel._panalName != menuPanelName))
            {
                float _time = menuPanel.ActivatePanel(false);
                yield return new WaitForSeconds(_time);
            }
        }

        //Then activate second
        foreach (MenuPanel menuPanel in menuPanels)
        {
            if (menuPanel.IsActive() == false && menuPanel._panalName == menuPanelName)
            {
                float _time = menuPanel.ActivatePanel(true);
                yield return new WaitForSeconds(_time);
            }
        }

    }
}


[System.Serializable]
public class MenuPanel
{
    [SerializeField] public MenuPanelNames _panalName;
    [SerializeField] public GameObject _menuObjectl;
    [SerializeField] public List<MenuPanelButton> _menuPanelItems;
    [SerializeField] MenuAnimatioSettings _animationSettings;

    bool _active;

    public bool IsActive()
    {
        return _active;
    }

    public float ActivatePanel(bool _state)
    {
        _active = _state;

        float _time = 0;

        if (_state)
            _time = _animationSettings._InAnimationTime;
        else
            _time = _animationSettings._OutAnimationTime;

        CoRoutineProvider.instance.StartCoroutine(ActivatePanel_Coroutine(_state, _time));

        return _time;
    }
    IEnumerator ActivatePanel_Coroutine(bool _state, float _time)
    {
        if (_state)
            _menuObjectl.SetActive(_state);

        foreach (MenuPanelButton item in _menuPanelItems)
            item.ActivateItem(_state, _animationSettings);
        
        if (!_state)
        {
            yield return new WaitForSeconds(_time);
            _menuObjectl.SetActive(_state);
        }

    }
}


[System.Serializable]
public class MenuPanelButton
{
    [SerializeField] public string _name;
    [SerializeField] public Button _item;
    [SerializeField] public UnityEvent _OnPress;
    bool _active;

    public void Initialize()
    {
        _item.onClick.AddListener(InvokeEvent);
    }
    void InvokeEvent()
    {
        _OnPress.Invoke();
    }
    public void ActivateItem(bool _state, MenuAnimatioSettings _animationSettings)
    {
        CoRoutineProvider.instance.StartCoroutine(ActivateItem_CoRoutine(_state, _animationSettings));
    }
    IEnumerator ActivateItem_CoRoutine(bool _state, MenuAnimatioSettings _animationSettings)
    {
        if(_state)
        {
            _item.gameObject.SetActive(_state);

            _item.gameObject.LeanScale(_animationSettings._offScale, 0f);
            _item.gameObject.LeanScale(_animationSettings._onScale, _animationSettings._InAnimationTime).setEase(_animationSettings._InAnimationCurve);

            yield return new WaitForSeconds(_animationSettings._InAnimationTime);
        }
        else
        {
            _item.gameObject.LeanScale(_animationSettings._onScale, 0f);
            _item.transform.LeanScale(_animationSettings._offScale, _animationSettings._OutAnimationTime).setEase(_animationSettings._OutAnimationCurve);

            yield return new WaitForSeconds(_animationSettings._OutAnimationTime);

            _item.gameObject.SetActive(_state);
        }

        _active = _state;
    }

    public bool IsActive()
    {
        return _active;
    }
}

[System.Serializable]
public class MenuAnimatioSettings
{
    [SerializeField] public Vector3 _offScale = Vector3.zero;
    [SerializeField] public Vector3 _onScale = new Vector3(1, 1, 1);
    [SerializeField] public float _InAnimationTime = 0.3f;
    [SerializeField] public float _OutAnimationTime = 0.3f;
    [SerializeField] public LeanTweenType _InAnimationCurve;
    [SerializeField] public LeanTweenType _OutAnimationCurve;

}

[System.Serializable]
public class DynamicBackgroundInfo
{
    public Vector2 position;

    public float alpha;
    public Color32 color;
}

