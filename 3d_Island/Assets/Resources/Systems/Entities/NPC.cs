using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

enum MovementStatus { Move, Picked, Idel, Explore, Sleep, Eat, Lay, Ball };
enum BooleanStates { bored, tired, fruitNear, ballNear, alterNear, Picked }
enum TriggerStates { foundTarget, lostTarget, doneEating, doneLaying, doneSleeping, doneBalling, reached }


public class NPC : Pickable, IController, IStateMachineController
{
    [SerializeField] HandSystem handSystem;
    [SerializeField] DetectorSystem detector;
    [SerializeField] GroundDetector groundDetector;


    [Header("Growing Parameters")]
    [SerializeField] LevelController levelController;
    [SerializeField] public float growTime = 5f;
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
    [Header("Timing Parameters")]
    [SerializeField] public float boredTime = 30f;
    [SerializeField] public float sleepTime = 10f;
    [SerializeField] public float layingTime = 10f;
    [SerializeField] public float eatTime = 10f;
    [SerializeField] public float deathTime = 50f;

    [Header("Probability Parameters")]
    [SerializeField] [Range(0, 1)] public float seekPlayerProb = 0.1f;
    [SerializeField] [Range(0, 1)] public float seekNpcProb = 0.1f;
    [SerializeField] [Range(0, 1)] public float seekBallProb = 0.1f;
    [SerializeField] [Range(0, 1)] public float seekTreeProb = 0.1f;
    [SerializeField] [Range(0, 1)] public float dropBallProb = 0.1f;
    [SerializeField] [Range(0, 1)] public float throwBallOnNpcProb = 0.1f;
    [SerializeField] [Range(0, 1)] public float throwBallOnPlayerProb = 0.1f;
    [SerializeField] [Range(0, 1)] public float punchNpcProb = 0.1f;
    [SerializeField] [Range(0, 1)] public float seekFruitProb = 1f;
    [SerializeField] [Range(0, 1)] public float seekAlterProb = 1f;


    [Header("References")]
    [SerializeField] AIStateMachine aiStateMachine;
    [SerializeField] GameObject model;
    [SerializeField] List<MeshRenderer> bodyRenderers;
    [SerializeField] Material grownMaterial;
    [SerializeField] GameObject eggAsset;
    [SerializeField] GameObject deadNpcAsset;


    //Private data
    List<GameObject> _wantToFollow = new List<GameObject>();
    NavMeshAgent myAgent;
    float bornSince = 0f;
    bool petting = false;
    bool canLay = false;

    //Helper functions
    private void Awake()
    {
        myAgent = GetComponent<NavMeshAgent>();
        myAgent.stoppingDistance = 0.9f * nearObjectDistance;
        detector.Initialize(nearObjectDistance);
        handSystem.Initialize(detector, this);
        groundDetector.Initialize();
        levelController.Initialize(OnLevelIncrease, OnXPIncrease);

        MovementStatus _state = MovementStatus.Idel;
        aiStateMachine.Initialize((Enum)_state);
        InitializeLevelUI();


        foreach(DetectableElement _element in detector.detectableElements)
        {
            _element.OnNear += OnDetectableNear;
            _element.OnInRange += OnDetectableInRange;
            _element.OnInRangeExit += OnDetectableExit;
        }

        base.StartCoroutine(GrowingUp());
        base.StartCoroutine(AiContinous());
        base.StartCoroutine(AiDescrete());
    }
    public void Update()
    {
        handSystem.Update();
        detector.Update();

        if (groundDetector.IsOnWater(myBody))
            Die();
    }
    bool GotTypeInHand(Type _type)
    {
        if (handSystem.GetObjectInHand() != null && handSystem.GetObjectInHand().GetType() == _type)
            return true;
        else
            return false;
    }


    //Interface
    public void LoadData(NPC_Data npc_data)
    {
        transform.position = npc_data.position.GetVector();
        transform.rotation = npc_data.rotation.GetQuaternion();
        bornSince = npc_data.bornSince;
        
        levelController.IncreaseXP(npc_data.xp);
        OnXPIncrease();
    }
    public NPC_Data GetData()
    {
        NPC_Data npc_data = new NPC_Data();

        npc_data.position = new nVector3(transform.position);
        npc_data.rotation = new nQuaternion(transform.rotation);
        npc_data.bornSince = bornSince;
        npc_data.xp = levelController.GetXp();

        return npc_data;
    }
    public override void Pick(HandSystem _picker)
    {
        base.Pick(_picker);
        myAgent.enabled = false;
        aiStateMachine.SetBool(BooleanStates.Picked, true); 
    }
    public override void Drop()
    {
        base.Drop();

        aiStateMachine.SetBool(BooleanStates.Picked, false);
    }
    public void StartPetting()
    {
        levelController.IncreaseXP(pettingXP);
        myBody.isKinematic = true;
        myBody.velocity = Vector3.zero;
        petting = true;
    }
    public void EndPetting()
    {
        myBody.isKinematic = false;
        petting = false;
    }

    //Growing Up
    IEnumerator GrowingUp()
    {
        while ((bornSince < growTime))
        {
            bornSince += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        canLay = true;
        GrowUp();

        while((bornSince < deathTime))
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
        var _deadNPC =  Instantiate(deadNpcAsset, this.transform.position, Quaternion.identity);
        UIController.instance.DestroyNpcUiElement(this.gameObject);
        UIController.instance.RepeatMessage("Death!!", _deadNPC.transform, 2f, 4f, new ConditionChecker(true));
        Destroy(this.gameObject);
    }


    //UI-Level Functions
    public void InitializeLevelUI()
    {
        UIController.instance.CreateNPCUi(this.gameObject, levelController.GetLevelLimits(), this.transform);
        UIController.instance.UpateNpcUiElement(this.gameObject, "Level " + levelController.GetLevel().ToString());
    }
    public void OnXPIncrease()
    {
        UIController.instance.UpateNpcUiElement(this.gameObject, levelController.GetXp());
    }
    public void OnLevelIncrease()
    {
        UIController.instance.UpateNpcUiElement(this.gameObject, levelController.GetLevelLimits());
        UIController.instance.UpateNpcUiElement(this.gameObject, "Level " + levelController.GetLevel().ToString());
        UIController.instance.RepeatMessage("Level Up", this.transform, 0.5f, 4, new ConditionChecker(true));
    }


    //AI private variables
    Vector3 deltaExploration;
    public Transform dynamicDestination;

    //AI Decision Making
    IEnumerator AiDescrete()
    {
        while (true)
        {
            if (_wantToFollow.Count >= 1)
            {
                GameObject _obj = detector.GetHighestProp(_wantToFollow);

                if (_obj != dynamicDestination)
                {
                    if (_obj.tag == "Tree" && _obj.GetComponent<TreeSystem>().GotFruit())
                        SetDes(_obj);
                    else if(_obj.tag != "Tree")
                        SetDes(_obj);
                }
            }

            if ((aiStateMachine.GetTimeSinceLastChange() >= boredTime))
                aiStateMachine.SetBool(BooleanStates.tired, true);
            else
                aiStateMachine.SetBool(BooleanStates.tired, false);

            if ((aiStateMachine.GetTimeSinceLastChange() >= (boredTime/4f)) && (IsCurrentState(MovementStatus.Idel)))
                aiStateMachine.SetBool(BooleanStates.bored, true);
            else
                aiStateMachine.SetBool(BooleanStates.bored, false);

            //i got a throwable object (ball).
            if (GotTypeInHand(typeof(Ball)))
            {
                if (detector.GetDetectable("Player").detectionStatus == DetectionStatus.InRange)
                {
                    ThinkAboutThrowing(((PlayerSystem)(detector.DetectableInRange("Player"))).gameObject, throwBallOnPlayerProb);
                }
                if (detector.GetDetectable("NPC").detectionStatus == DetectionStatus.InRange)
                {
                    ThinkAboutThrowing(((NPC)(detector.DetectableInRange("NPC"))).gameObject, throwBallOnNpcProb);
                }

                //No One is near
                ThinkaboutDroppingTheBall();
            }


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
    public void ActionExecution(Enum _action)
    {
        MovementStatus _state = (MovementStatus)_action;

        if (_state == MovementStatus.Explore)
        {
            ExplorePoint();
        }
        else if (_state == MovementStatus.Sleep)
        {
            StartCoroutine(Sleeping());
        }
        else if (_state == MovementStatus.Eat)
        {
            bool _willEat = false;

            if (handSystem.GetNearest())
            {
                if ((handSystem.GetNearest()).tag == "Fruit")
                {
                    handSystem.PickObject();

                    if ((Fruit)(handSystem.GetObjectInHand()))
                        _willEat = true;
                }
            }

            //so that if eating failed, it tells the _state machine.
            if (_willEat)
                StartCoroutine(Eating((Fruit)(handSystem.GetObjectInHand())));
            else
                aiStateMachine.SetTrigger(TriggerStates.doneEating);

        }
        else if (_state == MovementStatus.Lay)
        {
            StartCoroutine(Laying());
        }
        else if (_state == MovementStatus.Ball)
        {
            if ((handSystem.GetNearest()).tag == "Ball" && (handSystem.GetObjectInHand() == null))
                handSystem.PickObject();
        }
    }
    void OnDetectableInRange(IDetectable _detectable)
    {
        if (_detectable.tag == "Player")
            ThinkAboutFollowingObject(((PlayerSystem)_detectable).gameObject, seekPlayerProb);

        if (_detectable.tag == ("NPC"))
            ThinkAboutFollowingObject(((NPC)_detectable).gameObject, seekNpcProb);

        if (_detectable.tag == ("Ball"))
            ThinkAboutFollowingObject(((Ball)_detectable).gameObject, seekBallProb);

        if (_detectable.tag == ("Tree"))
            ThinkAboutFollowingObject(((TreeSystem)_detectable).gameObject, seekTreeProb);

        if (_detectable.tag == ("Fruit"))
            if (((Fruit)_detectable).GetComponent<Fruit>().OnGround())
                ThinkAboutFollowingObject(((Fruit)_detectable).gameObject, seekFruitProb);

        if (_detectable.tag == ("Alter"))
            if (canLay)
                ThinkAboutFollowingObject(((FertilityAlter)_detectable).gameObject, seekAlterProb);
    }
    void OnDetectableExit(IDetectable _detectable)
    {
        //If the object i am following got out of range
        if (dynamicDestination == ((MonoBehaviour)_detectable).gameObject.transform)
        {
            if (IsCurrentState(MovementStatus.Move))
                aiStateMachine.SetTrigger(TriggerStates.lostTarget);
        }

        if (_wantToFollow.Contains(_detectable.GetGameObject()))
            _wantToFollow.Remove(_detectable.GetGameObject());
    }
    void OnDetectableNear(IDetectable _detectable)
    {
        if (_detectable.tag == ("NPC"))
            ThinkAboutPunchingAnNpc(((NPC)(_detectable)).myBody);

        if (_detectable.tag == ("Tree"))
            ThinkAboutShakingTree((TreeSystem)_detectable);
    }


    //Algorithms
    IEnumerator Eating(Fruit _fruit)
    {
        float _time = 0;
        ConditionChecker _condition = new ConditionChecker(!isPicked);
        UIController.instance.RepeatMessage("Eating", this.transform, eatTime, 15, _condition);

        while (_condition.isTrue)
        {
            _time += Time.fixedDeltaTime;

            bool _timeCond = (_time <= eatTime);
            bool _fruitInHand = GotTypeInHand(typeof(Fruit));
            bool _hasEnergy = _fruit.HasEnergy();

            _condition.Update(IsCurrentState(MovementStatus.Eat) && _timeCond && _fruitInHand && _hasEnergy);

            levelController.IncreaseXP(_fruit.GetEnergy());

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        handSystem.DropObject();

        aiStateMachine.SetTrigger(TriggerStates.doneEating);
    }
    IEnumerator Sleeping()
    {
        float _time = 0;
        ConditionChecker _condition = new ConditionChecker(!isPicked);
        UIController.instance.RepeatMessage("Sleeping", this.transform, sleepTime, 15, _condition);

        //sleep
        myBody.isKinematic = true;
        myAgent.enabled = false;

        while (_condition.isTrue)
        {
            _time += Time.fixedDeltaTime;

            _condition.Update((_time <= sleepTime) && IsCurrentState(MovementStatus.Sleep));

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
        float _time = 0;
        ConditionChecker _condition = new ConditionChecker(!isPicked);
        UIController.instance.RepeatMessage("Laying", this.transform, layingTime, 15, _condition);

        //Laying
        myBody.isKinematic = true;
        myAgent.enabled = false;

        while (_condition.isTrue)
        {
            _time += Time.fixedDeltaTime;

            _condition.Update((_time <= layingTime) && IsCurrentState(MovementStatus.Lay));

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        //Done
        if (!isPicked)
        {
            myAgent.enabled = true;
            myBody.isKinematic = false;
        }


        if (_time >= layingTime)
        {
            canLay = false;

            Egg _egg = Instantiate(eggAsset.gameObject, this.transform.position + Vector3.up, Quaternion.identity).GetComponent<Egg>();
            _egg.SetRottenness(1f - levelController.GetLevelToLevelsRation());

            aiStateMachine.SetTrigger(TriggerStates.doneLaying);

            //Reset the ability to lay
            _time = 0;
            while (_time <= layingTimeInBetween)
            {
                _time += Time.fixedDeltaTime;

                yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
            }

            canLay = true;
        }
    }
    void ThinkAboutShakingTree(TreeSystem _tree)
    {
        float _randomChance = UnityEngine.Random.Range(0f, 1f);

        if ((_randomChance < seekTreeProb) && (_randomChance > 0))
        {
            if(_tree.GotFruit() && !IsCurrentState(MovementStatus.Sleep))
                _tree.Shake();
        }
    }
    void ThinkAboutPunchingAnNpc(Rigidbody _body)
    {
        float _randomChance = UnityEngine.Random.Range(0f, 1f);

        if ((_randomChance < punchNpcProb) && (_randomChance > 0))
        {
            Vector3 _direction = (_body.transform.position - this.transform.position).normalized;
            _body.AddForce(_direction * punchForce, ForceMode.Impulse);
        }
    }
    void ThinkAboutThrowing(GameObject _target, float _chance)
    {
        float _randomChance = UnityEngine.Random.Range(0f, 1f);

        if ((_randomChance < _chance) && (_randomChance > 0))
        {
            handSystem.ThrowObject((_target.transform.position));
        }
    }
    void ThinkaboutDroppingTheBall()
    {
        float _randomChance = UnityEngine.Random.Range(0f, 1f);

        if ((_randomChance < dropBallProb) && (_randomChance > 0))
        {
            handSystem.DropObject();
        }
    }
    void ThinkAboutFollowingObject(GameObject _obj, float _chance)
    {
        float _randomChance = UnityEngine.Random.Range(0f, 1f);

        if((_randomChance < _chance) && (_randomChance > 0))
        {
            if (!_wantToFollow.Contains(_obj))
                _wantToFollow.Add(_obj);
        }
    }
    void ExplorePoint()
    {
        deltaExploration = Vector3.zero;
        Vector3 _destination = MapSystem.instance.GetRandomExplorationPoint();

        float _distance = (this.transform.position - myAgent.destination).magnitude;

        if (_distance <= nearObjectDistance)
        {
            float _x = UnityEngine.Random.Range(0f, explorationAmplitude);
            float _z = UnityEngine.Random.Range(0f, explorationAmplitude);

            deltaExploration = new Vector3(_x, 0, _z);
        }

        if(myAgent.isActiveAndEnabled)
            myAgent.destination = _destination + deltaExploration;

    }
    void SetDes(GameObject _obj)
    {
        if(myAgent.isActiveAndEnabled)
        {
            dynamicDestination = _obj.transform;
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
    bool IsCurrentState(MovementStatus _state)
    {
        if (((MovementStatus)aiStateMachine.GetCurrentState()) == _state)
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


