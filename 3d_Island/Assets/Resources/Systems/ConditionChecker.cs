using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionChecker
{
    public bool isTrue = true;

    public ConditionChecker(bool _condition)
    {
        isTrue = _condition;
    }
    public void Update(bool _condition)
    {
        isTrue = _condition;
    }
}

