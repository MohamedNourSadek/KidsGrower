using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum MovementStatus {Moving, Idel, Exploring, Watching, Sleeping, Eating, Laying};

public class NPC : Pickable, IHandController
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
    [SerializeField] MovementStatus _movementStatus = MovementStatus.Idel;
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

        //Reactivate AI only if the npc were thrown and touched the ground and not being bet
        if (_groundDetector.IsOnGroud(_myBody) && !_petting)
            _myAgent.enabled = true;
        else if (_petting)
            _myAgent.enabled = false;
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
        UIController.uIController.DestroyProgressBar(this.gameObject);
        UIController.uIController.RepeatMessage("Death!!", _deadNPC.transform, 2f, 4f, new ConditionChecker(true));
        Destroy(this.gameObject);
    }


    //UI-Level Functions
    public void InitializeLevelUI()
    {
        UIController.uIController.CreateProgressBar(this.gameObject, _levelController.GetLevelLimits(), this.transform);
    }
    public void OnXPIncrease()
    {
        UIController.uIController.UpdateProgressBar(this.gameObject, _levelController.GetXp());
    }
    public void OnLevelIncrease()
    {
        UIController.uIController.UpdateProgressBarLimits(this.gameObject, _levelController.GetLevelLimits());
        UIController.uIController.RepeatMessage("Level Up", this.transform, 0.5f, 4, new ConditionChecker(true));
    }


    //AI private variables
    public float _timeSinceLastAction = 0;
    Vector3 _deltaExploration = Vector3.zero;
    Vector3 _destination = new();
    public Transform _dynamicDestination;


    //AI Decision Making
    IEnumerator AiDescrete()
    {
        while (true)
        {
            if(!(_movementStatus == MovementStatus.Sleeping))
            {
                if (_movementStatus == MovementStatus.Idel)
                {
                    ChooseRandomExplorationPoint();
                }
                if (_timeSinceLastAction >= boredTime)
                {
                    _movementStatus = MovementStatus.Sleeping;
                    StartCoroutine(Sleeping());
                }

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
            }

            yield return new WaitForSecondsRealtime(_decisionsDelay);
        }
    }
    IEnumerator AiContinous()
    {
        while (true)
        {
            if ((_movementStatus == MovementStatus.Moving))
            {
                MoveTo(_dynamicDestination);
            }
            else if (_movementStatus == MovementStatus.Exploring)
            {
                ExplorePoint(_destination);
            }


            _timeSinceLastAction += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
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
            {
                Debug.Log("Alter in Range");

                ThinkAboutlayingAnEgg(((FertilityAlter)detectable));
            }
    }
    void OnDetectableExit(IDetectable detectable)
    {
        //If the object i am following got out of range
        if (_dynamicDestination == ((MonoBehaviour)detectable).gameObject.transform)
        {
            _movementStatus = MovementStatus.Idel;
        }
    }
    void OnDetectableNear(IDetectable detectable)
    {
        if (detectable.tag == "Player")
            ThinkAboutFollowingObject(((PlayerSystem)detectable).gameObject, seekPlayerProb);

        if (detectable.tag == ("NPC"))
            if (_movementStatus != MovementStatus.Sleeping)
                ThinkAboutPunchingAnNpc(((NPC)(detectable))._myBody);

        if (detectable.tag == ("Ball"))
            if (_movementStatus == MovementStatus.Watching)
                ThinkAboutPickingBall();

        if (detectable.tag == ("Tree"))
            ThinkAboutShakingTree((TreeSystem)detectable);

        if (detectable.tag == ("Fruit"))
            if (_movementStatus != MovementStatus.Eating && ((Fruit)detectable).OnGround())
            {
                _movementStatus = MovementStatus.Eating;

                _handSystem.PickObject();
                StartCoroutine(Eating(((Fruit)detectable)));
            }

        if (detectable.tag == ("Alter"))
            if (_canLay)
            {
                Debug.Log("Alter is near");

                _movementStatus = MovementStatus.Laying;
                StartCoroutine(Laying(((FertilityAlter)detectable)));
            }
    }


    //Algorithms
    IEnumerator Eating(Fruit _fruit)
    {
        float _time = 0;
        ConditionChecker condition = new ConditionChecker(!_isPicked);
        UIController.uIController.RepeatMessage("Eating", this.transform, eatTime, 15, condition);

        while (condition.isTrue)
        {
            _time += Time.fixedDeltaTime;

            condition.Update(!_isPicked && (_time <= eatTime) && GotTypeInHand(typeof(Fruit)) && _fruit.HasEnergy());

            _levelController.IncreaseXP(_fruit.GetEnergy());

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        _handSystem.DropObject();

        _movementStatus = MovementStatus.Idel;
    }
    IEnumerator Sleeping()
    {
        float _time = 0;
        ConditionChecker condition = new ConditionChecker(!_isPicked);
        UIController.uIController.RepeatMessage("Sleeping", this.transform, sleepTime, 15, condition);

        //sleep
        _myBody.isKinematic = true;
        _myAgent.enabled = false;

        while (condition.isTrue)
        {
            _time += Time.fixedDeltaTime;

            condition.Update(!_isPicked && (_time <= sleepTime));
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        //Wake up
        _movementStatus = MovementStatus.Idel;
        if (!_isPicked)
        {
            _myAgent.enabled = true;
            _myBody.isKinematic = false;
        }
    }
    IEnumerator Laying(FertilityAlter alter)
    {
        float _time = 0;
        ConditionChecker condition = new ConditionChecker(!_isPicked);
        UIController.uIController.RepeatMessage("Laying", this.transform, layingTime, 15, condition);

        //Laying
        _myBody.isKinematic = true;
        _myAgent.enabled = false;

        while (condition.isTrue)
        {
            _time += Time.fixedDeltaTime;

            condition.Update(!_isPicked && (_time <= layingTime));

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        //Done
        if (!_isPicked)
        {
            _myAgent.enabled = true;
            _myBody.isKinematic = false;
        }
        _movementStatus = MovementStatus.Idel;

        if (_time >= layingTime)
        {
            _canLay = false;

            Egg _egg = Instantiate(_eggAsset.gameObject, alter.transform.position + Vector3.up, Quaternion.identity).GetComponent<Egg>();
            _egg.SetRottenness(1f - _levelController.GetLevelToLevelsRation());

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
    void ThinkAboutlayingAnEgg(FertilityAlter alter)
    {
        float _randomChance = Random.Range(0f, 1f);

        if ((_randomChance < seekAlterProb) && (_randomChance > 0))
        {
            ThinkAboutFollowingObject(alter.gameObject, 1f);
        }
    }
    void ThinkAboutShakingTree(TreeSystem tree)
    {
        float _randomChance = Random.Range(0f, 1f);

        if ((_randomChance < seekTreeProb) && (_randomChance > 0))
        {
            tree.Shake();
            _movementStatus = MovementStatus.Idel;
        }
    }
    void ThinkAboutPickingBall()
    {
        if (_dynamicDestination.CompareTag("Ball"))
        {
            _handSystem.PickObject();
            _movementStatus = MovementStatus.Idel;
        }
    }
    void ThinkAboutPunchingAnNpc(Rigidbody body)
    {
        Debug.Log("Thinking about punching NPC");
        
        float _randomChance = Random.Range(0f, 1f);

        if ((_randomChance < punchNpcProb) && (_randomChance > 0))
        {
            Vector3 direction = (body.transform.position - this.transform.position).normalized;
            body.AddForce(direction * _punchForce, ForceMode.Impulse);

            _movementStatus = MovementStatus.Idel;
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
            _timeSinceLastAction = 0;
            _dynamicDestination = obj.transform;
            _movementStatus = MovementStatus.Moving;
        }
    }
    void ChooseRandomExplorationPoint()
    {
        _timeSinceLastAction = 0;
        _deltaExploration = Vector3.zero;
        _destination = MapSystem.GetRandomExplorationPoint();
        _movementStatus = MovementStatus.Exploring;
    }
    void MoveTo(Transform followed)
    {
        if (!_isPicked)
        {
            _myAgent.destination = followed.position;

            float distance = (this.transform.position - _myAgent.destination).magnitude;

            if (distance <= _nearObjectDistance)
            {
                _movementStatus = MovementStatus.Watching;
            }
        }
    }
    void ExplorePoint(Vector3 position)
    {
        _myAgent.destination = position + _deltaExploration;

        float distance = (this.transform.position - _myAgent.destination).magnitude;

        if (distance <= _nearObjectDistance)
        {
            float x = Random.Range(0f, _explorationAmplitude);
            float z = Random.Range(0f, _explorationAmplitude);

            _deltaExploration = new Vector3(x, 0, z);
        }
    }
}


