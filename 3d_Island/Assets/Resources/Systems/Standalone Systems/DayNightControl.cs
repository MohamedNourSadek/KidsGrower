using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightControl : MonoBehaviour
{
    public static DayNightControl instance;

    [SerializeField] public float factor;
    [SerializeField] float speed = 1f;

    [Header("Light")]
    [SerializeField] float maxDayLightIntensity = 1.4f;
    [SerializeField] float maxNightLightIntensity = 1.4f;
    [SerializeField] float startingAngle = 50f;
    [SerializeField] Color dayColor;
    [SerializeField] Color nightColor;

    [Header("Water")]
    [SerializeField] Material water;
    [SerializeField] Material outerWater;
    [SerializeField] Color waterDayColor;
    [SerializeField] Color waterNightColor;
    [SerializeField] Color foamDayColor;
    [SerializeField] Color foamNightColor;

    [Header("Environment")]
    [SerializeField] Color fogDayColor;
    [SerializeField] Color fogNightColor;
    [SerializeField] Material cloud;

    [Header("References")]
    [SerializeField] Light dayLight;
    [SerializeField] Light nightLight;

    private void Awake()
    {
        instance = this;
        factor = startingAngle * Mathf.Deg2Rad;
    }
    private void FixedUpdate()
    {
        if (factor >= (2 * Mathf.PI))
        {
            factor = 0;
        }

        factor += (Time.fixedDeltaTime * speed);

        UpdateLight();
        UpdateWater();
        UpdateEnvironment();
    }

    void UpdateLight()
    {
        if (factor >= Mathf.PI)
        {
            //Night
            dayLight.intensity = 0f;
            nightLight.intensity = Mathf.Abs(maxNightLightIntensity * Mathf.Sin(factor));
        }
        else
        {
            //Day

            dayLight.intensity = maxDayLightIntensity * GetDayIntensity();
            nightLight.intensity = 0f;
        }

        RenderSettings.ambientLight = Color.Lerp(nightColor, dayColor ,  GetDayNightFactor());

        dayLight.transform.rotation = Quaternion.Euler((factor * Mathf.Rad2Deg), -30f, 0f);
        nightLight.transform.rotation = Quaternion.Euler((factor * Mathf.Rad2Deg) + 180f, -30f, 0f);
    }
    void UpdateWater()
    {
        water.SetColor("_WaterColorShallow", Color.Lerp(waterNightColor, waterDayColor, GetDayNightFactor()));
        water.SetColor("_WaterColorDeep", Color.Lerp(waterNightColor, waterDayColor, GetDayNightFactor()));
        water.SetColor("_WaterColorHorizon", Color.Lerp(waterNightColor, waterDayColor, GetDayNightFactor()));
        outerWater.color = Color.Lerp(waterNightColor, waterDayColor, GetDayNightFactor());

        water.SetColor("_IntersectionFoamColor", Color.Lerp(foamNightColor, foamDayColor, GetDayNightFactor()));
    }
    void UpdateEnvironment()
    {
        RenderSettings.fogColor = Color.Lerp(fogNightColor, foamDayColor, GetDayNightFactor());
        
        cloud.SetFloat("_Intensity", Mathf.Pow(GetDayNightFactor(),4));
    }


    public float GetDayNightFactor()
    {
        //1 is absolute day, 0 is absolute night.

        float sin = Mathf.Sin(factor);
        return AdditionalMath.RemapRange(sin, new Vector2(-1f, 1f), new Vector2(0f, 1f));
    }
    public bool IsDay()
    {
        if(GetDayNightFactor() >= 0.5f)
            return true;
        else 
            return false;
    }
    float GetDayIntensity()
    {
        float convergValue = 0.3f;

        if(Mathf.Abs(Mathf.Sin(factor)) >= convergValue)
        {
            return 1f;
        }
        else
        {
            return Mathf.Abs(Mathf.Sin(factor)) * (1f/ convergValue);
        }
    }
}
