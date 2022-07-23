using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionChecker
{
    public bool condition = true;

    public ConditionChecker(ref bool _condition)
    {
        condition = _condition;
    }
    public void Update(ref bool _condition)
    {
        condition = _condition;
    }
}

