using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void levelNotify();
public delegate void xpNotify();

[System.Serializable]
public class LevelController
{
    public int currentLevel = 0;
    public float currentXp = 0;


    static List<float> levelsXP = new List<float>()
    {
        500, 1000, 1500, 2000f, 3000f, 5000f, 7500f, 15000f, 20000f, 25000f
    };

    event levelNotify OnLevelChange;
    event xpNotify OnXpChange;


    public void Initialize(levelNotify _OnLevelChange, xpNotify _OnXpChange)
    {
        OnLevelChange += _OnLevelChange;
        OnXpChange += _OnXpChange;
    }
    public int GetLevel()
    {
        return currentLevel;
    }
    public float GetLevelToLevelsRation()
    {
        return ((GetLevel() * 1f) / levelsXP.Count);
    }
    public int GetLevelsCount()
    {
        return levelsXP.Count;
    }
    public Vector2 GetLevelLimits()
    {
        Vector2 _limits = new Vector2();

        //Min
        _limits.x = levelsXP[currentLevel];

        //Max
        //Return Next level's XP if this is not the last level;

        int _lastLevelIndex = levelsXP.Count - 1;

        if (currentLevel == _lastLevelIndex)
            _limits.y = levelsXP[currentLevel];
        else
            _limits.y = levelsXP[currentLevel + 1];


        return _limits;
    }
    public float GetXp()
    {
        return currentXp;
    }
    public void IncreaseXP(float _amount)
    {
        int _oldLevel = currentLevel;

        currentXp += _amount;
        OnXpChange?.Invoke();

        ComputeLevel();

        int _newLevel = currentLevel;

        if (_oldLevel != _newLevel)
            OnLevelChange?.Invoke();
    }
    void ComputeLevel()
    {
        int _level = 0;

        for (int i = levelsXP.Count - 1; i >= 0; i--)
        {
            if (currentXp >= levelsXP[i])
            {
                _level = i;
                break;
            }
        }

        currentLevel = _level;
    }
}