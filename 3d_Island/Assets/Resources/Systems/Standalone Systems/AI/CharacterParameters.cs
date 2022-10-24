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
    public float minTimeBetweenLays = 5000f;
    public float minLevelToLay = 3;
    public float layingTime = 10f;
    public float minLayFertility = 0.5f;

    [Header("Extroversion")]
    public float extroversionFactor = .5f;

    [Header("Aggressiveness")]
    public float aggressivenessFactor = 0f;

    [Header("Power")]
    public float powerFactor = 0.5f;
    public float minSpeed = 0.5f;
    public float maxSpeed = 2.5f;
    public float minPunchForce = 2;
    public float maxPunchForce = 20;

    [Header("Health")]
    public float healthFactor = 0f;
    public float maxSleepTime = 40f;
    public float deathTime = 10000f;
    public float growTime = 20f;


    //Main Axioms
    public float GetFertility()
    {
        float levelFactor = Mathf.Clamp01(1f - (levelControl.GetLevel() / levelControl.GetLevelsCount()));
        float historyFactor = Mathf.Clamp01(lastLaidSince / minTimeBetweenLays);

        return Mathf.Clamp01(levelFactor * fertilityFactor * GetHealth() * historyFactor);
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
        // Health equation 
        //
        //     (X1/2, Y1)
        //         *
        //       *   *
        //      *     *
        //     *       *
        //    *         * 
        // (0,0)        (X1,0)

        // y =  (-4*Y1/X1^2) x^2 + (4*Y1/X1) x
        // X1 = Max Age = Death time
        // Y1 = Max health
        // x = age
        // y = health

        float firstTerm = (-4 * (healthFactor / Mathf.Pow(deathTime, 2))) * Mathf.Pow(age,2);
        float secondTerm = (4 * (healthFactor / deathTime)) * age;

        float health = firstTerm + secondTerm;

        return health;
    }



    //Dependent
    public bool CanLay()
    {
        return GetFertility() >= (minLayFertility);
    }
    public float GetSleepTime()
    {
        return maxSleepTime * GetHealth();
    }
    public float GetSpeed()
    {
        return minSpeed + (maxSpeed- minSpeed) * GetPower();
    }
    public float GetPunchForce()
    {
        return minPunchForce + (maxPunchForce - minPunchForce) * GetPower();
    }
}
