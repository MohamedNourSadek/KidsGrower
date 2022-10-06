using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

enum MovementStatus { Move, Picked, Idel, Explore, Sleep, Eat, Lay, Ball, Pet };
enum BooleanStates { bored, tired, fruitNear, ballNear, alterNear, Picked, OnGround, Pet }
enum TriggerStates { foundTarget, lostTarget, doneEating, doneLaying, doneSleeping, doneBalling, reached }


public class NPC : Pickable, IController, IStateMachineController, ISavable
{
    public static int NPCsCount = 0;

    [SerializeField] HandSystem handSystem;
    [SerializeField] DetectorSystem detector;
    [SerializeField] GroundDetector groundDetector;


    [Header("Growing Parameters")]
    [SerializeField] LevelController levelController;
    [SerializeField] float grownBodyMultiplier = 1.35f;
    [SerializeField] float grownMassMultiplier = 1.35f;


    [Header("Character parameters")]
    [SerializeField] float nearObjectDistance = 1f;
    [SerializeField] float decisionsDelay = 0.5f;
    [SerializeField] float punchForce = 120f;
    [SerializeField] float explorationAmplitude = 10f;
    [SerializeField] public float layingTimeInBetween = 60f;
    [SerializeField] public float eatingXpPerUpdate = 1f;
    [SerializeField] public float pettingXP = 100f;

    [Header("AI Parameters")]
    [SerializeField] public float layingTime = 10f;
    [SerializeField] public float eatTime = 10f;

    [Header("Saved Parameters")]
    [SerializeField] public List<AIParameter> aIParameters = new List<AIParameter>();
     
    [Header("References")]
    [SerializeField] AIStateMachine aiStateMachine;
    [SerializeField] GameObject model;
    [SerializeField] List<MeshRenderer> bodyRenderers;
    [SerializeField] Material grownMaterial;
    [SerializeField] GameObject eggAsset;
    [SerializeField] GameObject deadNpcAsset;


    //Private data
    public List<GameObject> wantToFollow = new List<GameObject>();
    NavMeshAgent myAgent;
    public float bornSince = 0f;
    bool petting = false;
    bool canLay = false;
    string saveName = "Nameless";

    //Helper functions
    void Awake()
    {
        NPCsCount++;

        myAgent = GetComponent<NavMeshAgent>();
        myAgent.stoppingDistance = 0.9f * nearObjectDistance;
        detector.Initialize(nearObjectDistance);
        handSystem.Initialize(detector, this);
        groundDetector.Initialize();
        levelController.Initialize(OnLevelIncrease, OnXPIncrease);

        MovementStatus state = MovementStatus.Idel;
        aiStateMachine.Initialize((Enum)state);

        aIParameters = DataManager.instance.GetCurrentSession().aIParameters;

        foreach (DetectableElement element in detector.detectableElements)
        {
            element.OnNear += OnDetectableNear;
            element.OnInRange += OnDetectableInRange;
            element.OnInRangeExit += OnDetectableExit;
        }

        UIGame.instance.CreateNPCUi(this.gameObject, this.transform);
        UIGame.instance.UpateNpcUiElement(this.gameObject, saveName);

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
        detector.Update();

        if (groundDetector.IsOnWater(myBody))
            Die();
    }
    bool GotTypeInHand(Type type)
    {
        if (handSystem.GetObjectInHand() != null && handSystem.GetObjectInHand().GetType() == type)
            return true;
        else
            return false;
    }


    //Interface
    public void LoadData(SaveStructure saveData)
    {
        NPC_Data npc_data = (NPC_Data)saveData;

        saveName = npc_data.name;
        transform.position = npc_data.position.GetVector();
        transform.rotation = npc_data.rotation.GetQuaternion();
        bornSince = npc_data.bornSince;
        levelController.IncreaseXP(npc_data.xp);

        UIGame.instance.UpateNpcUiElement(this.gameObject, saveName);

        OnXPIncrease();
    }
    public NPC_Data GetData()
    {
        NPC_Data npc_data = new NPC_Data();

        npc_data.name = saveName;
        npc_data.position = new nVector3(transform.position);
        npc_data.rotation = new nQuaternion(transform.rotation);
        npc_data.bornSince = bornSince;
        npc_data.xp = levelController.GetXp();

        return npc_data;
    }
    public override void Pick(HandSystem picker)
    {
        base.Pick(picker);
        myAgent.enabled = false;
        aiStateMachine.SetBool(BooleanStates.Picked, true);
        handSystem.DropObject();
        UIGame.instance.UpateNpcUiElement(this.gameObject, "");
    }
    public override void Drop()
    {
        base.Drop();

        UIGame.instance.UpateNpcUiElement(this.gameObject, saveName);
        aiStateMachine.SetBool(BooleanStates.Picked, false);
    }
    public void StartPetting()
    {
        aiStateMachine.SetBool(BooleanStates.Pet, true);
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

            aiStateMachine.SetBool(BooleanStates.Pet, false);
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

        canLay = true;
        GrowUp();

        float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.DeathTime);

        while ((bornSince < parameter))
        {
            bornSince += Time.fixedDeltaTime;
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
        var deadNPC =  Instantiate(deadNpcAsset, this.transform.position, Quaternion.identity);
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

        if(inUi)
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


    //AI private variables
    Vector3 deltaExploration;
    public Transform dynamicDestination;


    //AI Decision Making
    IEnumerator AiDescrete()
    {
        while (true)
        {
            if (wantToFollow.Count >= 1)
            {
                GameObject obj = detector.GetHighestProp(wantToFollow);

                if (obj != dynamicDestination)
                {
                    if (obj.tag == "Tree" && obj.GetComponent<TreeSystem>().GotFruit())
                        SetDes(obj); 
                    else if (obj.tag != "Tree")
                        SetDes(obj);
                }
            }

            float boredProb = AIParameter.GetValue(aIParameters, AIParametersNames.BoredTime);

            if ((aiStateMachine.GetTimeSinceLastChange() >= boredProb))
                aiStateMachine.SetBool(BooleanStates.tired, true);
            else
                aiStateMachine.SetBool(BooleanStates.tired, false);

            if ((aiStateMachine.GetTimeSinceLastChange() >= (boredProb / 4f)) && (IsCurrentState(MovementStatus.Idel)))
                aiStateMachine.SetBool(BooleanStates.bored, true);
            else
                aiStateMachine.SetBool(BooleanStates.bored, false);

            //i got a throwable object (ball).
            if (GotTypeInHand(typeof(Ball)))
            {
                if (detector.GetDetectable("Player").detectionStatus == DetectionStatus.InRange)
                {
                    float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.ThrowBallOnPlayerProb);

                    ThinkAboutThrowing(((PlayerSystem)(detector.DetectableInRange("Player"))).gameObject, parameter);
                }
                if (detector.GetDetectable("NPC").detectionStatus == DetectionStatus.InRange)
                {
                    float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.ThrowBallOnNpcProb);

                    ThinkAboutThrowing(((NPC)(detector.DetectableInRange("NPC"))).gameObject, parameter);
                }

                //No One is near
                ThinkaboutDroppingTheBall();
            }

            aiStateMachine.SetBool(BooleanStates.OnGround, groundDetector.IsOnGroud(this.myBody));

            yield return new WaitForSecondsRealtime(decisionsDelay);
        }
    }
    IEnumerator AiContinous()
    {
        while (true)
        {
            if (IsCurrentState(MovementStatus.Move))
            {
                if (dynamicDestination && myAgent.isActiveAndEnabled)
                    myAgent.destination = dynamicDestination.position;

                CheckReached();
            }
            else if(IsCurrentState(MovementStatus.Explore))
            {
                CheckReached();
            }

            //Reactivate AI only if the npc were thrown and touched the ground and not being bet
            if (groundDetector.IsOnGroud(myBody) && !petting)
                myAgent.enabled = true;
            else if (petting)
                myAgent.enabled = false;

            //When the ball is dropped
            if( (IsCurrentState(MovementStatus.Ball)) && handSystem.GetObjectInHand() == null)
            {
                aiStateMachine.SetTrigger(TriggerStates.doneBalling);
            }

            //See if there's a near fruit that is clear to eat
            if ((detector.GetDetectable("Fruit").detectionStatus == DetectionStatus.VeryNear) && handSystem.GetObjectInHand() == null && !((Fruit)detector.DetectableInRange("Fruit")).IsPicked())
                aiStateMachine.SetBool(BooleanStates.fruitNear, true);
            else
                aiStateMachine.SetBool(BooleanStates.fruitNear, false);

            //See if there's a near ball that is clear to eat
            if ((detector.GetDetectable("Ball").detectionStatus == DetectionStatus.VeryNear) && handSystem.GetObjectInHand() == null)
                aiStateMachine.SetBool(BooleanStates.ballNear, true);
            else
                aiStateMachine.SetBool(BooleanStates.ballNear, false);

            //see if there's a near alter
            if ((detector.GetDetectable("Alter").detectionStatus == DetectionStatus.VeryNear) && canLay)
                aiStateMachine.SetBool(BooleanStates.alterNear, true);
            else
                aiStateMachine.SetBool(BooleanStates.alterNear, false);

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }
    public void ActionExecution(Enum action)
    {
        MovementStatus state = (MovementStatus)action;

        if (state == MovementStatus.Explore)
        {
            ExplorePoint();
        }
        else if (state == MovementStatus.Sleep)
        {
            StartCoroutine(Sleeping());
        }
        else if (state == MovementStatus.Eat)
        {
            bool willEat = false;

            if (handSystem.GetNearest())
            {
                if ((handSystem.GetNearest()).tag == "Fruit")
                {
                    handSystem.PickObject();

                    if ((Fruit)(handSystem.GetObjectInHand()))
                        willEat = true;
                }
            }

            //so that if eating failed, it tells the _state machine.
            if (willEat)
                StartCoroutine(Eating((Fruit)(handSystem.GetObjectInHand())));
            else
                aiStateMachine.SetTrigger(TriggerStates.doneEating);

        }
        else if (state == MovementStatus.Lay)
        {
            StartCoroutine(Laying());
        }
        else if (state == MovementStatus.Ball)
        {
            if ((handSystem.GetNearest()).tag == "Ball" && (handSystem.GetObjectInHand() == null))
                handSystem.PickObject();
        }
    }
    void OnDetectableInRange(IDetectable detectable)
    {
        if (detectable.tag == "Player")
        {
            float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.SeekPlayerProb);
            ThinkAboutFollowingObject(((PlayerSystem)detectable).gameObject, parameter);
        }

        if (detectable.tag == ("NPC"))
        {
            float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.SeekNpcProb);
            ThinkAboutFollowingObject(((NPC)detectable).gameObject, parameter);
        }

        if (detectable.tag == ("Ball"))
        {
            float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.SeekBallProb);
            ThinkAboutFollowingObject(((Ball)detectable).gameObject, parameter);
        }

        if (detectable.tag == ("Tree"))
        {
            float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.SeekTreeProb);

            if (((TreeSystem)detectable).GotFruit())
            {
                ThinkAboutFollowingObject(((TreeSystem)detectable).gameObject, parameter);
            }
        }

        if (detectable.tag == ("Fruit"))
            if (((Fruit)detectable).GetComponent<Fruit>().OnGround())
            {
                float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.SeekFruitProb);
                ThinkAboutFollowingObject(((Fruit)detectable).gameObject, parameter);
            }

        if (detectable.tag == ("Alter"))
            if (canLay)
            {
                float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.SeekAlterProb);
                ThinkAboutFollowingObject(((FertilityAlter)detectable).gameObject, parameter);
            }
    }
    void OnDetectableExit(IDetectable detectable)
    {
        //If the object i am following got out of range
        if (dynamicDestination == ((MonoBehaviour)detectable).gameObject.transform)
        {
            if (IsCurrentState(MovementStatus.Move))
                aiStateMachine.SetTrigger(TriggerStates.lostTarget);
        }

        if (wantToFollow.Contains(detectable.GetGameObject()))
            wantToFollow.Remove(detectable.GetGameObject());
    }
    void OnDetectableNear(IDetectable detectable)
    {
        if (detectable.tag == ("NPC"))
            ThinkAboutPunchingAnNpc(((NPC)(detectable)).myBody);

        if (detectable.tag == ("Tree"))
            ThinkAboutShakingTree((TreeSystem)detectable);
    }


    //Internal Algorithms
    IEnumerator Eating(Fruit fruit)
    {
        float time = 0;
        ConditionChecker condition = new ConditionChecker(!isPicked);
        UIGame.instance.ShowRepeatingMessage("Eating", this.transform, eatTime, 15, condition);

        while (condition.isTrue)
        {
            time += Time.fixedDeltaTime;

            bool timeCond = (time <= eatTime);
            bool fruitInHand = GotTypeInHand(typeof(Fruit));
            bool hasEnergy = fruit.HasEnergy();

            condition.Update(IsCurrentState(MovementStatus.Eat) && timeCond && fruitInHand && hasEnergy);

            levelController.IncreaseXP(fruit.GetEnergy());

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        handSystem.DropObject();

        aiStateMachine.SetTrigger(TriggerStates.doneEating);
    }
    IEnumerator Sleeping()
    {
        float time = 0;
        ConditionChecker condition = new ConditionChecker(!isPicked);

        float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.SleepTime);

        UIGame.instance.ShowRepeatingMessage("Sleeping", this.transform, parameter, 15, condition);

        //sleep
        myBody.isKinematic = true;
        myAgent.enabled = false;

        while (condition.isTrue)
        {
            time += Time.fixedDeltaTime;

            condition.Update((time <= parameter) && IsCurrentState(MovementStatus.Sleep));

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        //Wake up
        aiStateMachine.SetTrigger(TriggerStates.doneSleeping);

        if (!isPicked)
        {
            myAgent.enabled = true;
            myBody.isKinematic = false;
        }
    }
    IEnumerator Laying()
    {
        float time = 0;
        ConditionChecker condition = new ConditionChecker(!isPicked);
        UIGame.instance.ShowRepeatingMessage("Laying", this.transform, layingTime, 15, condition);

        //Laying
        myBody.isKinematic = true;
        myAgent.enabled = false;

        while (condition.isTrue)
        {
            time += Time.fixedDeltaTime;

            condition.Update((time <= layingTime) && IsCurrentState(MovementStatus.Lay));

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        //Done
        if (!isPicked)
        {
            myAgent.enabled = true;
            myBody.isKinematic = false;
        }


        if (time >= layingTime)
        {
            canLay = false;

            Egg egg = Instantiate(eggAsset.gameObject, this.transform.position + Vector3.up, Quaternion.identity).GetComponent<Egg>();
            egg.SetRottenness(1f - levelController.GetLevelToLevelsRation());

            aiStateMachine.SetTrigger(TriggerStates.doneLaying);

            //Reset the ability to lay
            time = 0;
            while (time <= layingTimeInBetween)
            {
                time += Time.fixedDeltaTime;

                yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
            }

            canLay = true;
        }
    }
    void ThinkAboutShakingTree(TreeSystem tree)
    {
        float randomChance = UnityEngine.Random.Range(0f, 1f);

        float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.SeekTreeProb);

        if ((randomChance < parameter) && (randomChance > 0))
        {
            if(tree.GotFruit() && !IsCurrentState(MovementStatus.Sleep))
                tree.Shake();
        }
    }
    void ThinkAboutPunchingAnNpc(Rigidbody body)
    {
        float randomChance = UnityEngine.Random.Range(0f, 1f);

        float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.PunchNpcProb);

        if ((randomChance < parameter) && (randomChance > 0))
        {
            Vector3 direction = (body.transform.position - this.transform.position).normalized;
            body.AddForce(direction * punchForce, ForceMode.Impulse);
        }
    }
    void ThinkAboutThrowing(GameObject target, float chance)
    {
        float randomChance = UnityEngine.Random.Range(0f, 1f);

        if ((randomChance < chance) && (randomChance > 0))
        {
            handSystem.ThrowObject((target.transform.position));
        }
    }
    void ThinkaboutDroppingTheBall()
    {
        float randomChance = UnityEngine.Random.Range(0f, 1f);

        float parameter = AIParameter.GetValue(aIParameters, AIParametersNames.DropBallProb);

        if ((randomChance < parameter) && (randomChance > 0))
        {
            handSystem.DropObject();
        }
    }
    void ThinkAboutFollowingObject(GameObject obj, float chance)
    {
        float randomChance = UnityEngine.Random.Range(0f, 1f);

        if((randomChance < chance) && (randomChance > 0))
        {
            if (!wantToFollow.Contains(obj))
                wantToFollow.Add(obj);
        }
    }
    void ExplorePoint()
    {
        deltaExploration = Vector3.zero;
        Vector3 destination = MapSystem.instance.GetRandomExplorationPoint();

        float distance = (this.transform.position - myAgent.destination).magnitude;

        if (distance <= nearObjectDistance)
        {
            float x = UnityEngine.Random.Range(0f, explorationAmplitude);
            float z = UnityEngine.Random.Range(0f, explorationAmplitude);

            deltaExploration = new Vector3(x, 0, z);
        }

        if(myAgent.isActiveAndEnabled)
            myAgent.destination = destination + deltaExploration;

    }
    void SetDes(GameObject obj)
    {
        if(myAgent.isActiveAndEnabled)
        {
            dynamicDestination = obj.transform;
            myAgent.destination = dynamicDestination.position;
            aiStateMachine.SetTrigger(TriggerStates.foundTarget);
        }
    }
    void CheckReached()
    {
        if(myAgent.isActiveAndEnabled)
            if (myAgent.remainingDistance <= nearObjectDistance)
               aiStateMachine.SetTrigger(TriggerStates.reached);
    }
    bool IsCurrentState(MovementStatus state)
    {
        if (((MovementStatus)aiStateMachine.GetCurrentState()) == state)
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


