using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class NPC : Pickable, IController, ISavable
{
    public static int NPCsCount = 0;

    [SerializeField] public HandSystem handSystem;
    [SerializeField] public DetectorSystem detector;
    [SerializeField] AppearanceControl appearanceControl;
    [SerializeField] FacialControl facialControl;
    [SerializeField] Animator animator;
    [SerializeField] float animationLerpSpeed = 1f;
    [SerializeField] FacialControl myFace;
    [SerializeField] AiBehaviour aiBehavior;

    [Header("Character parameters")]
    [Header("Dynamic")]
    [SerializeField] public CharacterParameters character;
    [Header("Static")]
    [SerializeField] float grownMassMultiplier = 1.35f;
    [SerializeField] public float nearObjectDistance = 1f;
    [SerializeField] public float eatingXpPerUpdate = 1f;
    [SerializeField] public float pettingXP = 100f;

    //Internal
    [System.NonSerialized] public NavMeshAgent navMeshAgent;
    bool petting = false;
    float moveAnimtion = 0f;
    
    //Helper functions
    void Start()
    {
        NPCsCount++;

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = 0.9f * nearObjectDistance;
        detector.Initialize(nearObjectDistance, aiBehavior.OnDetectableInRange, aiBehavior.OnDetectableExit, aiBehavior.OnDetectableNear, aiBehavior.OnDetectableNearExit);
        handSystem.Initialize(detector, this);
        appearanceControl.Initialize(this);
        myFace.Initialize();

        groundDetector.Initialize(myBody);
        character.levelControl.Initialize(OnLevelIncrease, OnXPIncrease);

        UIGame.instance.CreateNPCUi(this.gameObject, this.transform);
        UIGame.instance.UpateNpcUiElement(this.gameObject, character.saveName);

        StartCoroutine(GrowingUp());
        aiBehavior.Initialize(this);
    }
    void OnDestroy()
    {
        NPCsCount--;
    }
    void Update()
    {
        handSystem.Update();
        appearanceControl.UpdateAppearance();
        ApplyCharacterParameters();
        UpdateAnimationParameters();

        if (groundDetector.IsOnLayer(GroundLayers.Water))
            Die();

        //Reactivate AI only if the npc were thrown and touched the ground and not being bet
        if (groundDetector.IsOnLayer(GroundLayers.Ground) && !petting)
            navMeshAgent.enabled = true;
        else if (petting)
            navMeshAgent.enabled = false;
    }

    //Interface
    public void LoadData(SaveStructure saveData)
    {
        NPC_Data npc_data = (NPC_Data)saveData;

        transform.position = npc_data.position.GetVector();
        transform.rotation = npc_data.rotation.GetQuaternion();
        character = npc_data.characterParameters;

        OnXPIncrease();
    }
    public NPC_Data GetData()
    {
        NPC_Data npc_data = new NPC_Data();

        npc_data.position = new nVector3(transform.position);
        npc_data.rotation = new nQuaternion(transform.rotation);
        npc_data.characterParameters = character;

        return npc_data;
    }
    public override void Pick(HandSystem picker)
    {
        base.Pick(picker);
        navMeshAgent.enabled = false;
        handSystem.DropObjectInHand();
        UIGame.instance.UpateNpcUiElement(this.gameObject, "");
        aiBehavior.AbortCurrentAction();
    }
    public override void Drop()
    {
        base.Drop();
        UIGame.instance.UpateNpcUiElement(this.gameObject, character.saveName);
    }
    public void StartPetting()
    {
        PlayerSystem.instance.LockPlayer(false);

        character.levelControl.IncreaseXP(pettingXP);
        myBody.isKinematic = true;
        myBody.velocity = Vector3.zero;
        petting = true;
    }
    public void EndPetting()
    {
        if (myBody != null)
        {
            myBody.isKinematic = false;
            petting = false;

            PlayerSystem.instance.LockPlayer(true);
        }
    }
    public bool GotEatableInHand()
    {
        if (handSystem.GetObjectInHand() != null && handSystem.GetObjectInHand().GetType().IsSubclassOf(typeof(Eatable)))
            return true;
        else
            return false;
    }
    public GroundDetector GetGroundDetector()
    {
        return groundDetector;
    }


    //Growing Up
    IEnumerator GrowingUp()
    {
        while ((character.age < character.growTime))
        {
            character.age += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        GrowUp();

        while ((character.age < character.deathTime))
        {
            character.age += Time.fixedDeltaTime;
            character.lastLaidSince += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        Die();

    }
    void GrowUp()
    {
        myBody.mass = grownMassMultiplier * myBody.mass;
    }
    void Die()
    {
        var deadNPC = GameManager.instance.SpawnDeadBody_ReturnDeadBody(this.transform.position);
        UIGame.instance.DestroyNpcUiElement(this.gameObject);
        UIGame.instance.ShowRepeatingMessage("Death!!", deadNPC.transform, 2f, 4f, new ConditionChecker(true));
        UIGame.instance.ShowDeclare(character.saveName + " has Died!");

        Destroy(this.gameObject);
    }


    //UI-Level Functions
    public void OnXPIncrease()
    {
    }
    public void OnLevelIncrease()
    {
        UIGame.instance.ShowRepeatingMessage("Level Up", this.transform, 0.5f, 4, new ConditionChecker(true));
        UIGame.instance.ShowDeclare(character.saveName + " has Leveled Up!");
    }
    public void ChangeName(string newName, bool inUi)
    {
        character.saveName = newName;

        if (inUi)
            UIGame.instance.UpateNpcUiElement(this.gameObject, character.saveName);
    }


    //Functions
    void UpdateAnimationParameters()
    {
        Vector2 horizontalVelocity = new Vector2(navMeshAgent.velocity.x, navMeshAgent.velocity.z);
        float finalMoveX = Mathf.Clamp01((horizontalVelocity.magnitude / character.maxSpeed));
        moveAnimtion = Mathf.Lerp(moveAnimtion, finalMoveX, Time.fixedDeltaTime * animationLerpSpeed);
        animator.SetFloat("MoveX", moveAnimtion);
    }
    void ApplyCharacterParameters()
    {
        navMeshAgent.speed = character.GetSpeed();
    }
}


