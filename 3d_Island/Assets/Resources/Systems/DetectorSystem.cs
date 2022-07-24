using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BallDetectionStatus { None, InRange, VeryNear };
public enum TreeDetectionStatus { None, InRange, VeryNear };
public enum PlayerDetectionStatus { None, InRange, VeryNear };
public enum NpcDetectionStatus { None, InRange, VeryNear };
public enum EggDetectionStatus { None, InRange, VeryNear };


public delegate void notify(GameObject obj);
public delegate void notifyBall(Ball ball);
public delegate void notifyNPC(NPC npc);
public delegate void notifyTree(TreeSystem tree);

[System.Serializable]
public class DetectorSystem : MonoBehaviour
{

    public float _nearObjectDistance = 1f;

    public BallDetectionStatus _ballDetectionStatus = BallDetectionStatus.None;
    public TreeDetectionStatus _treeDetectionStatus = TreeDetectionStatus.None;
    public PlayerDetectionStatus _playerDetectionStatus = PlayerDetectionStatus.None;
    public NpcDetectionStatus _npcDetectionStatus = NpcDetectionStatus.None;
    public EggDetectionStatus _eggDetectionStatus = EggDetectionStatus.None;

    public event notify OnObjectEnter;
    public event notify OnObjectExit;


    public event notifyBall OnBallNear;
    public event notifyTree OnTreeNear;
    public event notifyNPC OnNpcNear;

    //Private data
    readonly DetectorData _detectorData = new();


    public void Initialize(float nearObjectDistance)
    {
        _nearObjectDistance = nearObjectDistance;
    }
    public void Update()
    {
        //Safty for destroyed Objects
        int destroyedIndex = -1;
        for(int i = 0; i < _detectorData.eggs.Count; i++)
        {
            if (_detectorData.eggs[i] == null)
                destroyedIndex = i;
        }
        if (destroyedIndex != -1)   
            _detectorData.eggs.RemoveAt(destroyedIndex);

        destroyedIndex = -1;
        for (int i = 0; i < _detectorData.npcs.Count; i++)
        {
            if (_detectorData.npcs[i] == null)
                destroyedIndex = i;
        }
        if (destroyedIndex != -1)
            _detectorData.npcs.RemoveAt(destroyedIndex);

        //Update status
        if (BallInRange(_nearObjectDistance))
        {
            //To invoke the event once.
            OnBallNear?.Invoke(BallInRange(_nearObjectDistance));
            _ballDetectionStatus = BallDetectionStatus.VeryNear;
        }
        else if (BallInRange())
        {

            _ballDetectionStatus = BallDetectionStatus.InRange;
        }
        else
        {
            _ballDetectionStatus = BallDetectionStatus.None;
        }

        if (TreeInRange(_nearObjectDistance))
        {
            OnTreeNear?.Invoke(TreeInRange(_nearObjectDistance));

            _treeDetectionStatus = TreeDetectionStatus.VeryNear;
        }
        else if (TreeInRange())
        {
            _treeDetectionStatus = TreeDetectionStatus.InRange;
        }
        else
        {
            _treeDetectionStatus = TreeDetectionStatus.None;
        }

        if (PlayerInRange(_nearObjectDistance))
        {
            _playerDetectionStatus = PlayerDetectionStatus.VeryNear;
        }
        else if (PlayerInRange())
        {
            _playerDetectionStatus = PlayerDetectionStatus.InRange;
        }
        else
        {
            _playerDetectionStatus = PlayerDetectionStatus.None;
        }

        if (NpcInRange(_nearObjectDistance))
        {
            if(_npcDetectionStatus != NpcDetectionStatus.VeryNear)
                OnNpcNear?.Invoke(NpcInRange(_nearObjectDistance));

            _npcDetectionStatus = NpcDetectionStatus.VeryNear;
        }
        else if (NpcInRange())
        {
            _npcDetectionStatus = NpcDetectionStatus.InRange;
        }
        else
        {
            _npcDetectionStatus = NpcDetectionStatus.None;
        }

        if (EggInRange(_nearObjectDistance))
        {
            _eggDetectionStatus = EggDetectionStatus.VeryNear;
        }
        else if (EggInRange())
        {
            _eggDetectionStatus = EggDetectionStatus.InRange;
        }
        else
        {
            _eggDetectionStatus = EggDetectionStatus.None;
        }
    }



    //Interfaces for outside use
    public DetectorData GetDetectedData()
    {
        return _detectorData;
    }

    public PlayerSystem PlayerInRange()
    {
        PlayerSystem player = null;

        if(_detectorData.players.Count == 1)
        {
            player = _detectorData.players[0];
        }
        else if(_detectorData.players.Count > 1)
        {
            player = _detectorData.players[0];

            foreach (PlayerSystem _player in _detectorData.players)
                if (Distance(_player.gameObject) < Distance(player.gameObject))
                    player = _player;
        }

        return player;
    }
    public PlayerSystem PlayerInRange(float _range)
    {
        PlayerSystem player = PlayerInRange();

        if (player != null && IsNear(player.gameObject, _range))
        {
            return player;
        }
        else
        {
            return null;
        }
    }

    public NPC NpcInRange()
    {
        NPC npc = null;

        if (_detectorData.npcs.Count == 1)
        {
            npc = _detectorData.npcs[0];
        }
        else if (_detectorData.npcs.Count > 1)
        {
            npc = _detectorData.npcs[0];

            foreach (NPC _npc in _detectorData.npcs)
                if (Distance(_npc.gameObject) < Distance(npc.gameObject))
                    npc = _npc;
        }

        return npc;
    }
    public NPC NpcInRange(float _range)
    {
        NPC npc = NpcInRange();

        if (npc != null && IsNear(npc.gameObject, _range))
        {
            return npc;
        }
        else
        {
            return null;
        }
    }

    public Ball BallInRange()
    {
        Ball ball = null;

        if (_detectorData.balls.Count == 1)
        {
            ball = _detectorData.balls[0];
        }
        else if (_detectorData.balls.Count > 1)
        {
            ball = _detectorData.balls[0];

            foreach (Ball _ball in _detectorData.balls)
                if (Distance(_ball.gameObject) < Distance(ball.gameObject))
                    ball = _ball;
        }

        return ball;
    }
    public Ball BallInRange(float _range)
    {
        Ball ball = BallInRange();

        if (ball != null && IsNear(ball.gameObject, _range))
        {
            return ball;
        }
        else
        {
            return null;
        }
    }

    public TreeSystem TreeInRange()
    {
        TreeSystem tree = null;

        if (_detectorData.trees.Count == 1)
        {
            tree = _detectorData.trees[0];
        }
        else if (_detectorData.trees.Count > 1)
        {
            tree = _detectorData.trees[0];

            foreach (TreeSystem _tree in _detectorData.trees)
                if (Distance(_tree.gameObject) < Distance(tree.gameObject))
                    tree = _tree;
        }

        return tree;
    }
    public TreeSystem TreeInRange(float _range)
    {
        TreeSystem tree = TreeInRange();

        if (tree != null && IsNear(tree.gameObject, _range))
        {
            return tree;
        }
        else
        {
            return null;
        }
    }


    public Egg EggInRange()
    {
        Egg egg = null;

        if (_detectorData.eggs.Count == 1)
        {
            egg = _detectorData.eggs[0];
        }
        else if (_detectorData.eggs.Count > 1)
        {
            egg = _detectorData.eggs[0];

            foreach (Egg _egg in _detectorData.eggs)
                if (Distance(_egg.gameObject) < Distance(egg.gameObject))
                    egg = _egg;
        }

        return egg;
    }
    public Egg EggInRange(float _range)
    {
        Egg egg = EggInRange();

        if (egg != null && IsNear(egg.gameObject, _range))
        {
            return egg;
        }
        else
        {
            return null;
        }
    }




    //Help functions
    public float Distance(GameObject _object)
    {
        return (_object.transform.position - this.transform.position).magnitude;
    }
    bool IsNear(GameObject _object, float _range)
    {
        return Distance(_object) <= _range;
    }


    //Detection functions
    private void OnTriggerEnter(Collider collider)
    {
        OnObjectEnter?.Invoke(collider.gameObject);

        if (collider.CompareTag("Player"))
        {
            PlayerSystem _player = collider.GetComponentInParent<PlayerSystem>();

            if (_detectorData.players.Contains(_player) == false)
                _detectorData.players.Add(_player);
        }
        else if (collider.CompareTag("NPC"))
        {
            NPC _npc = collider.GetComponentInParent<NPC>();

            if (_detectorData.npcs.Contains(_npc) == false)
                _detectorData.npcs.Add(_npc);
        }
        else if (collider.CompareTag("Ball"))
        {
            Ball _ball = collider.GetComponentInParent<Ball>();

            if (_detectorData.balls.Contains(_ball) == false)
                _detectorData.balls.Add(_ball);
        }
        else if (collider.CompareTag("Tree"))
        {
            TreeSystem _tree = collider.GetComponentInParent<TreeSystem>();

            if (_detectorData.trees.Contains(_tree) == false)
                _detectorData.trees.Add(_tree);
        }
        else if (collider.CompareTag("Egg"))
        {
            Egg _egg = collider.GetComponentInParent<Egg>();

            if (_detectorData.eggs.Contains(_egg) == false)
                _detectorData.eggs.Add(_egg);
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        OnObjectExit?.Invoke(collider.gameObject);

        if (collider.CompareTag("Player"))
        {
            PlayerSystem _player = collider.GetComponentInParent<PlayerSystem>();

            if (_detectorData.players.Contains(_player) == true)
                _detectorData.players.Remove(_player);
        }
        else if (collider.CompareTag("NPC"))
        {
            NPC _npc = collider.GetComponentInParent<NPC>();

            if (_detectorData.npcs.Contains(_npc) == true)
                _detectorData.npcs.Remove(_npc);
        }
        else if (collider.CompareTag("Ball"))
        {
            Ball _ball = collider.GetComponentInParent<Ball>();

            if (_detectorData.balls.Contains(_ball) == true)
                _detectorData.balls.Remove(_ball);
        }
        else if (collider.CompareTag("Tree"))
        {
            TreeSystem _tree = collider.GetComponentInParent<TreeSystem>();

            if (_detectorData.trees.Contains(_tree) == true)
                _detectorData.trees.Remove(_tree);
        }
        else if (collider.CompareTag("Egg"))
        {
            Egg _egg = collider.GetComponentInParent<Egg>();

            if (_detectorData.eggs.Contains(_egg) == true)
                _detectorData.eggs.Remove(_egg);
        }
    }
}



[System.Serializable]
public class  DetectorData
{
    [SerializeField] public List<PlayerSystem> players = new();
    [SerializeField] public List<NPC> npcs = new();
    [SerializeField] public List<Ball> balls = new();
    [SerializeField] public List<TreeSystem> trees = new();
    [SerializeField] public List<Egg> eggs = new();
}
