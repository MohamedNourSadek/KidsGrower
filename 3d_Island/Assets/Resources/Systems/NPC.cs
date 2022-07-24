using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum MovementStatus {Moving, Idel, Exploring, Watching, Sleeping};

public class NPC : Pickable, IHandController
{
    [SerializeField] HandSystem _handSystem;
    [SerializeField] DetectorSystem _detector;
    [SerializeField] GroundDetector _groundDetector;

    [Header("Growing Parameters")]
    [SerializeField] public float growingUpTime = 5f;
    [SerializeField] float _grownBodyMultiplier = 1.35f;
    [SerializeField] float _grownMassMultiplier = 1.35f;

    [Header("AI Parameters")]
    [SerializeField] MovementStatus _movementStatus = MovementStatus.Idel;
    [SerializeField] float _stoppingDistance = 1f;
    [SerializeField] float _decisionsDelay = 0.5f;
    [SerializeField] float _punchableDistance = 1.5f;
    [SerializeField] float _punchForce = 120f;
    [SerializeField] float _nearObjectDistance = 1f;
    [SerializeField] float _explorationAmplitude = 10f;
    [SerializeField] public float _bordemTime = 30f;
    [SerializeField] public float _sleepTime = 10f;
    [SerializeField] [Range(0, 1)] public float _playerLove = 0.1f;
    [SerializeField] [Range(0, 1)] public float _npcLove = 0.1f;
    [SerializeField] [Range(0, 1)] public float _ballLove = 0.1f;
    [SerializeField] [Range(0, 1)] public float _treeLove = 0.1f;
    [SerializeField] [Range(0, 1)] public float _droppingBall = 0.1f;
    [SerializeField] [Range(0, 1)] public float _throwBallOnNPC = 0.1f;
    [SerializeField] [Range(0, 1)] public float _throwBallOnPlayer = 0.1f;
    [SerializeField] [Range(0, 1)] public float _punchNpcLove = 0.1f;



    [Header("References")]
    [SerializeField] GameObject _model;
    [SerializeField] List<MeshRenderer> _bodyRenderers;
    [SerializeField] Material _grownMaterial;


    //Private data
    NavMeshAgent _myAgent;
    float _bornSince = 0f;
    bool _petting = false;
    
    //Main Functions
    private void Awake()
    {
        _myAgent = GetComponent<NavMeshAgent>();
        _detector.Initialize(_nearObjectDistance);
        _handSystem.Initialize(_detector, this);
        _groundDetector.Initialize();

        _detector.OnObjectEnter += OnObjectEnter;
        _detector.OnObjectExit += OnObjectExit;
        _detector.OnBallNear += OnBallNear;
        _detector.OnTreeNear += OnTreeNear;
        _detector.OnNpcNear += OnNpcNear;
        base.StartCoroutine(GrowingUp());
        base.StartCoroutine(AiContinous());
        base.StartCoroutine(AiDescrete());
    }
    public override void Pick(Transform handPosition)
    {
        base.Pick(handPosition);
        _myAgent.enabled = false;
    }
    public void Update()
    {
        _handSystem.Update();
        _detector.Update();

        //Reactivate AI only if the npc were thrown and touched the ground and not being bet
        if (_groundDetector.IsOnGroud(_myBody) && !_petting)
            _myAgent.enabled = true;
        else if(_petting)
            _myAgent.enabled = false;

    }
    public void StartCoroutine_Custom(IEnumerator routine)
    {
        base.StartCoroutine(routine);
    }
    public void StartPetting()
    {
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
        while ((_bornSince < growingUpTime))
        {
            _bornSince += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        if ((_bornSince >= growingUpTime))
            GrowUp();
    }
    void GrowUp()
    {
        _myBody.mass = _grownMassMultiplier * _myBody.mass;
        _model.transform.localScale = _grownBodyMultiplier * _model.transform.localScale;

        foreach (MeshRenderer mesh in _bodyRenderers)
            mesh.material = _grownMaterial;
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
                if (_timeSinceLastAction >= _bordemTime)
                {
                    _movementStatus = MovementStatus.Sleeping;
                    StartCoroutine(Sleeping());
                }

                //Player is near and i got a throwable object.
                if (_detector._playerDetectionStatus == PlayerDetectionStatus.InRange && _handSystem._gotSomething)
                {
                    ThinkAboutThrowing(_detector.PlayerInRange().gameObject, _throwBallOnPlayer);
                }
                //NPC is near and i got a throwable object.
                if (_detector._npcDetectionStatus == NpcDetectionStatus.InRange && _handSystem._gotSomething)
                {
                    ThinkAboutThrowing(_detector.NpcInRange().gameObject, _throwBallOnNPC);
                }

                //No One is near
                if (_handSystem._gotSomething)
                {
                    ThinkaboutDroppingTheBall();
                }

            }

            yield return new WaitForSecondsRealtime(_decisionsDelay);
        }
    }
    void OnObjectEnter(GameObject obj)
    {
        if (obj.CompareTag("Player"))
            ThinkAboutFollowingObject(obj, _playerLove);

        if (obj.CompareTag("NPC"))
            ThinkAboutFollowingObject(obj, _npcLove);

        if (obj.CompareTag("Ball"))
            ThinkAboutFollowingObject(obj, _ballLove);

        if (obj.CompareTag("Tree"))
            ThinkAboutFollowingObject(obj, _treeLove);
    }
    void OnObjectExit(GameObject obj)
    {
        //If the object i am following got out of range
        if(_dynamicDestination == obj.transform)
        {
            _movementStatus = MovementStatus.Idel;
        }
    }
    void OnTreeNear(TreeSystem tree)
    {
        if(_movementStatus == MovementStatus.Watching)
            ThinkAboutShakingTree(tree);
    }
    void OnNpcNear(NPC npc)
    {
        if (_movementStatus == MovementStatus.Watching)
            ThinkAboutPunchingAnNpc(_detector.NpcInRange()._myBody);
    }
    void OnBallNear(Ball ball)
    {
        if (_movementStatus == MovementStatus.Watching)
            ThinkAboutPickingBall();
    }
    IEnumerator AiContinous()
    {
        while(true)
        {
            if ((_movementStatus == MovementStatus.Moving))
            {
                MoveTo(_dynamicDestination);
            }
            else if(_movementStatus == MovementStatus.Exploring)
            {
                ExplorePoint(_destination);
            }


            _timeSinceLastAction += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }

     
    //Algorithms
    IEnumerator Sleeping()
    {
        ConditionChecker condition = new ConditionChecker(!_isPicked);
        StartCoroutine(UpdateSleepCondition(condition));


        UIController.uIController.RepeatMessage("Sleeping", (this.transform.position + (1f * Vector3.up)), _sleepTime, 15, condition);

        Sleep();
        while (condition.isTrue)
        {
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
        WakeUp();
    }
    void Sleep()
    {
        _myBody.isKinematic = true;
        _myAgent.enabled = false;
    }
    IEnumerator UpdateSleepCondition(ConditionChecker condition)
    {
        bool isConditionTrue = true;
        float _time = 0;

        while (isConditionTrue)
        {
            condition.Update(true);

            isConditionTrue = !_isPicked && (_time <= _sleepTime);

            _time += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        condition.Update(false);
    }
    void WakeUp()
    {
        _movementStatus = MovementStatus.Idel;

        if (!_isPicked)
        {
            _myAgent.enabled = true;
            _myBody.isKinematic = false;
        }
    }
    void ThinkAboutShakingTree(TreeSystem tree)
    {
        float _randomChance = Random.Range(0f, 1f);

        if ((_randomChance < _treeLove) && (_randomChance > 0))
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
        float _randomChance = Random.Range(0f, 1f);

        if ((_randomChance < _punchNpcLove) && (_randomChance > 0))
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

        if ((_randomChance < _droppingBall) && (_randomChance > 0))
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
        if (!(MapSystem.ExplorationPoints.Count == 0))
        {
            _timeSinceLastAction = 0;
            _deltaExploration = Vector3.zero;
            var _randomLocation = Random.Range(0, MapSystem.ExplorationPoints.Count);
            _destination = MapSystem.ExplorationPoints[_randomLocation].transform.position;
            _movementStatus = MovementStatus.Exploring;
        }
    }
    void MoveTo(Transform followed)
    {
        if (!_isPicked)
        {
            _myAgent.destination = followed.position;

            float distance = (this.transform.position - _myAgent.destination).magnitude;

            if (distance <= _stoppingDistance)
            {
                _movementStatus = MovementStatus.Watching;
            }
        }
    }
    void ExplorePoint(Vector3 position)
    {
        _myAgent.destination = position + _deltaExploration;

        float distance = (this.transform.position - _myAgent.destination).magnitude;

        if (distance <= _stoppingDistance)
        {
            float x = Random.Range(0f, _explorationAmplitude);
            float z = Random.Range(0f, _explorationAmplitude);

            _deltaExploration = new Vector3(x, 0, z);
        }
    }
}




