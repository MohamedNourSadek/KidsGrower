using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSystem : MonoBehaviour, IController, IDetectable, IInputUser, ISavable
{
    public static PlayerSystem instance;

    [Header("References")]
    [SerializeField] GameObject dashVFXAsset;
    [SerializeField] GameObject jumpVFXAsset;

    //Editor Fields
    [Header("Editor")]
    [SerializeField] float nearObjectDistance = 1f;
    [SerializeField] Rigidbody playerBody;
    [SerializeField] Animator animatior;
    [SerializeField] MovementSystem movementSystem;
    [SerializeField] CameraSystem myCamera;
    [SerializeField] HandSystem handSystem;
    [SerializeField] LegSystem legSystem;
    [SerializeField] DetectorSystem detector;

    [Header("Animator Variables")]
    [SerializeField] Vector2 animationLerpSpeed = new Vector2(1f,1f);

    public InventorySystem inventorySystem = null;

    public bool activeInput { get; set; }

    Vector2 moveAnimtion;
    float maxHeight = 2f;


    //Initialization and refreshable functions
    void Awake()
    {
        InputSystem.SubscribeUser(this);

        if(inventorySystem == null)
            inventorySystem = new InventorySystem(this);

        movementSystem.Initialize(playerBody, myCamera.GetCameraTransform());
        myCamera.Initialize(this.gameObject);
        detector.Initialize(nearObjectDistance);
        handSystem.Initialize(detector, this);
        legSystem.Initialize(this);

        instance = this;
    }
    void FixedUpdate()
    {
        myCamera.Update();
        movementSystem.Update();
        detector.Update();
        handSystem.Update();

        UpdateUi();
    }
    void Update()
    {
        UpdateAnimationParameters();
    }
    void UpdateUi()
    {
        if (handSystem.canDrop)
            UIGame.instance.PickDropButton_SwitchMode(PickMode.Drop);
        else if (handSystem.canPick)
            UIGame.instance.PickDropButton_SwitchMode(PickMode.Pick);
        else if (handSystem.detector.GetDetectable("Tree").detectionStatus == DetectionStatus.VeryNear)
            UIGame.instance.PickDropButton_SwitchMode(PickMode.Shake);
        else
            UIGame.instance.PickDropButton_SwitchMode(PickMode.Pick);


        bool _canShake = (!handSystem.canPick
                       && !handSystem.canDrop
                       && (handSystem.detector.GetDetectable("Tree").detectionStatus == DetectionStatus.VeryNear));

        if (handSystem.canPick || handSystem.canDrop || _canShake)
            UIGame.instance.PickDropButton_Enable(true);
        else
            UIGame.instance.PickDropButton_Enable(false);

        if (handSystem.canThrow)
            UIGame.instance.ThrowButton_Enable(true);
        else
            UIGame.instance.ThrowButton_Enable(false);

        if (handSystem.canPlant)
            UIGame.instance.PlantButton_Enable(true);
        else
            UIGame.instance.PlantButton_Enable(false);

        UIGame.instance.JumpButton_Enable(movementSystem.IsOnGround());
        UIGame.instance.DashButton_Enable(movementSystem.IsDashable());
        UIGame.instance.PetButton_Enable(handSystem.canPet);
    }
    void UpdateAnimationParameters()
    {
        float finalMoveX = Mathf.Clamp01((playerBody.velocity.magnitude / movementSystem.maxSpeed));
        float finalMoveY = movementSystem.IsOnGround() ? 0f : ((movementSystem.groundDetector.DistanceFromGround(this.playerBody) / maxHeight));

        moveAnimtion.x = Mathf.Lerp(moveAnimtion.x, finalMoveX, Time.fixedDeltaTime * animationLerpSpeed.x);
        moveAnimtion.y = Mathf.Lerp(moveAnimtion.y, finalMoveY, Time.fixedDeltaTime * animationLerpSpeed.y);

        animatior.SetFloat("MoveX", moveAnimtion.x);
        animatior.SetFloat("MoveY", moveAnimtion.y);
    }


    //Interface 
    public void LoadData(SaveStructure saveData)
    {
        Player_Data player_data = (Player_Data)saveData;

        if (player_data.position.GetVector().magnitude != 0)
            transform.position = player_data.position.GetVector();

        transform.rotation = player_data.rotation.GetQuaternion();

        inventorySystem = new InventorySystem(this);

        DeployInventory(player_data.inventoryData);
    }
    public Player_Data GetData()
    {
        Player_Data player_data = new Player_Data();

        player_data.position = new nVector3(transform.position);
        player_data.rotation = new nQuaternion(transform.rotation);
        player_data.inventoryData = inventorySystem.GetItems_Data();

        return player_data;
    }
    public void DeployInventory(List<InventoryItem_Data> data)
    {
        foreach(var item in data)
        {
            if(item.itemTag == "Harvest")
            {
                for (int i = 0; i < item.amount; i++)
                {
                    var Obj = GameManager.instance.SpawnHarvest().GetComponent<IInventoryItem>();
                    inventorySystem.Add(Obj, false);
                }
            }
            else if(item.itemTag == "Xp Boost")
            {
                for (int i = 0; i < item.amount; i++)
                {
                    var Obj = GameManager.instance.SpawnXPGiver().GetComponent<IInventoryItem>();
                    inventorySystem.Add(Obj, false);
                }
            }
        }

    }


    //Hand controller Interface implementations
    public Rigidbody GetBody()
    {
        return playerBody;
    }
    public void LockPlayer(bool state)
    {
        this.enabled = state;
        activeInput = state;
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
        Instantiate(jumpVFXAsset, transform.position, jumpVFXAsset.transform.rotation);

    }
    public void PickInput()
    {
        if(handSystem.canPick)
        {
            if(InventorySystem.IsStorable(handSystem.GetNearest()))
            {
                inventorySystem.Add((handSystem.GetNearest()).GetComponent<IInventoryItem>(), true);
            }
            else
            {
                handSystem.PickObject();    

                NPCPick(true, handSystem.GetObjectInHand());
            }
        }
        else if(handSystem.canDrop)
        {
            NPCPick(false, handSystem.GetObjectInHand());

            handSystem.DropObject();
        }
        else if(handSystem.detector.GetDetectable("Tree").detectionStatus == DetectionStatus.VeryNear)
        {
            ((TreeSystem)(handSystem.detector.DetectableInRange("Tree"))).Shake();
        }
    }
    public void ThrowInput()
    {
        if (handSystem.canThrow)
        {
            NPCPick(false, handSystem.GetObjectInHand());

            handSystem.ThrowObject(this.transform.position + (this.transform.forward));
        }
    }
    public void PlantInput()
    {
        if (handSystem.canPlant)
            handSystem.PlantObject();
    }
    public void DashInput()
    {
        movementSystem.PerformDash();
        Instantiate(dashVFXAsset, transform.position, Quaternion.Euler(this.transform.rotation.eulerAngles));
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
    public bool GotNpcInHand()
    {
        if(handSystem.gotSomething && handSystem.GetObjectInHand().tag == "NPC")
        {
            return true;  
        }
        else
        {
            return false;
        } 
    }
    public NPC getNPCInHand()
    {
        if (handSystem.gotSomething && handSystem.GetObjectInHand().tag == "NPC")
        {
            return handSystem.GetObjectInHand().GetComponentInParent<NPC>();
        }
        else
        {
            return null;
        }
    }
    public void NPCPick(bool pickNotDrop, Pickable obj)
    {
        if (obj.tag == "NPC")
        {
            NPC myNpc = obj.GetComponent<NPC>();

            if (pickNotDrop == true)
            {
                var data = UIGame.instance.GetNPCStatsUI();

                data.name.text = myNpc.GetName();
                data.xp.text = myNpc.GetXp().ToString() + " Xp";
                data.level.text = "Level " + myNpc.GetLevel().ToString();

                UIGame.instance.OpenMenuPanel("NPC Stats2");
            }
            else
            {
                UIGame.instance.OpenMenuPanel("Empty2");
            }
        }
    }

}
