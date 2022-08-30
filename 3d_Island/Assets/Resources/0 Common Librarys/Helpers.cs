using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers
{
    //returns time formated
    public static string GetTimeFormated(float timeInSeconds)
    {
        if (timeInSeconds < 60f)
        {
            return timeInSeconds.ToString("0.00") + " Seconds";
        }
        else if ((timeInSeconds / 60f) <= 60)
        {
            return (timeInSeconds / 60).ToString("0.00") + " Mins";
        }
        else
        {
            return (timeInSeconds / 3600f).ToString("0.00") + " Hours";
        }
    }
}
