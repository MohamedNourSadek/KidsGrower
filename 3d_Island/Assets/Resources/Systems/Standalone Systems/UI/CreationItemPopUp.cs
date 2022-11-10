using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CreationItemPopUp : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI requirement;
    [SerializeField] Button create;
    [SerializeField] Button close;

    CustomizePanel customizePanel;

    public void Initialize(CustomizePanel customize, string name, List<RequirementData> requirements)
    {
        this.customizePanel = customize;
        itemName.text = name;
        requirement.text = RequirementData.GetRequirementStatementText(PlayerSystem.instance.inventorySystem, requirements);
        create.interactable = RequirementData.IsRequirementTrue(PlayerSystem.instance.inventorySystem, requirements);

        create.Select();
    }

    private void Awake()
    {
        create.onClick.AddListener(OnCreatePress);
        close.onClick.AddListener(OnClosePressed);
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
        customizePanel.customizingState = CustomizingState.Placement;
        customizePanel.UpdateUi();
        UIGame.instance.ChangeCustomizingIndicator("Press on an empty area", Color.white);

        OnClosePressed();
    }
    void OnClosePressed()
    {
        Destroy(this.gameObject);
    }
}
