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
    public float SeekAlterProb = 1f;
    public float maxFertilityAge = 1000f;
    public float betweenLaysTime = 5000f;
    public float minLevelToLay = 3;
    public float layingTime = 10f;

    [Header("Extroversion")]
    public float SeekNpcProb = 0f;
    public float SeekPlayerProb = 0f;
    public float SeekTreeProb = 1f;
    public float SeekFruitProb = 1f;
    public float SeekBallProb = 0f;
    public float DropBallProb = 0f;

    [Header("Aggressiveness")]
    public float ThrowBallOnPlayerProb = 0f;
    public float ThrowBallOnNpcProb = 0f;
    public float PunchProb = 0f;

    [Header("Power")]
    public float speed = 0.75f;
    public float punchForce = 10f;

    [Header("Health")]
    public float SleepTime = 20f;
    public float DeathTime = 10000f;
    public float GrowTime = 20f;


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
        return SeekNpcProb * SeekPlayerProb * SeekTreeProb * SeekFruitProb * SeekBallProb * DropBallProb;
    }
    public float GetAggressiveness()
    {
        return ThrowBallOnNpcProb * ThrowBallOnNpcProb * PunchProb;
    }
    public float GetPower()
    {
        return speed * punchForce;
    }
    public float GetHealth()
    {
        return ((DeathTime-age) / (GrowTime * SleepTime));
    }

    public bool CanLay()
    {
        return (lastLaidSince >= betweenLaysTime) && (levelControl.GetLevel() >= minLevelToLay) && (age >= GrowTime);
    }
}
