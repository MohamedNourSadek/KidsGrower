using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSystem : MonoBehaviour, IController, IDetectable, IInputUser
{
    //Editor Fields
    [SerializeField] float nearObjectDistance = 1f;
    [SerializeField] Rigidbody playerBody;
    [SerializeField] MovementSystem movementSystem;
    [SerializeField] CameraSystem myCamera;
    [SerializeField] HandSystem handSystem;
    [SerializeField] DetectorSystem detector;
    
    InventorySystem inventorySystem;
   
    //Initialization and refreshable functions
    void Awake()
    {
        InputSystem.SubscribeUser(this);
        inventorySystem = new InventorySystem(this);
        movementSystem.Initialize(playerBody, myCamera.GetCameraTransform());
        myCamera.Initialize(this.gameObject);
        detector.Initialize(nearObjectDistance);
        handSystem.Initialize(detector, this);
    }
    void FixedUpdate()
    {
        myCamera.Update();
        movementSystem.Update();
        detector.Update();
        handSystem.Update();
        
        UpdateUi();
    }
    void UpdateUi()
    {
        if (handSystem.canDrop)
            UIController.instance.PickDropButton_SwitchMode(PickMode.Drop);
        else if (handSystem.canPick)
            UIController.instance.PickDropButton_SwitchMode(PickMode.Pick);
        else if (handSystem.detector.GetDetectable("Tree").detectionStatus == DetectionStatus.VeryNear)
            UIController.instance.PickDropButton_SwitchMode(PickMode.Shake);
        else
            UIController.instance.PickDropButton_SwitchMode(PickMode.Pick);


        bool _canShake = (!handSystem.canPick
                       && !handSystem.canDrop
                       && (handSystem.detector.GetDetectable("Tree").detectionStatus == DetectionStatus.VeryNear));

        if (handSystem.canPick || handSystem.canDrop || _canShake)
            UIController.instance.PickDropButton_Enable(true);
        else
            UIController.instance.PickDropButton_Enable(false);

        if (handSystem.canThrow)
            UIController.instance.ThrowButton_Enable(true);
        else
            UIController.instance.ThrowButton_Enable(false);

        if (handSystem.canPlant)
            UIController.instance.PlantButton_Enable(true);
        else
            UIController.instance.PlantButton_Enable(false);

        UIController.instance.JumpButton_Enable(movementSystem.IsOnGround());
        UIController.instance.DashButton_Enable(movementSystem.IsDashable());
        UIController.instance.PetButton_Enable(handSystem.canPet);
    }



    //Interface 
    public void LoadData(Player_Data player_data)
    {
        if (player_data.position.GetVector().magnitude != 0)
            transform.position = player_data.position.GetVector();

        transform.rotation = player_data.rotation.GetQuaternion();
    }
    public Player_Data GetData()
    {
        Player_Data player_data = new Player_Data();

        player_data.position = new nVector3(transform.position);
        player_data.rotation = new nQuaternion(transform.rotation);

        return player_data;
    }


    //Hand controller Interface implementations
    public Rigidbody GetBody()
    {
        return playerBody;
    }
    public void LockPlayer(bool state)
    {
        playerBody.isKinematic = !state;
    }



    ///(Movement-Input-Hand) Interface
    public void MoveInput(Vector2 _movementInput)
    {
        movementSystem.PreformMove(_movementInput);
    } 
    public void RotateInput(Vector2 _deltaRotation)
    {
        myCamera.RotateCamera(_deltaRotation);
    }
    public void JumpInput()
    {
        movementSystem.PreformJump();
    }
    public void PickInput()
    {
        if(handSystem.canPick)
        {
            if(InventorySystem.IsStorable(handSystem.GetNearest()))
            {
                inventorySystem.Add((handSystem.GetNearest()).GetComponent<IInventoryItem>());
            }
            else
            {
                handSystem.PickObject();
            }
        }
        else if(handSystem.canDrop)
        {
            handSystem.DropObject();
        }
        else if(handSystem.detector.GetDetectable("Tree").detectionStatus == DetectionStatus.VeryNear)
        {
            ((TreeSystem)(handSystem.detector.DetectableInRange("Tree"))).Shake();
        }
    }
    public void ThrowInput()
    {
        if(handSystem.canThrow)
            handSystem.ThrowObject(this.transform.position + (this.transform.forward));
    }
    public void PlantInput()
    {
        if (handSystem.canPlant)
            handSystem.PlantObject();
    }
    public void DashInput()
    {

        movementSystem.PerformDash();
    }
    public void PetInput()
    {
        if(handSystem.canPet)
            handSystem.PetObject();
    }
    public void PressInput() { }
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}
