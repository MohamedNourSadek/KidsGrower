using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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


    [SerializeField] FacialControl myFace;

    [SerializeField] public InventorySystem inventorySystem = new InventorySystem();

    public bool activeInput { get; set; }

    Vector2 moveAnimtion;
    float maxHeight = 2f;


    //Initialization and refreshable functions
    void Awake()
    {
        InputSystem.SubscribeUser(this);

        inventorySystem.Initialize(this);

        movementSystem.Initialize(playerBody, myCamera.GetCameraTransform());
        myCamera.Initialize(this.gameObject);
        detector.Initialize(nearObjectDistance, OnDetectableInRange, OnDetectableExit, OnDetectableNear, OnDetectableNearExit);
        handSystem.Initialize(detector, this);
        legSystem.Initialize(this);
        myFace.Initialize();

        instance = this;
    }
    void FixedUpdate()
    {
        myCamera.Update();
        movementSystem.Update();
        handSystem.Update();

        UpdateUi();


    }
    void Update()
    {
        UpdateAnimationParameters();
    }
    void UpdateUi()
    {
        //Set Interact Modes
        if (handSystem.canStore)
        {
            UIGame.instance.PickDropButton_SwitchMode(PickMode.Store);
            UIGame.instance.PickDropButton_Enable(true);
        }
        else if (handSystem.canShake)
        {
            UIGame.instance.PickDropButton_SwitchMode(PickMode.Shake);
            UIGame.instance.PickDropButton_Enable(true);
        }
        else if (handSystem.canPick)
        {
            UIGame.instance.PickDropButton_SwitchMode(PickMode.Pick);
            UIGame.instance.PickDropButton_Enable(true);
        }
        else if (handSystem.gotSomethingInHand)
        {
            UIGame.instance.PickDropButton_SwitchMode(PickMode.Drop);
            UIGame.instance.PickDropButton_Enable(true);
        }
        else
        {
            UIGame.instance.PickDropButton_SwitchMode(PickMode._);
            UIGame.instance.PickDropButton_Enable(false);
        }


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
        float finalMoveY = movementSystem.IsOnGround() ? 0f : ((movementSystem.groundDetector.GetDistanceFromGround() / maxHeight));

        moveAnimtion.x = Mathf.Lerp(moveAnimtion.x, finalMoveX, Time.fixedDeltaTime * animationLerpSpeed.x);
        moveAnimtion.y = Mathf.Lerp(moveAnimtion.y, finalMoveY, Time.fixedDeltaTime * animationLerpSpeed.y);

        animatior.SetFloat("MoveX", moveAnimtion.x);
        animatior.SetFloat("MoveY", moveAnimtion.y);
    }


    //Detector events
    void OnDetectableInRange(IDetectable detectable)
    {
    }
    void OnDetectableExit(IDetectable detectable)
    {
    }
    void OnDetectableNear(IDetectable detectable)
    {
        handSystem.AddToPickable(detectable);
    }
    void OnDetectableNearExit(IDetectable detectable)
    {
        handSystem.RemoveFromPickables(detectable);
    }


    //Interface 
    public void LoadData(SaveStructure saveData)
    {
        Player_Data player_data = (Player_Data)saveData;

        if (player_data.position.GetVector().magnitude != 0)
            transform.position = player_data.position.GetVector();

        transform.rotation = player_data.rotation.GetQuaternion();

        inventorySystem.Initialize(this);

        inventorySystem.LoadInventory(player_data.inventoryData);
    }
    public Player_Data GetData()
    {
        Player_Data player_data = new Player_Data();

        player_data.position = new nVector3(transform.position);
        player_data.rotation = new nQuaternion(transform.rotation);
        player_data.inventoryData = inventorySystem.GetItems_Data();

        return player_data;
    }
    public Rigidbody GetBody()
    {
        return playerBody;
    }
    public void LockPlayer(bool state)
    {
        this.enabled = state;
        activeInput = state;
    }
    public bool GotNpcInHand()
    {
        if (handSystem.gotSomethingInHand && handSystem.GetObjectInHand().tag == "NPC")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public NPC GetNPCInHand()
    {
        if (handSystem.gotSomethingInHand && handSystem.GetObjectInHand().tag == "NPC")
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
        if (obj != null && obj.tag == "NPC")
        {
            NPC myNpc = obj.GetComponent<NPC>();

            if (pickNotDrop == true)
            {
                var data = UIGame.instance.GetNPCStatsUI();

                data.name.text = myNpc.character.saveName;
                data.xp.text = myNpc.character.levelControl.GetXp().ToString() + " Xp";
                data.level.text = "Level " + myNpc.character.levelControl.GetLevel().ToString();

                UIGame.instance.OpenMenuPanel("NPC Stats1");
            }
            else
            {
                UIGame.instance.OpenMenuPanel("Empty1");
            }
        }
    }
    public void UpdateFullStats()
    {
        NPC npc = ((NPC)handSystem.GetObjectInHand());

        if (npc != null)
            UIGame.instance.UpdateFullNPCStats(npc.character);
    }
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
    public GroundDetector GetGroundDetector()
    {
        return movementSystem.groundDetector;
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
        if(handSystem.canStore)
        {
            inventorySystem.Add(handSystem.GetNearestPickable().gameObject, true);
        }
        else if(handSystem.canShake)
        {
            ((TreeSystem)(detector.GetNear("Tree"))).Shake();
        }
        else if (handSystem.canPick)
        {
            handSystem.PickNearestObject();
            NPCPick(true, handSystem.GetObjectInHand());
        }
        else if(handSystem.gotSomethingInHand)
        {
            NPCPick(false, handSystem.GetObjectInHand());
            handSystem.DropObjectInHand();
        }
    }
    public void ThrowInput()
    {
        if (handSystem.canThrow)
        {
            NPCPick(false, handSystem.GetObjectInHand());

            handSystem.ThrowObjectInHand(this.transform.position + (this.transform.forward));
        }
    }
    public void PlantInput()
    {
        if (handSystem.canPlant)
            handSystem.PlantObjectInHand();
    }
    public void DashInput()
    {
        movementSystem.PerformDash();
        Instantiate(dashVFXAsset, transform.position, Quaternion.Euler(this.transform.rotation.eulerAngles));
    }
    public void PetInput()
    {
        if(handSystem.canPet)
            handSystem.PetNearestObject();
    }
    public void PressInput() 
    {
    }
}
