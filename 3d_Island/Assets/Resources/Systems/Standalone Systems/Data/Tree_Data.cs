using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree_Data : SaveStructure
{
    public nVector3 position = new nVector3();
    public static List<Tree_Data> GameToDate(TreeSystem[] trees)
    {
        List<Tree_Data> list = new List<Tree_Data>();

        foreach (TreeSystem tree in trees)
            list.Add(tree.GetData());

        return list;
    }
}
