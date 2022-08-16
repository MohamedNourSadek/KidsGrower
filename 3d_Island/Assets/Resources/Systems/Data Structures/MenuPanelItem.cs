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
    bool active;

    public void Initialize()
    {
        if(item.GetComponentInChildren<Button>())
            (item.GetComponentInChildren<Button>()).onClick.AddListener(InvokeEvent);
    }
    public void ActivateItem(bool _state, MenuAnimatioSettings _animationSettings)
    {
        ServicesProvider.instance.StartCoroutine(ActivateItem_CoRoutine(_state, _animationSettings));
    }
    public bool IsActive()
    {
        return active;
    }

    IEnumerator ActivateItem_CoRoutine(bool _state, MenuAnimatioSettings _animationSettings)
    {
        if (_state)
        {
            item.gameObject.SetActive(_state);

            item.gameObject.LeanScale(_animationSettings.offScale, 0f);
            item.gameObject.LeanScale(_animationSettings.onScale, _animationSettings.InAnimationTime).setEase(_animationSettings.InAnimationCurve);

            yield return new WaitForSeconds(_animationSettings.InAnimationTime);
        }
        else
        {
            item.gameObject.LeanScale(_animationSettings.onScale, 0f);
            item.transform.LeanScale(_animationSettings.offScale, _animationSettings.OutAnimationTime).setEase(_animationSettings.OutAnimationCurve);

            yield return new WaitForSeconds(_animationSettings.OutAnimationTime);

            item.gameObject.SetActive(_state);
        }

        active = _state;
    }
    void InvokeEvent()
    {
        Debug.Log("Pressed");
        OnPress.Invoke();
    }
}

