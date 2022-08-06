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
    [SerializeField] public float boredTime = 30f;
    [SerializeField] public float sleepTime = 10f;
    [SerializeField] public float layingTime = 10f;
    [SerializeField] public float eatTime = 10f;
    [SerializeField] public float deathTime = 50f;
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
    NavMeshAgent _myAgent;
    float _bornSince = 0f;
    bool _petting = false;
    bool _canLay = true;
    List<MovementStatus> actionsQueue = new();

    //Helper functions
    private void Awake()
    {
        _myAgent = GetComponent<NavMeshAgent>();
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
        aiStateMachine.ActionRequest(MovementStatus.Picked);
    }
    public override void Drop()
    {
        base.Drop();

        aiStateMachine.ActionRequest(MovementStatus.Idel);
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
    Vector3 _destination = new();
    public Transform _dynamicDestination;


    //AI Decision Making
    IEnumerator AiDescrete()
    {
        while (true)
        {
            if (actionsQueue.Count > 0)
                SendActionQueue();

            if ((aiStateMachine.GetTimeSinceLastChange() >= boredTime) && !(aiStateMachine.GetCurrentState() == MovementStatus.Sleep))
                ActionRequest(MovementStatus.Sleep);


            if ((aiStateMachine.GetCurrentState()) == MovementStatus.Idel && !_isPicked)
                ActionRequest(MovementStatus.Explore);

            //i got a throwable object (ball).
            if(GotTypeInHand(typeof(Ball)))
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
            CheckReached();

            if ((aiStateMachine.GetCurrentState() == MovementStatus.Move))
            {
                if(_dynamicDestination && _myAgent.isActiveAndEnabled)
                    MoveTo(_dynamicDestination);
            }

            //Reactivate AI only if the npc were thrown and touched the ground and not being bet
            if (_groundDetector.IsOnGroud(_myBody) && !_petting)
                _myAgent.enabled = true;
            else if (_petting)
                _myAgent.enabled = false;

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }
    public void ActionRequest(MovementStatus mov)
    {
        //only add it if it's not the last one added to the list
        if ((actionsQueue.Count == 0) || (actionsQueue[actionsQueue.Count - 1] != mov))
        {
            actionsQueue.Add(mov);
        }
    }
    public void SendActionQueue()
    {
        List<MovementStatus> actions = new List<MovementStatus>();

        foreach(var action in actionsQueue)
        {
            actions.Add(action);
        }

        actionsQueue.Clear();

        foreach(var action in actions)
            aiStateMachine.ActionRequest(action);
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
            if (_handSystem.ObjectInHand())
            {
                if ((_handSystem.ObjectInHand()).tag == "Fruit")
                {
                    _handSystem.PickObject();

                    if ((Fruit)(_handSystem.ObjectInHand()))
                        StartCoroutine(Eating((Fruit)(_handSystem.ObjectInHand())));
                }
            }

        }
        else if (mov == MovementStatus.Lay)
        {
            StartCoroutine(Laying());
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
            ActionRequest(MovementStatus.Idel);
        }
    }
    void OnDetectableNear(IDetectable detectable)
    {
        if (detectable.tag == ("NPC"))
            ThinkAboutPunchingAnNpc(((NPC)(detectable))._myBody);

        if (detectable.tag == ("Ball"))
            ThinkAboutPickingBall();

        if (detectable.tag == ("Tree"))
            ThinkAboutShakingTree((TreeSystem)detectable);

        if (detectable.tag == ("Fruit") && _handSystem.ObjectInHand() == null)
            if (((Fruit)detectable).OnGround())
                ActionRequest(MovementStatus.Eat);

        if (detectable.tag == ("Alter"))
            if (_canLay)
                ActionRequest(MovementStatus.Lay);
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

            condition.Update(!_isPicked && _timeCond && _fruitInHand && _hasEnergy);

            _levelController.IncreaseXP(_fruit.GetEnergy());

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        _handSystem.DropObject();

        ActionRequest(MovementStatus.Idel);
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

            condition.Update(!_isPicked &&
                (_time <= sleepTime) &&
                aiStateMachine.GetCurrentState() == MovementStatus.Sleep);

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        //Wake up
        ActionRequest(MovementStatus.Idel);

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

            condition.Update(!_isPicked && (_time <= layingTime) &&
                aiStateMachine.GetCurrentState() == MovementStatus.Lay);

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

            ActionRequest(MovementStatus.Idel);

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
            ActionRequest(MovementStatus.Idel);
        }
    }
    void ThinkAboutPickingBall()
    {
        if (_dynamicDestination.CompareTag("Ball"))
        {
            _handSystem.PickObject();
            ActionRequest(MovementStatus.Idel);
        }
    }
    void ThinkAboutPunchingAnNpc(Rigidbody body)
    {
        float _randomChance = Random.Range(0f, 1f);

        if ((_randomChance < punchNpcProb) && (_randomChance > 0))
        {
            Vector3 direction = (body.transform.position - this.transform.position).normalized;
            body.AddForce(direction * _punchForce, ForceMode.Impulse);

            ActionRequest(MovementStatus.Idel);
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
            _dynamicDestination = obj.transform;

            ActionRequest(MovementStatus.Move);
        }
    }
    void MoveTo(Transform followed)
    {
        if (!_isPicked)
        {
            _myAgent.destination = followed.position;
        }
    }
    void ExplorePoint()
    {
        _deltaExploration = Vector3.zero;
        _destination = MapSystem.instance.GetRandomExplorationPoint();

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
    void CheckReached()
    {
        float distance = (this.transform.position - _myAgent.destination).magnitude;

        if (distance <= _nearObjectDistance)
        {
            ActionRequest(MovementStatus.Idel);
        }
    }


}


