using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerSystem : MonoBehaviour, IController, IDetectable, IInputUser, ISavable
{

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
    [SerializeField] public HandSystem handSystem;
    [SerializeField] LegSystem legSystem;
    [SerializeField] DetectorSystem detector;
    [SerializeField] FacialControl myFace;
    [SerializeField] public InventorySystem inventorySystem = new InventorySystem();
    [SerializeField] AbilitySystem abilitySystem;

    [Header("Animator Variables")]
    [SerializeField] Vector2 animationLerpSpeed = new Vector2(1f,1f);

    public bool activeInput { get; set; }
    public static PlayerSystem instance;
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
        handSystem.Initialize(detector, this, abilitySystem);
        legSystem.Initialize(this);
        myFace.Initialize();
        abilitySystem.Initialize(detector, handSystem, movementSystem);

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
        abilitySystem.Update(); 
    }
    void UpdateUi()
    {
        //Set Interact Modes
        if(abilitySystem.canDress)
        {
            UIGame.instance.PickDropButton_SwitchMode(ButtonMode.Dress);
            UIGame.instance.PickDropButton_Enable(true);
        }
        else if (abilitySystem.canShake)
        {
            UIGame.instance.PickDropButton_SwitchMode(ButtonMode.Shake);
            UIGame.instance.PickDropButton_Enable(true);
        }
        else if (abilitySystem.canPick)
        {
            UIGame.instance.PickDropButton_SwitchMode(ButtonMode.Pick);
            UIGame.instance.PickDropButton_Enable(true);
        }
        else if (handSystem.GetObjectInHand() != null)
        {
            UIGame.instance.PickDropButton_SwitchMode(ButtonMode.Drop);
            UIGame.instance.PickDropButton_Enable(true);
        }
        else
        {
            UIGame.instance.PickDropButton_SwitchMode(ButtonMode._);
            UIGame.instance.PickDropButton_Enable(false);
        }

        if (abilitySystem.canTear)
        {
            UIGame.instance.AttackTearButton_SwitchMode(ButtonMode.Tear);
            UIGame.instance.AttackTearButton_Enable(true);
        }
        else
        {
            UIGame.instance.AttackTearButton_SwitchMode(ButtonMode._);
            UIGame.instance.AttackTearButton_Enable(false);
        }

        if (abilitySystem.canStore)
        {
            UIGame.instance.StoreButton_Enable(true);
        }
        else
        {
            UIGame.instance.StoreButton_Enable(false);
        }

        if (abilitySystem.canThrow)
            UIGame.instance.ThrowButton_Enable(true);
        else
            UIGame.instance.ThrowButton_Enable(false);

        if (abilitySystem.canPlant)
            UIGame.instance.PlantButton_Enable(true);
        else
            UIGame.instance.PlantButton_Enable(false);

        UIGame.instance.JumpButton_Enable(abilitySystem.canJump);
        UIGame.instance.DashButton_Enable(abilitySystem.canDash);
        UIGame.instance.PetButton_Enable(abilitySystem.canPet);
    }
    void UpdateAnimationParameters()
    {
        float finalMoveX = movementSystem.GetSpeedRatio();
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

        inventorySystem.LoadSavedData(player_data.inventoryData);
    }
    public Player_Data GetData()
    {
        Player_Data player_data = new Player_Data();

        player_data.position = new nVector3(transform.position);
        player_data.rotation = new nQuaternion(transform.rotation);
        player_data.inventoryData = inventorySystem.GetInventoryData();

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
        if (handSystem.GetObjectInHand() != null && handSystem.GetObjectInHand().tag == "NPC")
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
        if (handSystem.GetObjectInHand() != null && handSystem.GetObjectInHand().tag == "NPC")
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


    //(Movement-Input-Hand) Interface
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
        if (abilitySystem.canJump)
        {
            movementSystem.PreformJump();
            Instantiate(jumpVFXAsset, transform.position, jumpVFXAsset.transform.rotation);
        }
    }
    public void PickInput()
    {
        if(abilitySystem.canDress)
        {
            handSystem.ConfirmDress();
        }
        else if(abilitySystem.canShake)
        {
            ((TreeSystem)(detector.GetNear("Tree"))).Shake();
        }
        else if (abilitySystem.canPick)
        {
            handSystem.PickNearestObject();
            NPCPick(true, handSystem.GetObjectInHand());
        } 
        else if(handSystem.GetObjectInHand())
        {
            NPCPick(false, handSystem.GetObjectInHand());
            handSystem.DropObjectInHand();
        }
    }
    public void ThrowInput()
    {
        if (abilitySystem.canThrow)
        {
            NPCPick(false, handSystem.GetObjectInHand());

            handSystem.ThrowObjectInHand(this.transform.position + (this.transform.forward));
        }
    }
    public void PlantInput()
    {
        if (abilitySystem.canPlant)
            handSystem.PlantObjectInHand();
    }
    public void DashInput()
    {
        if (abilitySystem.canDash)
        {
            movementSystem.PerformDash();
            Instantiate(dashVFXAsset, transform.position, Quaternion.Euler(this.transform.rotation.eulerAngles));
        }
    }
    public void PetInput()
    {
        if(abilitySystem.canPet)
            handSystem.PetNearestObject();
    }
    public void AttackInput()
    {
        if (abilitySystem.canTear)
        {
            if (((Tearable)(detector.GetNear("Tree"))))
                ((Tearable)(detector.GetNear("Tree"))).TearDown();
            else
                ((Tearable)(detector.GetNear("Rock"))).TearDown();
        }
    }
    public void StoreInput()
    {
        if(abilitySystem.canStore)
            inventorySystem.Store(handSystem.GetNearestPickable().gameObject, true);
    }
    public void PressDownInput()
    {
    }
    public void PressUpInput()
    {
    }
}
