using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;

public class NPC : Pickable, IController, ISavable
{
    public static int NPCsCount = 0;

    [SerializeField] public HandSystem handSystem;
    [SerializeField] DetectorSystem detector;
    [SerializeField] GroundDetector groundDetector;

    [Header("Growing Parameters")]
    [SerializeField] public LevelController levelController;
    [SerializeField] float grownBodyMultiplier = 1.35f;
    [SerializeField] float grownMassMultiplier = 1.35f;

    [Header("Character parameters")]
    [SerializeField] public float nearObjectDistance = 1f;
    [SerializeField] public float punchForce = 120f;
    [SerializeField] public float eatingXpPerUpdate = 1f;
    [SerializeField] public float pettingXP = 100f;
    [SerializeField] public float maxFertilityAge = 250f;
    [SerializeField] float decisionsDelay = 0.5f;
    [SerializeField] float betweenLaysTime = 50f;

    [Header("AI")]
    [SerializeField] AbstractAction currentAction;
    [SerializeField] List<AbstractAction> actions = new List<AbstractAction>();
    [SerializeField] public float layingTime = 10f;


    [Header("Saved Parameters")]
    [SerializeField] public List<AIParameter> aIParameters;

    [Header("References")]
    [SerializeField] GameObject model;
    [SerializeField] List<MeshRenderer> bodyRenderers;
    [SerializeField] Material grownMaterial;
    [SerializeField] GameObject eggAsset;
    [SerializeField] GameObject deadNpcAsset;

    public NavMeshAgent myAgent;
    public float bornSince = 0f;
    public float lastLaidSince = 50000f;
    bool petting = false;
    string saveName = "Nameless";
    bool dataLoaded; 

    //Helper functions
    void Start()
    {
        NPCsCount++;

        myAgent = GetComponent<NavMeshAgent>();
        myAgent.stoppingDistance = 0.9f * nearObjectDistance;
        detector.Initialize(nearObjectDistance, OnDetectableInRange, OnDetectableExit, OnDetectableNear, OnDetectableNearExit);
        handSystem.Initialize(detector, this);
        groundDetector.Initialize();
        levelController.Initialize(OnLevelIncrease, OnXPIncrease);

        UIGame.instance.CreateNPCUi(this.gameObject, this.transform);
        UIGame.instance.UpateNpcUiElement(this.gameObject, saveName);

        if(!dataLoaded)
            aIParameters = new NPC_Data(DataManager.instance.GetCurrentSession().aiSet).aIParameters;

        base.StartCoroutine(GrowingUp());
        base.StartCoroutine(AiContinous());
        base.StartCoroutine(AiDescrete());
    }
    void OnDestroy()
    {
        NPCsCount--;
    }
    public void Update()
    {
        handSystem.Update();

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

        saveName = npc_data.name;
        transform.position = npc_data.position.GetVector();
        transform.rotation = npc_data.rotation.GetQuaternion();
        bornSince = npc_data.bornSince;
        lastLaidSince = npc_data.lastLaidSince;
        levelController.IncreaseXP(npc_data.xp);
        aIParameters = npc_data.aIParameters;

        dataLoaded = true;

        OnXPIncrease();
    }
    public NPC_Data GetData()
    {
        NPC_Data npc_data = new NPC_Data(DataManager.instance.GetCurrentSession().aiSet);

        npc_data.name = saveName;
        npc_data.position = new nVector3(transform.position);
        npc_data.rotation = new nQuaternion(transform.rotation);
        npc_data.bornSince = bornSince;
        npc_data.xp = levelController.GetXp();
        npc_data.lastLaidSince = lastLaidSince;
        npc_data.aIParameters = aIParameters;

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
        UIGame.instance.UpateNpcUiElement(this.gameObject, saveName);
    }
    public void StartPetting()
    {
        PlayerSystem.instance.LockPlayer(false);

        levelController.IncreaseXP(pettingXP);
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
        float growingUpTime = AIParameter.GetValue(aIParameters, AIParametersNames.GrowTime);

        while ((bornSince < growingUpTime))
        {
            bornSince += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        GrowUp();

        float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.DeathTime);

        while ((bornSince < parameter))
        {
            bornSince += Time.fixedDeltaTime;
            lastLaidSince += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        Die();

    }
    void GrowUp()
    {
        myBody.mass = grownMassMultiplier * myBody.mass;
        model.transform.localScale = grownBodyMultiplier * model.transform.localScale;

        foreach (MeshRenderer mesh in bodyRenderers)
            mesh.material = grownMaterial;
    }
    void Die()
    {
        var deadNPC = Instantiate(deadNpcAsset, this.transform.position, Quaternion.identity);
        UIGame.instance.DestroyNpcUiElement(this.gameObject);
        UIGame.instance.ShowRepeatingMessage("Death!!", deadNPC.transform, 2f, 4f, new ConditionChecker(true));
        UIGame.instance.ShowDeclare(saveName + " has Died!");

        Destroy(this.gameObject);
    }


    //UI-Level Functions
    public void OnXPIncrease()
    {
    }
    public void OnLevelIncrease()
    {
        UIGame.instance.ShowRepeatingMessage("Level Up", this.transform, 0.5f, 4, new ConditionChecker(true));
        UIGame.instance.ShowDeclare(saveName + " has Leveled Up!");
    }
    public void ChangeName(string newName, bool inUi)
    {
        saveName = newName;

        if (inUi)
            UIGame.instance.UpateNpcUiElement(this.gameObject, saveName);
    }
    public string GetName()
    {
        return saveName;
    }
    public int GetLevel()
    {
        return levelController.GetLevel();
    }
    public float GetXp()
    {
        return levelController.GetXp();
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
    void CleanFaultyActions()
    {
        List<AbstractAction> faultyActions = new List<AbstractAction>();

        foreach (AbstractAction act in actions)
            if (act == null || act.actionName == "" || act.subject == null)
                faultyActions.Add(act);

        foreach (AbstractAction act in faultyActions)
            actions.Remove(act);
    }


    //AI - Intercepting Information
    void OnDetectableInRange(IDetectable detectable)
    {
        if (detectable.tag == "Fruit")
        {
            if (((Fruit)detectable).OnGround())
            {
                float followFruit = AIParameter.GetValue(aIParameters, AIParametersNames.SeekFruitProb);

                if(FlipACoinWithProb(followFruit))
                {
                    AddAction(detectable, ActionTypes.Follow);
                    AddAction(detectable, ActionTypes.Eat);
                }
            }
        }
        else if (detectable.tag == "Alter")
        {
            float followAlter = AIParameter.GetValue(aIParameters, AIParametersNames.SeekAlterProb);

            if (LayCondition() && FlipACoinWithProb(followAlter))
            {
                AddAction(detectable, ActionTypes.Follow);
                AddAction(detectable, ActionTypes.Lay);
            }
        }
        else if (detectable.tag == "Ball")
        {
            float followBall = AIParameter.GetValue(aIParameters, AIParametersNames.SeekBallProb);

            if ((((Ball)detectable).IsPicked() == false) && FlipACoinWithProb(followBall))
            {
                AddAction(detectable, ActionTypes.Follow);
                AddAction(detectable, ActionTypes.Pick);
            }
        }
        else if ((detectable.tag == "Player"))
        {
            float followPlayer = AIParameter.GetValue(aIParameters, AIParametersNames.SeekPlayerProb);
            float throwAtPlayer = AIParameter.GetValue(aIParameters, AIParametersNames.ThrowBallOnPlayerProb);

            if ((handSystem.GetObjectInHand() != null) && (handSystem.GetObjectInHand().tag == "Ball"))
            {
                if(FlipACoinWithProb(throwAtPlayer))
                    AddAction(detectable, ActionTypes.Throw);
            }
            else
            {
                if (FlipACoinWithProb(followPlayer))
                    AddAction(detectable, ActionTypes.Follow);
            }
        }
        else if ((detectable.tag == "NPC"))
        {
            float followNPC = AIParameter.GetValue(aIParameters, AIParametersNames.SeekNpcProb);
            float punchNPC = AIParameter.GetValue(aIParameters, AIParametersNames.PunchNpcProb);
            float throwAtNPC = AIParameter.GetValue(aIParameters, AIParametersNames.ThrowBallOnNpcProb);

            if ((handSystem.GetObjectInHand() != null) && (handSystem.GetObjectInHand().tag == "Ball"))
            {
                if (FlipACoinWithProb(throwAtNPC))
                    AddAction(detectable, ActionTypes.Throw);
            }
            else
            {
                if (FlipACoinWithProb(punchNPC) && FlipACoinWithProb(followNPC))
                {
                    AddAction(detectable, ActionTypes.Follow);
                    AddAction(detectable, ActionTypes.Punch);
                }

            }

        }
        else if ((detectable.tag == "Tree"))
        {
            float followTree = AIParameter.GetValue(aIParameters, AIParametersNames.SeekTreeProb);

            if (((TreeSystem)detectable).GotFruit() && FlipACoinWithProb(followTree))
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


    public bool LayCondition()
    {
        float growingUpTime = AIParameter.GetValue(aIParameters, AIParametersNames.GrowTime);

        return (lastLaidSince >= betweenLaysTime) && (levelController.GetLevel() >= 3) && (bornSince >= growingUpTime);
    }
    bool FlipACoinWithProb(float prob)
    {
        float random = UnityEngine.Random.Range(0f, 1f);

        if (random <= prob)
            return true;
        else
            return false;
    }

    void OnDrawGizmos()
    {
        if(myAgent)
            Gizmos.DrawSphere(myAgent.destination, .2f);
    }
}


