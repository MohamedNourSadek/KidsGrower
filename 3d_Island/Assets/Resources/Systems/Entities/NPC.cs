using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class NPC : Pickable, IController, ISavable
{
    public static int NPCsCount = 0;

    [SerializeField] public HandSystem handSystem;
    [SerializeField] DetectorSystem detector;
    [SerializeField] GroundDetector groundDetector;

    [Header("Character parameters")]
    [Header("Dynamic")]
    [SerializeField] public CharacterParameters character;
    [Header("Static")]
    [SerializeField] public float nearObjectDistance = 1f;
    [SerializeField] public float eatingXpPerUpdate = 1f;
    [SerializeField] public float pettingXP = 100f;
    [SerializeField] float decisionsDelay = 0.5f;


    [Header("AI")]
    [SerializeField] AbstractAction currentAction;
    [SerializeField] List<AbstractAction> actions = new List<AbstractAction>();

    [Header("Appearance")]
    [SerializeField] MeshRenderer upperBody;
    [SerializeField] MeshRenderer face;
    [SerializeField] MeshRenderer downBody;
    [SerializeField] List<MeshRenderer> handsLegs;
    [SerializeField] List<GameObject> horns;


    [Header("Scale")]
    [SerializeField] GameObject wholeBody;
    [SerializeField] float grownMassMultiplier = 1.35f;
    [SerializeField] Vector3 initialScale = new Vector3(1f, 1f, 1f);
    [SerializeField] Vector3 finalScale = new Vector3(5f, 5f, 5f);

    [Header("Colors")]
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color extrovertColor;
    [SerializeField] Color fertilityColor;
    [SerializeField] Color powerColor;
    [SerializeField] Color healthColor;


    [Header("References")]
    [SerializeField] GameObject eggAsset;
    [SerializeField] GameObject deadNpcAsset;

    public NavMeshAgent myAgent;
    bool petting = false;

    //Helper functions
    void Start()
    {
        NPCsCount++;

        myAgent = GetComponent<NavMeshAgent>();
        myAgent.stoppingDistance = 0.9f * nearObjectDistance;
        detector.Initialize(nearObjectDistance, OnDetectableInRange, OnDetectableExit, OnDetectableNear, OnDetectableNearExit);
        handSystem.Initialize(detector, this);
        groundDetector.Initialize();
        character.levelControl.Initialize(OnLevelIncrease, OnXPIncrease);

        UIGame.instance.CreateNPCUi(this.gameObject, this.transform);
        UIGame.instance.UpateNpcUiElement(this.gameObject, character.saveName);

        base.StartCoroutine(GrowingUp());
        base.StartCoroutine(AiContinous());
        base.StartCoroutine(AiDescrete());
    }
    void OnDestroy()
    {
        NPCsCount--;
    }
    void Update()
    {
        handSystem.Update();
        UpdateChildAppearance();
        ApplyCharacterParameters();

        if (groundDetector.IsOnWater(myBody))
            Die();

        //Reactivate AI only if the npc were thrown and touched the ground and not being bet
        if (groundDetector.IsOnGroud(myBody) && !petting)
            myAgent.enabled = true;
        else if (petting)
            myAgent.enabled = false;
    }


    //Interface
    public bool GotTypeInHand(Type type)
    {
        if (handSystem.GetObjectInHand() != null && handSystem.GetObjectInHand().GetType() == type)
            return true;
        else
            return false;
    }
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
        myAgent.enabled = false;
        handSystem.DropObject();
        UIGame.instance.UpateNpcUiElement(this.gameObject, "");
        AbortAction(currentAction);
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
        var deadNPC = Instantiate(deadNpcAsset, this.transform.position, Quaternion.identity);
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
    public string GetName()
    {
        return character.saveName;
    }
    public int GetLevel()
    {
        return character.levelControl.GetLevel();
    }
    public float GetXp()
    {
        return character.levelControl.GetXp();
    }


    //AI
    IEnumerator AiDescrete()
    {
        while(true)
        {
            GenerateActionIfEmptyAction();
            
            InturrptActionsWithLowPriority();

            yield return new WaitForSecondsRealtime(decisionsDelay);
        }
    }
    IEnumerator AiContinous()
    {
        while(true)
        {
            //Execute Actions
            if (currentAction.actionName == "" && actions.Count > 0 && !isPicked)
            {
                currentAction = ReprioritizeActions();

                if (currentAction == null)
                {
                    yield return new WaitForFixedUpdate();
                    continue;
                }
                else if (currentAction.subject == null)
                {
                    currentAction.actionName = "";
                 
                    yield return new WaitForFixedUpdate();
                    continue;
                }
                else if(handSystem.GetObjectInHand())
                {
                    if(((Pickable)handSystem.GetObjectInHand()) != null)
                    {
                        //currentAction = AbstractAction.ActionFactory(ActionTypes.Drop, handSystem.GetObjectInHand().gameObject, this);
                    }
                }

                if(actions.Contains(currentAction))
                    actions.Remove(currentAction);

                currentAction.Execute();

                while (currentAction.IsDone() == false)
                {
                    yield return new WaitForFixedUpdate();
                }

                currentAction.actionName = "";
            }
            yield return new WaitForFixedUpdate();
        }
    }


    //AI - Decision Making
    void InturrptActionsWithLowPriority()
    {
        if (actions.Count > 0)
            if (ReprioritizeActions().GetPriority() > currentAction.GetPriority())
                AbortAction(currentAction);
    }
    AbstractAction ReprioritizeActions()
    {
        CleanFaultyActions();

        if (actions.Count > 0)
        {
            AbstractAction action = actions[0];

            foreach (AbstractAction act in actions)
                if (act.GetPriority() > action.GetPriority())
                    action = act;

            return action;
        }
        else
            return new AbstractAction(this.gameObject,this);

    }



    //AI - Intercepting Information
    void OnDetectableInRange(IDetectable detectable)
    {
        if (detectable.tag == "Fruit")
        {
            if (((Fruit)detectable).OnGround())
            {
                if(FlipACoinWithProb(character.GetExtroversion()))
                {
                    AddAction(detectable, ActionTypes.Follow);
                    AddAction(detectable, ActionTypes.Eat);
                }
            }
        }
        else if (detectable.tag == "Alter")
        {
            if (character.CanLay() && FlipACoinWithProb(character.seekAlterProb))
            {
                AddAction(detectable, ActionTypes.Follow);
                AddAction(detectable, ActionTypes.Lay);
            }
        }
        else if (detectable.tag == "Ball")
        {

            if ((((Ball)detectable).IsPicked() == false) && FlipACoinWithProb(character.GetExtroversion()))
            {
                AddAction(detectable, ActionTypes.Follow);
                AddAction(detectable, ActionTypes.Pick);
            }
        }
        else if ((detectable.tag == "Player"))
        {
            if ((handSystem.GetObjectInHand() != null) && (handSystem.GetObjectInHand().tag == "Ball"))
            {
                if(FlipACoinWithProb(character.GetAggressiveness()))
                    AddAction(detectable, ActionTypes.Throw);
            }
            else
            {
                if (FlipACoinWithProb(character.GetExtroversion()))
                {
                    AddAction(detectable, ActionTypes.Follow);
                    if (FlipACoinWithProb(character.GetAggressiveness()))
                        AddAction(detectable, ActionTypes.Punch);
                }
            }
        }
        else if ((detectable.tag == "NPC"))
        {

            if ((handSystem.GetObjectInHand() != null) && (handSystem.GetObjectInHand().tag == "Ball"))
            {
                if (FlipACoinWithProb(character.GetAggressiveness()))
                    AddAction(detectable, ActionTypes.Throw);
            }
            else
            {
                if (FlipACoinWithProb(character.GetExtroversion()))
                {
                    AddAction(detectable, ActionTypes.Follow);

                    if (FlipACoinWithProb(character.GetAggressiveness()))
                        AddAction(detectable, ActionTypes.Punch);
                }
            }

        }
        else if ((detectable.tag == "Tree"))
        {
            if (((TreeSystem)detectable).GotFruit() && FlipACoinWithProb(character.GetExtroversion()))
            {
                AddAction(detectable, ActionTypes.Follow);
                AddAction(detectable, ActionTypes.Shake);
            }
        }
    }
    void OnDetectableExit(IDetectable detectable)
    {
        //Only cancel if it's far away, not in your hand
        if(detectable.GetGameObject() != null)
            if(!detector.IsNear(detectable.GetGameObject()))
                AbortAction(detectable);
    }
    void OnDetectableNear(IDetectable detectable)
    {
    }
    void OnDetectableNearExit(IDetectable detectable)
    {
    }


    //AI - Adding/Removing Actions (Instead of a factory)
    void AddAction(IDetectable subject, ActionTypes actionType)
    {
        if (subject != null && subject.GetGameObject() != null)
        {
            AbstractAction newAction = AbstractAction.ActionFactory(actionType, subject.GetGameObject(), this);
            actions.Add(newAction);
        }
    }
    void AbortAction(IDetectable subject)
    {
        if (actions.Count > 0)
        {
            List<AbstractAction> toRemove = new List<AbstractAction>();

            foreach (AbstractAction action in actions)
            {
                if(action.subject != null && subject != null && subject.GetGameObject() != null)
                    if (action.subject == subject.GetGameObject())
                       toRemove.Add(action);
            }

            foreach (AbstractAction action in toRemove)
            {
                actions.Remove(action);
                action.isDone = true;
            }
        }

        if (currentAction.subject != null && subject != null && subject.GetGameObject() != null)
            if (currentAction.subject == subject.GetGameObject())
                AbortAction(currentAction);
    }
    void AbortAction(AbstractAction action)
    {
        if(action != null)
        {
            action.Abort();
            action.actionName = "";
            if (myAgent.isActiveAndEnabled)
                myAgent.SetDestination(this.transform.position);
        }

    }
    AbstractAction GetRandomIdleAction()
    {
        float randomNum = UnityEngine.Random.Range(0f, 1f);
        AbstractAction action;
        if (randomNum >= 0.67)
            action = new Action_Explore(this.gameObject, this);
        else if (randomNum >= 0.33)
            action = new Action_Sleep(this.gameObject, this);
        else
            action = new Action_Idle(this.gameObject, this);

        return action;
    }
    void GenerateActionIfEmptyAction()
    {
        if ((actions.Count == 0) && currentAction.actionName == "" && !isPicked)
        {
            if (detector.GetInRange().Count == 0 && detector.GetNear().Count == 0)
            {
                actions.Add(GetRandomIdleAction());
            }
            else
            {
                foreach (IDetectable detectable in detector.GetInRange())
                    OnDetectableInRange(detectable);

                foreach (IDetectable detectable in detector.GetNear())
                    OnDetectableNear(detectable);
            }
        }
    }


    //Functions
    void UpdateChildAppearance()
    {
        float ageFactor = character.age / character.deathTime;
        wholeBody.transform.localScale = initialScale + (ageFactor * (finalScale-initialScale));

        //face
        face.material.color = Color.Lerp(normalColor, healthColor, character.GetHealth());

        //upper
        upperBody.material.color = Color.Lerp(normalColor, extrovertColor, character.GetExtroversion());

        //down
        downBody.material.color = Color.Lerp(normalColor, fertilityColor, character.GetFertility());

        //Horns
        foreach (GameObject obj in horns)
            obj.transform.localScale = new Vector3(obj.transform.localScale.x, 0, obj.transform.localScale.z) + (Vector3.up * character.GetAggressiveness());

        //hands and legs
        foreach (MeshRenderer renderer in handsLegs)
            renderer.material.color = Color.Lerp(normalColor, powerColor, character.GetPower());
    }
    void ApplyCharacterParameters()
    {
        myAgent.speed = character.GetSpeed();
    }
    bool FlipACoinWithProb(float prob)
    {
        float random = UnityEngine.Random.Range(0f, 1f);

        if (random <= prob)
            return true;
        else
            return false;
    }
    void CleanFaultyActions()
    {
        List<AbstractAction> faultyActions = new List<AbstractAction>();

        foreach (AbstractAction act in actions)
            if (act == null || act.actionName == "" || act.subject == null)
                faultyActions.Add(act);

        foreach (AbstractAction act in faultyActions)
            actions.Remove(act);
    }
    void OnDrawGizmos()
    {
        if(myAgent)
            Gizmos.DrawSphere(myAgent.destination, .2f);
    }
}


