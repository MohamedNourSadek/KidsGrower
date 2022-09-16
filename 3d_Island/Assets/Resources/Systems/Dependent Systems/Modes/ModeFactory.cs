using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModeFactory : MonoBehaviour
{
    public static AbstractMode CreateModeHandler(SessionData session)
    {
        if (session.modeData.modeName == modes.UnderPopulation)
            return new UnderPopulation(session.modeData);
        else if(session.modeData.modeName == modes.FreeMode)
            return new FreeMode(session.modeData);
        else
            return null;
    }
}
