using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DetectorSystem : MonoBehaviour
{
    [SerializeField] float _interactabilityRange = 1f;

    //Private data
    public DetectorData _detectorData;


    //Interface for outside use
    public DetectorData GetDetectedData()
    {
        return _detectorData;
    }

    public PlayerSystem PlayerInRange(bool _onlyVeryNear)
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

        if(player != null && _onlyVeryNear && IsNear(player.gameObject))
        {
            return player;
        }
        else if(player != null && !_onlyVeryNear)
        {
            return player;
        }
        else 
        {
            return null;
        }
    }
    public NPC NpcInRange(bool _onlyVeryNear)
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

        if (npc != null && (_onlyVeryNear == true) && IsNear(npc.gameObject))
        {
            return npc;
        }
        else if (npc != null && (_onlyVeryNear == false))
        {
            return npc;
        }
        else
        {
            return null;
        }
    }
    public Ball BallInRange(bool _onlyVeryNear)
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

        if (ball != null && (_onlyVeryNear == true) && IsNear(ball.gameObject))
        {
            return ball;
        }
        else if (ball != null && (_onlyVeryNear == false))
        {
            return ball;
        }
        else
        {
            return null;
        }
    }
    public TreeSystem TreeInRange(bool _onlyVeryNear)
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

        if (tree != null && (_onlyVeryNear == true) && IsNear(tree.gameObject))
        {
            return tree;
        }
        else if (tree != null && (_onlyVeryNear == false))
        {
            return tree;
        }
        else
        {
            return null;
        }
    }


    //Help functions
    float Distance(GameObject _object)
    {
        return (_object.transform.position - this.transform.position).magnitude;
    }
    bool IsNear(GameObject _object)
    {
        return Distance(_object) <= _interactabilityRange;
    }


    //Detection functions
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag == "Player")
        {
            PlayerSystem _player = collider.GetComponentInParent<PlayerSystem>();

            if (_detectorData.players.Contains(_player) == false)
                _detectorData.players.Add(_player);
        }
        else if (collider.tag == "NPC")
        {
            NPC _npc = collider.GetComponentInParent<NPC>();

            if (_detectorData.npcs.Contains(_npc) == false)
                _detectorData.npcs.Add(_npc);
        }
        else if (collider.tag == "Ball")
        {
            Ball _ball = collider.GetComponentInParent<Ball>();

            if (_detectorData.balls.Contains(_ball) == false)
                _detectorData.balls.Add(_ball);
        }
        else if (collider.tag == "Tree")
        {
            TreeSystem _tree = collider.GetComponentInParent<TreeSystem>();

            if (_detectorData.trees.Contains(_tree) == false)
                _detectorData.trees.Add(_tree);
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            PlayerSystem _player = collider.GetComponentInParent<PlayerSystem>();

            if (_detectorData.players.Contains(_player) == true)
                _detectorData.players.Remove(_player);
        }
        else if (collider.tag == "NPC")
        {
            NPC _npc = collider.GetComponentInParent<NPC>();

            if (_detectorData.npcs.Contains(_npc) == true)
                _detectorData.npcs.Remove(_npc);
        }
        else if (collider.tag == "Ball")
        {
            Ball _ball = collider.GetComponentInParent<Ball>();

            if (_detectorData.balls.Contains(_ball) == true)
                _detectorData.balls.Remove(_ball);
        }
        else if (collider.tag == "Tree")
        {
            TreeSystem _tree = collider.GetComponentInParent<TreeSystem>();

            if (_detectorData.trees.Contains(_tree) == true)
                _detectorData.trees.Remove(_tree);
        }
    }
}



[System.Serializable]
public class  DetectorData
{
    [SerializeField] public List<PlayerSystem> players = new List<PlayerSystem>();
    [SerializeField] public List<NPC> npcs = new List<NPC>();
    [SerializeField] public List<Ball> balls = new List<Ball>();
    [SerializeField] public List<TreeSystem> trees = new List<TreeSystem>();
}
