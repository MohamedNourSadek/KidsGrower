using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public enum CustomizingState { Detecting, Selected, Placement, Moving }

public class CustomizePanel : MenuPanel, IInputUser, ICreator
{
    [SerializeField] LayerMask customizeDetectable;
    [SerializeField] GameObject modificationMenu;

    //Ui References
    [SerializeField] CreationSystem creationSystem;
    [SerializeField] GameObject creationMenu;
    [SerializeField] CButton positionButton;
    [SerializeField] CButton rotationButton;
    [SerializeField] CButton confirmButton;
    [SerializeField] CButton closeButton;


    public CustomizingState customizingState = CustomizingState.Detecting;
    public bool activeInput { get; set; }
    CustomizableObject selectedObject;
    float modificationRelativePosition = 120;
    bool customizing = false;
    Quaternion originalRotation;
    Vector3 originalPosition;
    bool holdPress;

    //Menu Panel Interface
    public override void Initialize()
    {
        base.Initialize();
        creationSystem.Initialize(this);
        InputSystem.SubscribeUser(this);

        positionButton.onClick.AddListener(OnMovePress);    
        rotationButton.onClick.AddListener(OnRotatePress);
        confirmButton.onClick.AddListener(OnConfirmPress);
        closeButton.onClick.AddListener(OnClosePress);
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

    //Creation System related
    void TryPlace()
    {
        RaycastHit hit = CastFromMouse();

        if (hit.collider.gameObject.CompareTag("Ground"))
        {
            GameObject obj = creationSystem.SpawnItem(hit.point);

            Select(obj);
            customizingState = CustomizingState.Selected;

            UpdateUi();
            UIGame.instance.ChangeCustomizingIndicator("", Color.white);
        }
        else
        {
            UIGame.instance.ChangeCustomizingIndicator("Press on a valid area", Color.yellow);
        }
    }
    public void OnCreatePress()
    {
        customizingState = CustomizingState.Placement;
        UpdateUi();
        UIGame.instance.ChangeCustomizingIndicator("Press on an empty area", Color.white);
    }


    //Customization Functions
    void OnEnable()
    {
        ResetSelection();
    }
    void OnDisable()
    {
        ResetSelection();
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
            modificationMenu.transform.position = objectPositionInCam + (Vector2.up * modificationRelativePosition);
        }
        else
        {
            modificationMenu.SetActive(false);

            if (customizingState != CustomizingState.Placement)
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
    void TryMove()
    {
        StartCoroutine(Moving());
    }
    IEnumerator Moving()
    {
        while (holdPress)
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


    //Customization Events
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


    //Internal
    void ResetSelection()
    {
        if (selectedObject)
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
