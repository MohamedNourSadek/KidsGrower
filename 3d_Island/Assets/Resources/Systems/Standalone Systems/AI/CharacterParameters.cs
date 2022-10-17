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
    public float fertilityFactor = 0.5f;
    public float age = 0f;
    public float lastLaidSince = 0f;
    public float seekAlterProb = 1f;
    public float betweenLaysTime = 5000f;
    public float minLevelToLay = 3;
    public float layingTime = 10f;

    [Header("Extroversion")]
    public float extroversionFactor = 0.5f;

    [Header("Aggressiveness")]
    public float aggressivenessFactor = 0.5f;

    [Header("Power")]
    public float powerFactor = 0.5f;
    public float maxSpeed = 5;
    public float maxPunchForce = 20;

    [Header("Health")]
    public float maxSleepTime = 40f;
    public float deathTime = 10000f;
    public float growTime = 20f;


    //Main Axioms
    public float GetFertility()
    {
        float maxValue = levelControl.GetLevelsCount() * deathTime;
        float currentValue = levelControl.GetLevel() * age;

        if (CanLay())
            return (1f - (currentValue / maxValue)) * fertilityFactor;
        else
            return 0f;
    }
    public float GetExtroversion()
    {
        return extroversionFactor;
    }
    public float GetAggressiveness()
    {
        return aggressivenessFactor;
    }
    public float GetPower()
    {
        return powerFactor * GetHealth();
    }
    public float GetHealth()
    {
        float maxHealth = (deathTime / (growTime));
        float currentHealth = (deathTime - age) / (growTime);

        return currentHealth / maxHealth;
    }


    //Dependent
    public bool CanLay()
    {
        return (lastLaidSince >= betweenLaysTime) && (levelControl.GetLevel() >= minLevelToLay) && (age >= growTime);
    }
    public float GetSleepTime()
    {
        return maxSleepTime * GetHealth();
    }
    public float GetSpeed()
    {
        return GetPower() * maxSpeed;
    }
    public float GetPunchForce()
    {
        return GetPower() * maxPunchForce;
    }
}
