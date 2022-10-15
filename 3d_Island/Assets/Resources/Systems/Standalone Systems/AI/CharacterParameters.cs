using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;


[System.Serializable]
public class CharacterParameters 
{
    public LevelController levelControl = new LevelController();
    public string saveName = "Nameless";

    [Header("Fertility")]
    public float age = 0f;
    public float lastLaidSince = 0f;
    public float seekAlterProb = 1f;
    public float maxFertilityAge = 1000f;
    public float betweenLaysTime = 5000f;
    public float minLevelToLay = 3;
    public float layingTime = 10f;

    [Header("Extroversion")]
    public float seekNpcProb = 0f;
    public float seekPlayerProb = 0f;
    public float seekTreeProb = 1f;
    public float seekFruitProb = 1f;
    public float seekBallProb = 0f;
    public float dropBallProb = 0f;

    [Header("Aggressiveness")]
    public float throwBallOnPlayerProb = 0f;
    public float throwBallOnNpcProb = 0f;
    public float punchProb = 0f;

    [Header("Power")]
    public float speed = 0.75f;
    public float punchForce = 10f;

    [Header("Health")]
    public float sleepTime = 20f;
    public float deathTime = 10000f;
    public float growTime = 20f;

    public float GetFertility()
    {
        float maxValue = levelControl.GetLevelsCount() * maxFertilityAge;
        float currentValue = levelControl.GetLevel() * age;

        if (CanLay())
            return (1f - (currentValue / maxValue));
        else
            return 0f;
    }
    public float GetExtroversion()
    {
        return (seekNpcProb + seekPlayerProb + seekTreeProb + seekFruitProb + seekBallProb + dropBallProb)/6f;
    }
    public float GetAggressiveness()
    {
        return (throwBallOnNpcProb + throwBallOnPlayerProb + punchProb)/3f;
    }
    public float GetPower()
    {
        return (speed + punchForce);
    }
    public float GetHealth()
    {
        return ((deathTime-age) / (growTime * sleepTime));
    }
    public bool CanLay()
    {
        return (lastLaidSince >= betweenLaysTime) && (levelControl.GetLevel() >= minLevelToLay) && (age >= growTime);
    }
}
