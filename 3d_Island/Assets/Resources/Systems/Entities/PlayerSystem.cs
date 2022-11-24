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
    [SerializeField] GameObject myModel;

    //Editor Fields
    [Header("Editor")]
    [SerializeField] public HealthControl healthControl;
    [SerializeField] public InventorySystem inventorySystem = new InventorySystem();
    [SerializeField] public HandSystem handSystem;
    [SerializeField] Rigidbody playerBody;
    [SerializeField] Animator animatior;
    [SerializeField] MovementSystem movementSystem;
    [SerializeField] CameraSystem myCamera;
    [SerializeField] LegSystem legSystem;
    [SerializeField] DetectorSystem detector;
    [SerializeField] FacialControl myFace;
    [SerializeField] AbilitySystem abilitySystem;
    [SerializeField] float nearObjectDistance = 1f;
    [SerializeField] float shakingTime = 1f;
    [SerializeField] Vector2 animationLerpSpeed = new Vector2(1f,1f);

    public static PlayerSystem instance;
    public bool activeInput { get; set; }
    Vector2 moveAnimtion;
    float maxHeight = 2f;
    bool shaking;
    bool attacking;


    //Initialization and refreshable functions
    void Awake()
    {
        InputSystem.SubscribeUser(this);

        healthControl.Initialize(this.gameObject);
        healthControl.OnDeath += Respawn;

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

        if(Input.GetKeyDown("x"))
        {
            healthControl.GetAttacked(15, this.transform);
        }
    }
    void UpdateUi()
    {
        healthControl.Update();

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

        if(abilitySystem.canAttack)
        {
            UIGame.instance.AttackTearButton_SwitchMode(ButtonMode.Attack);
            UIGame.instance.AttackTearButton_Enable(true);
        }
        else if (abilitySystem.canTear)
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
        {
            UIGame.instance.PlantEatButton_SwitchMode(ButtonMode.Plant);
            UIGame.instance.PlantButton_Enable(true);
        }
        else if (abilitySystem.canEat)
        {
            UIGame.instance.PlantEatButton_SwitchMode(ButtonMode.Eat);
            UIGame.instance.PlantButton_Enable(true);
        }
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
    void Respawn()
    {
        StartCoroutine(RespwawnCoroutine());
    }
    IEnumerator RespwawnCoroutine()
    {
        myModel.SetActive(false);
        activeInput = false;
        Instantiate(jumpVFXAsset, transform.position, jumpVFXAsset.transform.rotation);
        GetBody().isKinematic= true;

        inventorySystem.items.Clear();
        if(handSystem.GetObjectInHand())
        {
            if(handSystem.GetObjectInHand().tag == "NPC") 
            {
                handSystem.DropObjectInHand();
            }
            else
            {
                var obj = handSystem.GetObjectInHand();
                handSystem.DropObjectInHand();
                Destroy(obj.gameObject);   
            }
        }

        yield return new WaitForSeconds(2f);

        this.transform.position = MapSystem.instance.GetRandomExplorationPoint();
        GetBody().isKinematic = false;
        activeInput = true;
        Instantiate(jumpVFXAsset, transform.position, jumpVFXAsset.transform.rotation);
        myModel.SetActive(true);
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

        healthControl.currentHealth = player_data.currentHealth;

        inventorySystem.Initialize(this);

        inventorySystem.LoadSavedData(player_data.inventoryData);
    }
    public Player_Data GetData()
    {
        Player_Data player_data = new Player_Data();

        player_data.position = new nVector3(transform.position);
        player_data.rotation = new nQuaternion(transform.rotation);
        player_data.inventoryData = inventorySystem.GetInventoryData();
        player_data.currentHealth = healthControl.currentHealth;

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
        else if(abilitySystem.canShake && !shaking)
        {
            ((TreeSystem)(detector.GetNear("Tree"))).Shake();

            StartCoroutine(ReactivateShake(shakingTime));
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
        else if (abilitySystem.canEat)
        {
            if((Fruit)handSystem.GetObjectInHand())
            {
                int value = ((Fruit)handSystem.GetObjectInHand()).GetMore();

                if(healthControl.currentHealth < healthControl.maxHealth)
                {
                    healthControl.currentHealth += value;
                }
            }
        }
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
        if (!attacking)
        {
            if (abilitySystem.canAttack)
            {
                if (((Zombie)(detector.GetNear("Zombie"))))
                {
                    Sword sword = ((Sword)handSystem.GetObjectInHand());
                    Zombie zombie = (Zombie)(detector.GetNear("Zombie"));

                    zombie.healthControl.GetAttacked(sword.damage,this.transform);
                    StartCoroutine(ReactivateAttack(sword.attackTime));
                }
            }
            else if (abilitySystem.canTear)
            {
                Tearable toTear;

                if (((Tearable)(detector.GetNear("Tree"))))
                    toTear = ((Tearable)(detector.GetNear("Tree")));
                else
                    toTear = ((Tearable)(detector.GetNear("Rock")));

                toTear.TearDown();

                StartCoroutine(ReactivateAttack(toTear.tearDownTime));
            }
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

    IEnumerator ReactivateShake(float time)
    {
        shaking = true;
        yield return new WaitForSecondsRealtime(time);
        shaking = false;
    }
    IEnumerator ReactivateAttack(float time)
    {
        attacking = true;
        yield return new WaitForSecondsRealtime(time);
        attacking = false;
    }
}
