using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionChecker
{
    public bool isTrue = true;

    public ConditionChecker(bool startValue)
    {
        isTrue = startValue;
    }
    public void Update(bool condition)
    {
        isTrue = condition;
    }
}

