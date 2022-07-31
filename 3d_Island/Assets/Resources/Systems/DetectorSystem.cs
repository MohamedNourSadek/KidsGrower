using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum DetectionStatus { None, InRange, VeryNear };

public delegate void notify(GameObject obj);
public delegate void notifyBall(Ball ball);
public delegate void notifyNPC(NPC npc);
public delegate void notifyTree(TreeSystem tree);
public delegate void notifyFruit(Fruit fruit);
public delegate void notifyAlter(FertilityAlter alter);

[System.Serializable]
public class DetectorSystem : MonoBehaviour
{

    [SerializeField] List<PickableTags> _whoCanIPick;
    [SerializeField] bool _highLightPickable;


    public float _nearObjectDistance = 1f;

    public DetectionStatus _ballDetectionStatus = DetectionStatus.None;
    public DetectionStatus _treeDetectionStatus = DetectionStatus.None;
    public DetectionStatus _playerDetectionStatus = DetectionStatus.None;
    public DetectionStatus _npcDetectionStatus = DetectionStatus.None;
    public DetectionStatus _eggDetectionStatus = DetectionStatus.None;
    public DetectionStatus _fruitDetectionStatus = DetectionStatus.None;
    public DetectionStatus _alterDetectionStatus = DetectionStatus.None;

    public event notify OnObjectEnter;
    public event notify OnObjectExit;


    public event notifyBall OnBallNear;
    public event notifyTree OnTreeNear;
    public event notifyNPC OnNpcNear;
    public event notifyFruit OnFruitNear;
    public event notifyAlter OnAlterNear;

    //Private data
    public DetectorData _detectorData = new();
    public List<Pickable> _toPick = new();


    public void Initialize(float nearObjectDistance)
    {
        _nearObjectDistance = nearObjectDistance;
    }

    public void Update()
    {
        CleanAllListsFromDestroyed();

        UpdateStates();
        UpdatePickables();
    }
    
    public void CleanAllListsFromDestroyed()
    {
        CleanListsFromDestroyedObjects(_toPick);
        CleanListsFromDestroyedObjects(_detectorData.eggs);
        CleanListsFromDestroyedObjects(_detectorData.fruits);
        CleanListsFromDestroyedObjects(_detectorData.npcs);
    }
    public void UpdatePickables()
    {
        //Detecting near objects
        foreach(PickableTags pickableTag in _whoCanIPick)
        {
            if (pickableTag == PickableTags.Ball)
            {
                Ball _ball = null;

                if (_ballDetectionStatus == DetectionStatus.VeryNear)
                    _ball = BallInRange(_nearObjectDistance);

                foreach (Ball ball in GetDetectedData().balls)
                {
                    if (ball == _ball)
                    {
                        if (!_toPick.Contains(ball))
                        {
                            if (_highLightPickable)
                                ball.PickablilityIndicator(true);

                            _toPick.Add(ball);
                        }
                    }
                    else
                    {
                        if (_toPick.Contains(ball))
                        {
                            if (_highLightPickable)
                                ball.PickablilityIndicator(false);

                            _toPick.Remove(ball);
                        }
                    }
                }
            }
            if (pickableTag == PickableTags.NPC)
            {
                NPC _npc = null;

                if (_npcDetectionStatus == DetectionStatus.VeryNear)
                    _npc = NpcInRange(_nearObjectDistance);

                foreach (NPC npc in GetDetectedData().npcs)
                {
                    if (npc == _npc)
                    {
                        if (!_toPick.Contains(npc))
                        {
                            if (_highLightPickable)
                                npc.PickablilityIndicator(true);

                            _toPick.Add(npc);
                        }

                    }
                    else
                    {
                        if (_toPick.Contains(npc))
                        {
                            if (_highLightPickable)
                                npc.PickablilityIndicator(false);

                            _toPick.Remove(npc);
                        }
                    }
                }
            }
            if (pickableTag == PickableTags.Egg)
            {
                Egg _egg = null;

                if (_eggDetectionStatus == DetectionStatus.VeryNear)
                    _egg = EggInRange(_nearObjectDistance);

                foreach (Egg egg in GetDetectedData().eggs)
                {
                    if (egg == _egg)
                    {
                        if (!_toPick.Contains(egg))
                        {
                            if (_highLightPickable)
                                egg.PickablilityIndicator(true);

                            _toPick.Add(egg);
                        }

                    }
                    else
                    {
                        if (_toPick.Contains(egg))
                        {
                            if (_highLightPickable)
                                egg.PickablilityIndicator(false);

                            _toPick.Remove(egg);
                        }
                    }
                }
            }
            if (pickableTag == PickableTags.Fruit)
            {
                Fruit _fruit = null;

                if (_fruitDetectionStatus == DetectionStatus.VeryNear)
                    _fruit = FruitInRange(_nearObjectDistance);

                foreach (Fruit fruit in GetDetectedData().fruits)
                {
                    if (fruit == _fruit)
                    {
                        if (!_toPick.Contains(fruit))
                        {
                            if (_highLightPickable)
                                fruit.PickablilityIndicator(true);

                            _toPick.Add(fruit);
                        }
                    }
                    else
                    {
                        if (_toPick.Contains(fruit))
                        {
                            if (_highLightPickable)
                                fruit.PickablilityIndicator(false);

                            _toPick.Remove(fruit);
                        }
                    }
                }
            }
        }

        
        if (_toPick.Count > 1)
        {
            //Safty for destroyed Objects
            List<Pickable> _newList = new();
            foreach (Pickable pickable in _toPick)
                if (pickable != null)
                    _newList.Add(pickable);

            var temp = _newList[0];

            foreach (Pickable pickable in _newList)
                if (Distance(pickable.gameObject) < Distance(temp.gameObject))
                    temp = pickable;

            foreach (Pickable pickable in _newList)
                if (_highLightPickable)
                    pickable.PickablilityIndicator(false);

            if (_highLightPickable)
                temp.PickablilityIndicator(true);

            _newList.Clear();
            _newList.Add(temp);
            _toPick = _newList;
        }
    }
    public void UpdateStates()
    {
        //Update status
        if (BallInRange(_nearObjectDistance))
        {
            //To invoke the event once.
            OnBallNear?.Invoke(BallInRange(_nearObjectDistance));
            _ballDetectionStatus = DetectionStatus.VeryNear;
        }
        else if (BallInRange())
        {

            _ballDetectionStatus = DetectionStatus.InRange;
        }
        else
        {
            _ballDetectionStatus = DetectionStatus.None;
        }

        if (TreeInRange(_nearObjectDistance))
        {
            if (_treeDetectionStatus != DetectionStatus.VeryNear)
                OnTreeNear?.Invoke(TreeInRange(_nearObjectDistance));

            _treeDetectionStatus = DetectionStatus.VeryNear;
        }
        else if (TreeInRange())
        {
            _treeDetectionStatus = DetectionStatus.InRange;
        }
        else
        {
            _treeDetectionStatus = DetectionStatus.None;
        }

        if (PlayerInRange(_nearObjectDistance))
        {
            _playerDetectionStatus = DetectionStatus.VeryNear;
        }
        else if (PlayerInRange())
        {
            _playerDetectionStatus = DetectionStatus.InRange;
        }
        else
        {
            _playerDetectionStatus = DetectionStatus.None;
        }

        if (NpcInRange(_nearObjectDistance))
        {
            if (_npcDetectionStatus != DetectionStatus.VeryNear)
                OnNpcNear?.Invoke(NpcInRange(_nearObjectDistance));

            _npcDetectionStatus = DetectionStatus.VeryNear;
        }
        else if (NpcInRange())
        {
            _npcDetectionStatus = DetectionStatus.InRange;
        }
        else
        {
            _npcDetectionStatus = DetectionStatus.None;
        }

        if (EggInRange(_nearObjectDistance))
        {
            _eggDetectionStatus = DetectionStatus.VeryNear;
        }
        else if (EggInRange())
        {
            _eggDetectionStatus = DetectionStatus.InRange;
        }
        else
        {
            _eggDetectionStatus = DetectionStatus.None;
        }

        if (FruitInRange(_nearObjectDistance))
        {
            if (FruitInRange(_nearObjectDistance))
            {
                try
                {
                    var fruit = FruitInRange(_nearObjectDistance);
                    OnFruitNear?.Invoke(fruit);
                }
                catch
                {
                }

            }

            _fruitDetectionStatus = DetectionStatus.VeryNear;
        }
        else if (FruitInRange())
        {
            _fruitDetectionStatus = DetectionStatus.InRange;
        }
        else
        {
            _fruitDetectionStatus = DetectionStatus.None;
        }


        if (AlterInRange(_nearObjectDistance))
        {
            if (_alterDetectionStatus != DetectionStatus.VeryNear)
                OnAlterNear?.Invoke(AlterInRange(_nearObjectDistance));

            _alterDetectionStatus = DetectionStatus.VeryNear;
        }
        else if (AlterInRange())
        {
            _alterDetectionStatus = DetectionStatus.InRange;
        }
        else
        {
            _alterDetectionStatus = DetectionStatus.None;
        }
    }


    //Interfaces for outside use
    public DetectorData GetDetectedData()
    {
        return _detectorData;
    }
    public List<Pickable> GetPickables()
    {
        return _toPick;
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


    public Fruit FruitInRange()
    {
        Fruit fruit = null;

        if (_detectorData.fruits.Count == 1)
        {
            fruit = _detectorData.fruits[0];
        }
        else if (_detectorData.fruits.Count > 1)
        {
            fruit = _detectorData.fruits[0];

            foreach (Fruit _fruit in _detectorData.fruits)
                if (Distance(_fruit.gameObject) < Distance(fruit.gameObject))
                    fruit = _fruit;
        }

        return fruit;
    }
    public Fruit FruitInRange(float _range)
    {
        Fruit fruit = FruitInRange();

        if (fruit != null && IsNear(fruit.gameObject, _range))
        {
            return fruit;
        }
        else
        {
            return null;
        }
    }


    public FertilityAlter AlterInRange()
    {
        FertilityAlter alter = null;

        if (_detectorData.alters.Count == 1)
        {
            alter = _detectorData.alters[0];
        }
        else if (_detectorData.alters.Count > 1)
        {
            alter = _detectorData.alters[0];

            foreach (FertilityAlter _alter in _detectorData.alters)
                if (Distance(_alter.gameObject) < Distance(alter.gameObject))
                    alter = _alter;
        }

        return alter;
    }
    public FertilityAlter AlterInRange(float _range)
    {
        FertilityAlter alter = AlterInRange();

        if (alter != null && IsNear(alter.gameObject, _range))
        {
            return alter;
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
    bool IsNear(GameObject _object, float _range)
    {
        return Distance(_object) <= _range;
    }
    bool IsPickable(PickableTags pickableType)
    {
        foreach (PickableTags _pickableType in _whoCanIPick)
        {
            if (_pickableType == pickableType)
                return true;
        }

        return false;
    }
    void CleanListsFromDestroyedObjects(IList list)
    {
        int destroyedIndex = -1;
        for (int i = 0; i < list.Count; i++)
        {
            if (((MonoBehaviour)list[i]) == null)
                destroyedIndex = i;
        }
        if (destroyedIndex != -1)
            list.RemoveAt(destroyedIndex);
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
        else if (collider.CompareTag("Fruit"))
        {
            Fruit _fruit = collider.GetComponentInParent<Fruit>();

            if (_detectorData.fruits.Contains(_fruit) == false)
                _detectorData.fruits.Add(_fruit);
        }
        else if (collider.CompareTag("Alter"))
        {
            FertilityAlter _alter = collider.GetComponentInParent<FertilityAlter>();

            if (_detectorData.alters.Contains(_alter) == false)
                _detectorData.alters.Add(_alter);
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
        else if (collider.CompareTag("Fruit"))
        {
            Fruit _fruit = collider.GetComponentInParent<Fruit>();

            if (_detectorData.fruits.Contains(_fruit) == true)
                _detectorData.fruits.Remove(_fruit);
        }
        else if (collider.CompareTag("Alter"))
        {
            FertilityAlter _alter = collider.GetComponentInParent<FertilityAlter>();

            if (_detectorData.alters.Contains(_alter) == true)
                _detectorData.alters.Remove(_alter);
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
    [SerializeField] public List<Fruit> fruits = new();
    [SerializeField] public List<FertilityAlter> alters = new();

}


