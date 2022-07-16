using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : Pickable
{
    [SerializeField] HandSystem _handSystem;
    [SerializeField] DetectorSystem _detector;
    [SerializeField] GroundDetector _groundDetector;

    [Header("Growing Parameters")]
    [SerializeField] float _growingUpTime = 120f;
    [SerializeField] float _grownBodyMultiplier = 1.35f;
    [SerializeField] float _grownMassMultiplier = 1.35f;

    [Header("AI Parameters")]
    [SerializeField] float _stoppingDistance = 1f;
    [SerializeField] float _decisionsDelay = 0.5f;
    [SerializeField] float _punchableDistance = 1.5f;
    [SerializeField] float _punchForce = 120f;

    [Header("References")]
    [SerializeField] GameObject _model;
    [SerializeField] List<MeshRenderer> _bodyRenderers;
    [SerializeField] Material _grownMaterial;


    public enum BallStatus {noBall, ballDetected, ballPicked, ballPickable};
    public enum NPCStatus {noNPC, NPCDetected};
    public enum MovementStatus { moving, idel };
    public MovementStatus _movementStats = MovementStatus.idel;
    public BallStatus _ballStatus = BallStatus.noBall;
    public NPCStatus _npcStatus = NPCStatus.NPCDetected;



    NavMeshAgent _myAgent;

    List<Pickable> _pickablesByMeDetected = new List<Pickable>();
    NPC _nearNPCs;
    Vector3 _destination = new Vector3();

    float _bornSince = 0f;
    bool _isBaby = true;


    public override void Awake()
    {
        base.Awake();

        _myAgent = GetComponent<NavMeshAgent>();
        _handSystem.Initialize(_pickablesByMeDetected);

        StartCoroutine(GrowingUp());
        StartCoroutine(AiInteractionDecision());
        StartCoroutine(AiMovementDecision());
    }
    public override void Pick(Transform handPosition)
    {
        base.Pick(handPosition);

        _myAgent.enabled = false;
    }
    public void Update()
    {
        _handSystem.Update();

        //Reactivate AI if the player were thrown.
        if (_groundDetector.IsOnGroud(_myBody.transform.position))
            _myAgent.enabled = true;

        if (_detector.TreeInRange(false))
        {
            Debug.Log(this.name + " Detected Ball In Range");
        }

        if (_detector.TreeInRange(true))
        {
            Debug.Log(this.name + " Detected Ball very near");

        }
    }


    IEnumerator GrowingUp()
    {
        while ((_bornSince < _growingUpTime))
        {
            _bornSince += Time.fixedDeltaTime;
            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }

        if ((_bornSince >= _growingUpTime))
            GrowUp();
    }
    void GrowUp()
    {
        _isBaby = false;

        _myBody.mass = _grownMassMultiplier * _myBody.mass;
        _model.transform.localScale = _grownBodyMultiplier * _model.transform.localScale;

        foreach (MeshRenderer mesh in _bodyRenderers)
            mesh.material = _grownMaterial;
    }


    //NPC AI Decision Makers
    IEnumerator AiInteractionDecision()
    {
        while(true)
        {
            if (_handSystem._canPick)
                _ballStatus = BallStatus.ballPickable;
            else if (!_handSystem._gotSomething &&  _pickablesByMeDetected.Count > 0)
                _ballStatus = BallStatus.ballDetected;

            if (_nearNPCs == null)
                _npcStatus = NPCStatus.noNPC;
            else
                _npcStatus = NPCStatus.NPCDetected;


            if (_ballStatus == BallStatus.ballPickable)
            {
                _handSystem.PickObject();
                _ballStatus = BallStatus.ballPicked;
            }
            else if ((_ballStatus == BallStatus.ballPicked) && (_npcStatus == NPCStatus.NPCDetected))
            {
                var _direction = (_nearNPCs.transform.position - this.transform.position).normalized;
                _handSystem.ThrowObject(_direction);
                _ballStatus = BallStatus.noBall;
            }

            yield return new WaitForSecondsRealtime(_decisionsDelay);
        }

    }
    IEnumerator AiMovementDecision()
    {
        while(true)
        {
            if (_movementStats == MovementStatus.idel)
            {
                ChooseRandomExplorationPoint();
            }
            else if ((_movementStats == MovementStatus.moving) && !(_ballStatus == BallStatus.ballPickable))
            {
                MoveTo(_destination);
            }

            yield return new WaitForSecondsRealtime(Time.fixedDeltaTime);
        }
    }


    void ChooseRandomExplorationPoint()
    {
        if(!(MapSystem.ExplorationPoints.Count == 0))
        {
            var _randomLocation = Random.Range(0, MapSystem.ExplorationPoints.Count);

            _destination = MapSystem.ExplorationPoints[_randomLocation].transform.position;
            _movementStats = MovementStatus.moving;
        }
    }
    void MoveTo(Vector3 Position)
    {
        _myAgent.destination = Position;

        float distance = (this.transform.position - Position).magnitude;

        if (distance <= _stoppingDistance)
        {
            _movementStats = MovementStatus.idel;
        }
    }

   

}

