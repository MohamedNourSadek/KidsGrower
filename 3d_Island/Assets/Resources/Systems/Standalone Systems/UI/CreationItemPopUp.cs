using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class CreationItemPopUp : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI requirement;
    [SerializeField] Button create;
    [SerializeField] Button close;
    List<RequirementData> reqs;
    bool closeOnCreate = false;

    public void Initialize(UnityAction OnCreatePress, string name, List<RequirementData> requirements, bool closeOnCreate)
    {
        create.onClick.AddListener(OnCreatePress);
        create.onClick.AddListener(this.OnCreatePress);
        close.onClick.AddListener(OnClosePressed);

        this.closeOnCreate = closeOnCreate;
        itemName.text = name;
        reqs = requirements;
        RefreshRequirements();

        create.Select();
    }
    private void Update()
    {
        if (IsAnySelected() == false)
            OnClosePressed();
    }
    public bool IsAnySelected()
    {
        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (current == background.gameObject || current == create.gameObject || current == close.gameObject)
        {
            return true;
        }
        else
        {
            return false;

        }

    }


    void OnCreatePress()
    {
        RefreshRequirements();

        if (closeOnCreate)
            OnClosePressed();
    }
    void OnClosePressed()
    {
        Destroy(this.gameObject);
    }
    void RefreshRequirements()
    {
        requirement.text = RequirementData.GetRequirementStatementText(PlayerSystem.instance.inventorySystem, reqs);
        create.interactable = RequirementData.IsRequirementTrue(PlayerSystem.instance.inventorySystem, reqs);
    }

}
