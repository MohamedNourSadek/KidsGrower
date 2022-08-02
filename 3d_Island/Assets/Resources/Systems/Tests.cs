using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tests : MonoBehaviour
{

    [SerializeField] Vector2 _range;
    [SerializeField] int increments;

    [SerializeField] DetectorSystem dete;

    private void Update()
    {
        if(Input.GetKeyDown("y"))
        {
            //Debug.Log(TestSuit.ComputeTime(test, (int)_range.y));
        }

        if (Input.GetKeyDown("x"))
        {
            //Debug.Log(TestSuit.ComputeTime(test2, (int)_range.y));
        }
    }

    void test()
    {

        List<Pickable> pickables = new List<Pickable>();

        pickables.Add(new Pickable());

        CleanListsFromDestroyedObjects(pickables);
    }

    void test2()
    {

        List<Pickable> pickables = new List<Pickable>();

        pickables.Add(new Pickable());

        CleanListsFromDestroyedObjects2(pickables);
    }


    void CleanListsFromDestroyedObjects(IList list)
    {
        int destroyedIndex = -1;
        for (int i = 0; i < list.Count; i++)
        {
            if (((MonoBehaviour)list[i]) == null)
                destroyedIndex = i;
        }
        if (destroyedIndex != -1)
            list.RemoveAt(destroyedIndex);
    }

    void CleanListsFromDestroyedObjects2(IList list)
    {
        int destroyedIndex = -1;
        for (int i = 0; i < list.Count; i++)
        {
            if ((list[i]) == null)
                destroyedIndex = i;
        }
        if (destroyedIndex != -1)
            list.RemoveAt(destroyedIndex);
    }
}
