using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class NPC : Pickable, IController, IStateMachineController
{
    [SerializeField] HandSystem _handSystem;
    [SerializeField] DetectorSystem _detector;
    [SerializeField] GroundDetector _groundDetector;


    [Header("Growing Parameters")]
    [SerializeField] LevelController _levelController;
    [SerializeField] public float growTime = 5f;
    [SerializeField] float _grownBodyMultiplier = 1.35f;
    [SerializeField] float _grownMassMultiplier = 1.35f;


    [Header("Character parameters")]
    [SerializeField] float _nearObjectDistance = 1f;
    [SerializeField] float _decisionsDelay = 0.5f;
    [SerializeField] float _punchForce = 120f;
    [SerializeField] float _explorationAmplitude = 10f;
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
    [SerializeField] GameObject _model;
    [SerializeField] List<MeshRenderer> _bodyRenderers;
    [SerializeField] Material _grownMaterial;
    [SerializeField] GameObject _eggAsset;
    [SerializeField] GameObject _deadNpcAsset;


    //Private data
    public List<GameObject> _wantToFollow = new List<GameObject>();
    NavMeshAgent _myAgent;
    float _bornSince = 0f;
    bool _petting = false;
    bool _canLay = false;

    //Helper functions
    private void Awake()
    {
        _myAgent = GetComponent<NavMeshAgent>();
        _myAgent.stoppingDistance = 0.9f * _nearObjectDistance;
        _detector.Initialize(_nearObjectDistance);
        _handSystem.Initialize(_detector, this);
        _groundDetector.Initialize();
        _levelController.Initialize(OnLevelIncrease, OnXPIncrease);
        InitializeLevelUI();


        foreach(DetectableElement element in _detector._detectableElements)
        {
            element._OnNear += OnDetectableNear;
            element._OnInRange += OnDetectableInRange;
            element._OnInRangeExit += OnDetectableExit;
        }

        base.StartCoroutine(GrowingUp());
        base.StartCoroutine(AiContinous());
        base.StartCoroutine(AiDescrete());
    }
    public void Update()
    {
        _handSystem.Update();
        _detector.Update();

        if (_groundDetector.IsOnWater(_myBody))
            Die();
    }
    bool GotTypeInHand(System.Type _type)
    {
        if (_handSystem.ObjectInHand() != null && _handSystem.ObjectInHand().GetType() == _type)
            return true;
        else
            return false;
    }


    //Interface
    public override void Pick(HandSystem _picker)
    {
        base.Pick(_picker);
        _myAgent.enabled = false;
        aiStateMachine.SetBool(BooleanStates.Picked, true); 
    }
    public override void Drop()
    {
        base.Drop();

        aiStateMachine.SetBool(BooleanStates.Picked, false);
    }
    public void StartCoroutine_Custom(IEnumerator routine)
    {
        base.StartCoroutine(routine);
    }
    public void StartPetting()
    {
        _levelController.IncreaseXP(pettingXP);
        _myBody.isKinematic = true;
        _myBody.velocity = Vector3.zero;
        _petting = true;
    }
    public void EndPetting()
    {
        _myBody.isKinematic = false;
        _petting = false;
    }


    //Growing Up
    IEnumerator GrowingUp()
    {
        while ((_bornSince < growTime))
        {
            _bornSince += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        _canLay = true;
        GrowUp();

        while((_bornSince < deathTime))
        {
            _bornSince += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        Die();

    }
    void GrowUp()
    {
        _myBody.mass = _grownMassMultiplier * _myBody.mass;
        _model.transform.localScale = _grownBodyMultiplier * _model.transform.localScale;

        foreach (MeshRenderer mesh in _bodyRenderers)
            mesh.material = _grownMaterial;
    }
    void Die()
    {
        var _deadNPC =  Instantiate(_deadNpcAsset, this.transform.position, Quaternion.identity);
        UIController.instance.DestroyNpcUiElement(this.gameObject);
        UIController.instance.RepeatMessage("Death!!", _deadNPC.transform, 2f, 4f, new ConditionChecker(true));
        Destroy(this.gameObject);
    }


    //UI-Level Functions
    public void InitializeLevelUI()
    {
        UIController.instance.CreateNPCUi(this.gameObject, _levelController.GetLevelLimits(), this.transform);
        UIController.instance.UpateNpcUiElement(this.gameObject, "Level " + _levelController.GetLevel().ToString());
    }
    public void OnXPIncrease()
    {
        UIController.instance.UpateNpcUiElement(this.gameObject, _levelController.GetXp());
    }
    public void OnLevelIncrease()
    {
        UIController.instance.UpateNpcUiElement(this.gameObject, _levelController.GetLevelLimits());
        UIController.instance.UpateNpcUiElement(this.gameObject, "Level " + _levelController.GetLevel().ToString());
        UIController.instance.RepeatMessage("Level Up", this.transform, 0.5f, 4, new ConditionChecker(true));
    }


    //AI private variables
    Vector3 _deltaExploration = Vector3.zero;
    public Transform _dynamicDestination;

    public bool debug = false;
    //AI Decision Making
    IEnumerator AiDescrete()
    {
        while (true)
        {
            if (_wantToFollow.Count >= 1)
            {
                GameObject _obj = _detector.GetHighestProp(_wantToFollow);

                if (_obj != _dynamicDestination)
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

            if ((aiStateMachine.GetTimeSinceLastChange() >= (boredTime/4f)) && (aiStateMachine.GetCurrentState() == MovementStatus.Idel))
                aiStateMachine.SetBool(BooleanStates.bored, true);
            else
                aiStateMachine.SetBool(BooleanStates.bored, false);

            //i got a throwable object (ball).
            if (GotTypeInHand(typeof(Ball)))
            {
                if (_detector.GetDetectable("Ball")._detectionStatus == DetectionStatus.InRange)
                {
                    ThinkAboutThrowing(((PlayerSystem)(_detector.DetectableInRange("Player"))).gameObject, throwBallOnPlayerProb);
                }
                if (_detector.GetDetectable("NPC")._detectionStatus == DetectionStatus.InRange)
                {
                    ThinkAboutThrowing(((PlayerSystem)(_detector.DetectableInRange("NPC"))).gameObject, throwBallOnNpcProb);
                }

                //No One is near
                ThinkaboutDroppingTheBall();
            }


            yield return new WaitForSecondsRealtime(_decisionsDelay);
        }
    }
    IEnumerator AiContinous()
    {
        while (true)
        {

            if ((aiStateMachine.GetCurrentState() == MovementStatus.Move))
            {
                if (_dynamicDestination && _myAgent.isActiveAndEnabled)
                    _myAgent.destination = _dynamicDestination.position;

                CheckReached();
            }
            else if((aiStateMachine.GetCurrentState() == MovementStatus.Explore))
            {
                CheckReached();
            }

            //Reactivate AI only if the npc were thrown and touched the ground and not being bet
            if (_groundDetector.IsOnGroud(_myBody) && !_petting)
                _myAgent.enabled = true;
            else if (_petting)
                _myAgent.enabled = false;


            //See if there's a near fruit that is clear to eat
            if ((_detector.GetDetectable("Fruit")._detectionStatus == DetectionStatus.VeryNear) && _handSystem.ObjectInHand() == null)
                aiStateMachine.SetBool(BooleanStates.fruitNear, true);
            else
                aiStateMachine.SetBool(BooleanStates.fruitNear, false);

            //See if there's a near ball that is clear to eat
            if ((_detector.GetDetectable("Ball")._detectionStatus == DetectionStatus.VeryNear) && _handSystem.ObjectInHand() == null)
                aiStateMachine.SetBool(BooleanStates.ballNear, true);
            else
                aiStateMachine.SetBool(BooleanStates.ballNear, false);

            //When the ball is dropped
            if( (aiStateMachine.GetCurrentState() == MovementStatus.Ball) && _handSystem.ObjectInHand() == null)
            {
                aiStateMachine.SetTrigger(TriggerStates.doneBalling);
            }


            //see if there's a near alter
            if ((_detector.GetDetectable("Alter")._detectionStatus == DetectionStatus.VeryNear) && _canLay)
                aiStateMachine.SetBool(BooleanStates.alterNear, true);
            else
                aiStateMachine.SetBool(BooleanStates.alterNear, false);

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }
    public void ActionExecution(MovementStatus mov)
    {
        if (mov == MovementStatus.Explore)
        {
            ExplorePoint();
        }
        else if (mov == MovementStatus.Sleep)
        {
            StartCoroutine(Sleeping());
        }
        else if (mov == MovementStatus.Eat)
        {
            bool _willEat = false;

            if (_handSystem.GetNearest())
            {
                if ((_handSystem.GetNearest()).tag == "Fruit")
                {
                    _handSystem.PickObject();

                    if ((Fruit)(_handSystem.ObjectInHand()))
                        _willEat = true;
                }
            }

            //so that if eating failed, it tells the state machine.
            if (_willEat)
                StartCoroutine(Eating((Fruit)(_handSystem.ObjectInHand())));
            else
                aiStateMachine.SetTrigger(TriggerStates.doneEating);

        }
        else if (mov == MovementStatus.Lay)
        {
            StartCoroutine(Laying());
        }
        else if (mov == MovementStatus.Ball)
        {
            if ((_handSystem.GetNearest()).tag == "Ball" && (_handSystem.ObjectInHand() == null))
                _handSystem.PickObject();
        }
    }
    void OnDetectableInRange(IDetectable detectable)
    {
        if (detectable.tag == "Player")
            ThinkAboutFollowingObject(((PlayerSystem)detectable).gameObject, seekPlayerProb);

        if (detectable.tag == ("NPC"))
            ThinkAboutFollowingObject(((NPC)detectable).gameObject, seekNpcProb);

        if (detectable.tag == ("Ball"))
            ThinkAboutFollowingObject(((Ball)detectable).gameObject, seekBallProb);

        if (detectable.tag == ("Tree"))
            ThinkAboutFollowingObject(((TreeSystem)detectable).gameObject, seekTreeProb);


        if (detectable.tag == ("Fruit"))
            if (((Fruit)detectable).GetComponent<Fruit>().OnGround())
                ThinkAboutFollowingObject(((Fruit)detectable).gameObject, seekFruitProb);

        if (detectable.tag == ("Alter"))
            if (_canLay)
                ThinkAboutFollowingObject(((FertilityAlter)detectable).gameObject, seekAlterProb);
    }
    void OnDetectableExit(IDetectable detectable)
    {
        //If the object i am following got out of range
        if (_dynamicDestination == ((MonoBehaviour)detectable).gameObject.transform)
        {
            if ((aiStateMachine.GetCurrentState() == MovementStatus.Move))
                aiStateMachine.SetTrigger(TriggerStates.lostTarget);
        }

        if (_wantToFollow.Contains(detectable.GetGameObject()))
            _wantToFollow.Remove(detectable.GetGameObject());
    }
    void OnDetectableNear(IDetectable detectable)
    {
        if (detectable.tag == ("NPC"))
            ThinkAboutPunchingAnNpc(((NPC)(detectable))._myBody);

        if (detectable.tag == ("Tree"))
            ThinkAboutShakingTree((TreeSystem)detectable);



    }


    //Algorithms
    IEnumerator Eating(Fruit _fruit)
    {
        float _time = 0;
        ConditionChecker condition = new ConditionChecker(!_isPicked);
        UIController.instance.RepeatMessage("Eating", this.transform, eatTime, 15, condition);

        while (condition.isTrue)
        {
            _time += Time.fixedDeltaTime;

            bool _timeCond = (_time <= eatTime);
            bool _fruitInHand = GotTypeInHand(typeof(Fruit));
            bool _hasEnergy = _fruit.HasEnergy();

            condition.Update(aiStateMachine.GetCurrentState() == MovementStatus.Eat && _timeCond && _fruitInHand && _hasEnergy);

            _levelController.IncreaseXP(_fruit.GetEnergy());

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        _handSystem.DropObject();

        aiStateMachine.SetTrigger(TriggerStates.doneEating);
    }
    IEnumerator Sleeping()
    {
        float _time = 0;
        ConditionChecker condition = new ConditionChecker(!_isPicked);
        UIController.instance.RepeatMessage("Sleeping", this.transform, sleepTime, 15, condition);

        //sleep
        _myBody.isKinematic = true;
        _myAgent.enabled = false;

        while (condition.isTrue)
        {
            _time += Time.fixedDeltaTime;

            condition.Update((_time <= sleepTime) && aiStateMachine.GetCurrentState() == MovementStatus.Sleep);

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        //Wake up
        aiStateMachine.SetTrigger(TriggerStates.doneSleeping);

        if (!_isPicked)
        {
            _myAgent.enabled = true;
            _myBody.isKinematic = false;
        }
    }
    IEnumerator Laying()
    {
        float _time = 0;
        ConditionChecker condition = new ConditionChecker(!_isPicked);
        UIController.instance.RepeatMessage("Laying", this.transform, layingTime, 15, condition);

        //Laying
        _myBody.isKinematic = true;
        _myAgent.enabled = false;

        while (condition.isTrue)
        {
            _time += Time.fixedDeltaTime;

            condition.Update((_time <= layingTime) &&  aiStateMachine.GetCurrentState() == MovementStatus.Lay);

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        //Done
        if (!_isPicked)
        {
            _myAgent.enabled = true;
            _myBody.isKinematic = false;
        }


        if (_time >= layingTime)
        {
            _canLay = false;

            Egg _egg = Instantiate(_eggAsset.gameObject, this.transform.position + Vector3.up, Quaternion.identity).GetComponent<Egg>();
            _egg.SetRottenness(1f - _levelController.GetLevelToLevelsRation());

            aiStateMachine.SetTrigger(TriggerStates.doneLaying);

            //Reset the ability to lay
            _time = 0;
            while (_time <= layingTimeInBetween)
            {
                _time += Time.fixedDeltaTime;

                yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
            }

            _canLay = true;
        }
    }
    void ThinkAboutShakingTree(TreeSystem tree)
    {
        float _randomChance = Random.Range(0f, 1f);

        if ((_randomChance < seekTreeProb) && (_randomChance > 0))
        {
            tree.Shake();
        }
    }
    void ThinkAboutPunchingAnNpc(Rigidbody body)
    {
        float _randomChance = Random.Range(0f, 1f);

        if ((_randomChance < punchNpcProb) && (_randomChance > 0))
        {
            Vector3 direction = (body.transform.position - this.transform.position).normalized;
            body.AddForce(direction * _punchForce, ForceMode.Impulse);
        }
    }
    void ThinkAboutThrowing(GameObject target, float chance)
    {
        float _randomChance = Random.Range(0f, 1f);

        if ((_randomChance < chance) && (_randomChance > 0))
        {
            _handSystem.ThrowObject((target.transform.position));
        }
    }
    void ThinkaboutDroppingTheBall()
    {
        float _randomChance = Random.Range(0f, 1f);

        if ((_randomChance < dropBallProb) && (_randomChance > 0))
        {
            _handSystem.DropObject();
        }
    }
    void ThinkAboutFollowingObject(GameObject obj, float chance)
    {
        float _randomChance = Random.Range(0f, 1f);

        if((_randomChance < chance) && (_randomChance > 0))
        {
            if (!_wantToFollow.Contains(obj))
                _wantToFollow.Add(obj);
        }
    }
    void ExplorePoint()
    {
        _deltaExploration = Vector3.zero;
        Vector3 _destination = MapSystem.instance.GetRandomExplorationPoint();

        float distance = (this.transform.position - _myAgent.destination).magnitude;

        if (distance <= _nearObjectDistance)
        {
            float x = Random.Range(0f, _explorationAmplitude);
            float z = Random.Range(0f, _explorationAmplitude);

            _deltaExploration = new Vector3(x, 0, z);
        }

        if(_myAgent.isActiveAndEnabled)
            _myAgent.destination = _destination + _deltaExploration;

    }
    void SetDes(GameObject obj)
    {
        if(_myAgent.isActiveAndEnabled)
        {
            _dynamicDestination = obj.transform;
            _myAgent.destination = _dynamicDestination.position;
            aiStateMachine.SetTrigger(TriggerStates.foundTarget);
        }
    }
    void CheckReached()
    {
        if(_myAgent.isActiveAndEnabled)
            if (_myAgent.remainingDistance <= _nearObjectDistance)
               aiStateMachine.SetTrigger(TriggerStates.reached);
    }

    private void OnDrawGizmos()
    {
        if(_myAgent)
            Gizmos.DrawSphere(_myAgent.destination, .2f);
    }

}


