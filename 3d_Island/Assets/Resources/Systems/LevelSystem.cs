using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void levelNotify();
public delegate void xpNotify();

[System.Serializable]
class LevelController
{
    [SerializeField] int _currentLevel = 0;
    [SerializeField] float _currentXp = 0;
    [SerializeField] List<float> _levelsXP = new List<float>();

    event levelNotify OnLevelChange;
    event xpNotify OnXpChange;


    public void Initialize(levelNotify _OnLevelChange, xpNotify _OnXpChange)
    {
        OnLevelChange += _OnLevelChange;
        OnXpChange += _OnXpChange;
    }

    public int GetLevel()
    {
        return _currentLevel;
    }
    public float GetLevelToLevelsRation()
    {
        return ((GetLevel() * 1f) / _levelsXP.Count);
    }

    public Vector2 GetLevelLimits()
    {
        Vector2 _limits = new Vector2();

        //Min
        _limits.x = _levelsXP[_currentLevel];

        //Max
        //Return Next level's XP if this is not the last level;
        
        int _lastLevelIndex = _levelsXP.Count - 1;
        
        if (_currentLevel == _lastLevelIndex)
            _limits.y = _levelsXP[_currentLevel];
        else
            _limits.y = _levelsXP[_currentLevel + 1];


        return _limits;
    }
    public float GetXp()
    {
        return _currentXp;
    }
    public void IncreaseXP(float _amount)
    {
        int _oldLevel = _currentLevel;

        _currentXp += _amount;
        OnXpChange?.Invoke();

        ComputeLevel();

        int _newLevel = _currentLevel;
        
        if (_oldLevel != _newLevel)
            OnLevelChange?.Invoke();
    }
    

    void ComputeLevel()
    {
        int _level = 0;

        for(int i = _levelsXP.Count - 1; i>= 0; i--)
        {
            if (_currentXp >= _levelsXP[i])
            {
                _level = i;
                break;
            }
        }

        _currentLevel = _level;
    }

}

