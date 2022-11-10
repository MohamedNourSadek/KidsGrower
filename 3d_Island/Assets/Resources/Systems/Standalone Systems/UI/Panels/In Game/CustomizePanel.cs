using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public enum CustomizingState { Detecting, Selected, Placement, Moving }

public class CustomizePanel : MenuPanel, IInputUser
{
    [SerializeField] LayerMask customizeDetectable;
    [SerializeField] GameObject modificationMenu;
    [SerializeField] GameObject creationItemPopUp;
    
    //Ui References
    [SerializeField] List<CButton> creationItems;
    [SerializeField] GameObject creationMenu;
    [SerializeField] CButton positionButton;
    [SerializeField] CButton rotationButton;
    [SerializeField] CButton confirmButton;
    [SerializeField] CButton closeButton;

    public bool activeInput { get; set; }
    float modificationRelativePosition = 120;
    bool customizing = false;
    public CustomizingState customizingState = CustomizingState.Detecting;
    Quaternion originalRotation;
    Vector3 originalPosition;
    bool holdPress;

    string currentItemTag = "";
    List<RequirementData> currentBuildingRequirements = new List<RequirementData>();
    CustomizableObject selectedObject;

    public override void Initialize()
    {
        base.Initialize();
        InputSystem.SubscribeUser(this);
        positionButton.onClick.AddListener(OnMovePress);    
        rotationButton.onClick.AddListener(OnRotatePress);
        confirmButton.onClick.AddListener(OnConfirmPress);
        closeButton.onClick.AddListener(OnClosePress);
        foreach (CButton button in creationItems)
            button.onClick.AddListener(OnCreationPress);
    }
    public override void FillFunctions()
    {
        base.FillFunctions();

        GetButton("Back").onClick.AddListener(new UnityAction(() => UIGame.instance.OpenMenuPanel("Main0")));
    }
    public override void OnActiveChange(bool state)
    {
        base.OnActiveChange(state);

        customizing = state;

        GameManager.instance.SetBlur(!customizing);
    }
    void OnEnable()
    {
        ResetSelection();
    }
    void OnDisable()
    {
        ResetSelection();
    }


    //Internal
    void ResetSelection()
    {
        if(selectedObject)
        {
            selectedObject.transform.position = originalPosition;
            selectedObject.transform.rotation = originalRotation;
        }

        DeSelect();
    }
    void DeSelect()
    {
        customizingState = CustomizingState.Detecting;

        if (selectedObject)
            selectedObject.SetSelectState(false);

        selectedObject = null;

        UpdateUi();
    }
    void Select(GameObject selected)
    {
        selectedObject = selected.GetComponentInParent<CustomizableObject>();
        selectedObject.SetSelectState(true);
        originalPosition = selectedObject.transform.position;
        originalRotation = selectedObject.transform.rotation;

        customizingState = CustomizingState.Selected;
    }
    RaycastHit CastFromMouse()
    {
        RaycastHit hit;

        Vector2 mouse2d = InputSystem.GetMousePosition();
        Vector3 mousePosition = new(mouse2d.x, mouse2d.y, 2f);

        Ray ray = Camera.main.ScreenPointToRay(mousePosition, Camera.MonoOrStereoscopicEye.Mono);

        Physics.Raycast(ray, out hit, customizeDetectable);

        return hit;
    }
    public void UpdateUi()
    {
        bool state = (customizingState == CustomizingState.Moving);

        positionButton.interactable = !state;
        rotationButton.gameObject.SetActive(!state);
        confirmButton.gameObject.SetActive(!state);
        closeButton.gameObject.SetActive(!state);
        modificationRelativePosition = (state) ? 0 : 120;

        if (selectedObject != null)
        {
            modificationMenu.SetActive(true);
            creationMenu.SetActive(false);
            Vector2 objectPositionInCam = Camera.main.WorldToScreenPoint(selectedObject.gameObject.transform.position);
            modificationMenu.transform.position = objectPositionInCam + (Vector2.up*modificationRelativePosition);
        }
        else
        {
            modificationMenu.SetActive(false);

            if(customizingState != CustomizingState.Placement)
                creationMenu.SetActive(true);
            else
                creationMenu.SetActive(false);
        }
    }
    void TrySelect()
    {
        RaycastHit hit = CastFromMouse();

        if (hit.collider.GetComponentInParent<CustomizableObject>())
        {
            DeSelect();
            Select(hit.collider.gameObject);
            UpdateUi();
            UIGame.instance.ChangeCustomizingIndicator("", Color.white);
        }
        else
        {
            DeSelect();
            UIGame.instance.ChangeCustomizingIndicator("", Color.white);
        }
    }
    void TryPlace()
    {
        RaycastHit hit = CastFromMouse();

        if (hit.collider.gameObject.CompareTag("Ground"))
        {
            GameObject obj = null;

            if (currentItemTag == "Magic House")
                obj = GameManager.instance.SpawnNamingHouse(hit.point);

            Select(obj);
            PlayerSystem.instance.inventorySystem.ConsumeResources(currentBuildingRequirements);

            customizingState = CustomizingState.Selected;
            UpdateUi();

            UIGame.instance.ChangeCustomizingIndicator("", Color.white);
        }
        else
        {
            UIGame.instance.ChangeCustomizingIndicator("Press on a valid area", Color.yellow);
        }
    }
    void TryMove()
    {
        StartCoroutine(Moving());
    }
    IEnumerator Moving()
    {
        while(holdPress)
        {
            RaycastHit hit = CastFromMouse();

            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                selectedObject.transform.position = hit.point;
                UpdateUi();
            }

            yield return new WaitForEndOfFrame();
        }

        customizingState = CustomizingState.Selected;
        UpdateUi();
    }
    void OnMovePress()
    {
        customizingState = CustomizingState.Moving;

        UpdateUi();
    }
    void OnRotatePress()
    {
        selectedObject.gameObject.transform.Rotate(0, 90, 0);
    }
    void OnConfirmPress()
    {
        DeSelect();
    }
    void OnClosePress()
    {
        ResetSelection();
    }
    void OnCreationPress()
    {
        currentItemTag = "Not found";
        CButton selectedButton = null;

        foreach (CButton button in creationItems)
            if (button.gameObject == EventSystem.current.currentSelectedGameObject)
            {
                selectedButton = button;
                currentItemTag = button.gameObject.name;
            }

        var Obj = Instantiate(creationItemPopUp, selectedButton.transform);
        Obj.transform.parent = UIGame.instance.gameCanvas.transform;

        currentBuildingRequirements = new List<RequirementData>()
        {
            new RequirementData() { itemTag = "WoodPack",  itemAmount = 2},
            new RequirementData() { itemTag = "StonePack",  itemAmount = 2},
        };

        Obj.GetComponent<CreationItemPopUp>().Initialize(this, currentItemTag, currentBuildingRequirements);
    }


    //Input Interface
    public void PressDownInput()
    {
        if (customizing)
        {
            if ((customizingState == CustomizingState.Detecting))
            {
                TrySelect();
            }
            else if (customizingState == CustomizingState.Moving)
            {
                holdPress = true;
                TryMove();
            }
            else if(customizingState == CustomizingState.Placement)
            {
                TryPlace();
            }
        }
    }
    public void PressUpInput()
    {
        holdPress = false;
    }
    public void PetInput() { }
    public void DashInput() { }
    public void PlantInput() { }
    public void ThrowInput() { }
    public void JumpInput() { }
    public void PickInput() { }
    public void MoveInput(Vector2 movementInput) { }
    public void RotateInput(Vector2 deltaRotate) { }
}
