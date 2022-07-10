using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Pickable
{
    [SerializeField] MovementSystem _movementSystem;
    [SerializeField] HandSystem _handSystem;

    [Header("Growing Parameters")]
    [SerializeField] float _growingUpTime = 120f;
    [SerializeField] float _grownBodyMultiplier = 1.35f;
    [SerializeField] float _grownMassMultiplier = 1.35f;

    [Header("AI Parameters")]
    [SerializeField] float _stoppingDistance = 1f;
    [SerializeField] List<string> _detectableTags;
    [SerializeField] float _decisionsDelay = 0.5f;
    [SerializeField] float _punchableDistance = 1.5f;
    [SerializeField] float _punchForce = 120f;

    [Header("References")]
    [SerializeField] GameObject _model;
    [SerializeField] GameObject _myHand;
    [SerializeField] List<MeshRenderer> _bodyRenderers;
    [SerializeField] Material _grownMaterial;


    public enum BallStatus {noBall, ballDetected, ballPicked, ballPickable};
    public enum NPCStatus {noNPC, NPCDetected};
    public enum MovementStatus { moving, idel };
    public MovementStatus _movementStats = MovementStatus.idel;
    public BallStatus _ballStatus = BallStatus.noBall;
    public NPCStatus _npcStatus = NPCStatus.NPCDetected;

    List<GameObject> _objectsDetected = new List<GameObject>();
    List<Pickable> _pickablesByMeDetected = new List<Pickable>();
    NPC _nearNPCs;
    Vector3 _destination = new Vector3();
    Transform _lookDirection;

    float _bornSince = 0f;
    bool _isBaby = true;



    public override void Awake()
    {
        base.Awake();

        _lookDirection = _myBody.transform;
        _movementSystem.Initialize(_myBody, _lookDirection);
        _handSystem.Initialize(_pickablesByMeDetected);
        StartCoroutine(GrowingUp());
        
        StartCoroutine(AiInteractionDecision());
        StartCoroutine(AiMovementDecision());
    }

    public void Update()
    {
        _movementSystem.Update();
        _handSystem.Update();
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

            Debug.Log("Choose to go to " + _randomLocation);

            _destination = MapSystem.ExplorationPoints[_randomLocation].transform.position;
            _movementStats = MovementStatus.moving;
        }
    }
    void MoveTo(Vector3 Position)
    {
        Vector3 _distance = (Position - this.transform.position);

        _lookDirection.forward = _distance.normalized;
        _movementSystem.PreformMove(new Vector2(0f,1f));

        if (_distance.magnitude <= _stoppingDistance)
        {
            _movementStats = MovementStatus.idel;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(this.transform.position, _destination);
        Gizmos.DrawSphere(_destination, 0.5f);
    }

    private void OnTriggerEnter(Collider collider)
    {
        //Catch only detectable tags
        bool _detectable = false;
        foreach(var _tag in _detectableTags)
            if (collider.tag == _tag)
                _detectable = true;

        if (_detectable && !_objectsDetected.Contains(collider.gameObject))
        {
            _objectsDetected.Add(collider.gameObject);

            if(collider.tag == "Ball")
                _pickablesByMeDetected.Add(collider.GetComponent<Pickable>());

            if (collider.tag == "NPC")
            {
                _nearNPCs = collider.GetComponent<NPC>();

            }
        }
    }



    private void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "NPC")
        {
            if ((collider.transform.position - this.transform.position).magnitude <= _punchableDistance)
            {

                float _probability = Random.RandomRange(0f, 1f);

                if (_probability > 0.2f)
                {
                    Vector3 _direction = new Vector3(Random.Range(0f, 1f), 0.1f , Random.Range(0f, 1f));

                    collider.GetComponent<NPC>()._myBody.AddForce((_direction) * _punchForce, ForceMode.Impulse);
                }


            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (_objectsDetected.Contains(collider.gameObject))
        {
            _objectsDetected.Remove(collider.gameObject);

            if (collider.tag == "Ball")
                _pickablesByMeDetected.Remove(collider.GetComponent<Pickable>());

            if (collider.tag == "NPC")
                _nearNPCs = null;
        }
    }
}

