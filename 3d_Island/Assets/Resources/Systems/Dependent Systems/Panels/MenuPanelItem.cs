using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



[System.Serializable]
public class MenuPanelItem
{
    [SerializeField] public string name;
    [SerializeField] public GameObject item;
    [SerializeField] public UnityEvent OnPress;
    [SerializeField] public List<modes> HiddenModes = new List<modes>();

    bool active;
    MenuAnimatioSettings animationSettings;

    public void Initialize(MenuAnimatioSettings _animationSettings)
    {
        if(item.GetComponentInChildren<CButton>())
        {
            (item.GetComponentInChildren<CButton>()).onClick.AddListener(OnPress.Invoke);
            (item.GetComponentInChildren<CButton>()).animationSettings = _animationSettings;
        }

        animationSettings = _animationSettings;
    }
    public void ActivateItem(bool _state)
    {
        ServicesProvider.instance.StartCoroutine(ActivateItem_CoRoutine(_state));
    }
    public bool IsActive()
    {
        return active;
    }

    IEnumerator ActivateItem_CoRoutine(bool _state)
    {
        bool supressed = false;

        foreach(modes mode in HiddenModes)
            if(DataManager.instance.GetCurrentMode() == mode)
                supressed = true;

        if (supressed == false)
        {
            if (_state)
            {
                item.gameObject.LeanScale(animationSettings.offScale, 0f);
                item.gameObject.LeanScale(animationSettings.onScale, animationSettings.InAnimationTime).setEase(animationSettings.InAnimationCurve);

                yield return new WaitForSeconds(animationSettings.InAnimationTime);
            }
            else
            {
                item.gameObject.LeanScale(animationSettings.onScale, 0f);
                item.transform.LeanScale(animationSettings.offScale, animationSettings.OutAnimationTime).setEase(animationSettings.OutAnimationCurve);

                yield return new WaitForSeconds(animationSettings.OutAnimationTime);
            }

            active = _state;
        }
        else
        {
            item.SetActive(false);
        }
    }
}

