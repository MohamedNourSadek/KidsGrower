using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void levelNotify();
public delegate void xpNotify();

[System.Serializable]
public class LevelController
{
    //Data structure that must be public to be serialzed in save game
    public int currentLevel = 0;
    public float currentXp = 0;

    //private variables
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

    //Interface
    public int GetLevel()
    {
        return currentLevel;
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

        currentLevel = ComputeCurrentLevel();

        int _newLevel = currentLevel;

        if (_oldLevel != _newLevel)
            OnLevelChange?.Invoke();
    }
    public int GetLevelsCount()
    {
        return levelsXP.Count;
    }
    
    
    //Internal Algorithms
    int ComputeCurrentLevel()
    {
        int level = 0;

        for (int i = levelsXP.Count - 1; i >= 0; i--)
        {
            if (currentXp >= levelsXP[i])
            {
                level = i;
                break;
            }
        }

        return level;
    }
}