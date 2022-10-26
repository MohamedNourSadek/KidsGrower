using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ActionTypes {Eat, Explore,Follow, Idle, Lay, Pick, Shake, Sleep, Throw, Punch, Drop, Null}

[System.Serializable]
public class AbstractAction
{
    public static AbstractAction ActionFactory(ActionTypes action, GameObject subject, NPC agent)
    {
        switch (action)
        {
            case(ActionTypes.Idle):
                return new Action_Idle(subject, agent);
            case(ActionTypes.Explore):
                return new Action_Explore(subject, agent);
            case (ActionTypes.Follow):
                return new Action_Follow(subject, agent);
            case (ActionTypes.Sleep):
                return new Action_Sleep(subject, agent);
            case (ActionTypes.Shake):
                return new Action_Shake(subject, agent);
            case (ActionTypes.Pick):
                return new Action_Pick(subject, agent);
            case (ActionTypes.Lay):
                return new Action_Lay(subject, agent);
            case (ActionTypes.Throw):
                return new Action_Throw(subject, agent);
            case (ActionTypes.Eat):
                return new Action_Eat(subject,agent);
            case (ActionTypes.Punch):
                return new Action_Punch(subject, agent);
            case (ActionTypes.Drop):
                return new Action_Drop(subject, agent);

            default: return null;
        }
    }

    public string actionName;
    protected int priority = 0;
    public bool isDone;
    public GameObject subject;
    protected NPC myAgent;
    [System.NonSerialized] public AbstractAction followUpAction;

    public AbstractAction(GameObject subject, NPC myAgent)
    {
        this.subject = subject;
        this.myAgent = myAgent;
    }

    public virtual void Execute()
    {
    }
    public virtual bool IsDone()
    {
        return isDone;
    }
    public virtual void Abort()
    {
        isDone = true;
    }
    protected virtual bool ShouldBreak()
    {
        if (subject == null)
            return true;

        if (isDone)
            return true;

        return false;
    }

    public int GetSubjectPriority()
    {
        if (subject)
        {
            if (subject.tag == "Fruit")
                return 3;
            else if (subject.tag == "Tree")
                return 2;
            else
                return 1;
        }
        else
            return 0;

    }
    public int GetPriority()
    {
        return GetSubjectPriority() * priority; 
    }
}



