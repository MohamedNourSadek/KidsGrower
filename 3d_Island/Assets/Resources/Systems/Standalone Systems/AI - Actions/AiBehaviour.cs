using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.TextCore.Text;


[System.Serializable]
public class AiBehaviour 
{
    [SerializeField] float decisionsDelay = 0.5f;
    [SerializeField] AbstractAction currentAction;
    [SerializeField] List<AbstractAction> actions = new List<AbstractAction>();
    NPC myAgent;

    public void Initialize(NPC agent)
    {
        myAgent = agent;
        ServicesProvider.instance.StartCoroutine(AiContinous());
        ServicesProvider.instance.StartCoroutine(AiDescrete());
    }
    public void AbortCurrentAction()
    {
        AbortAction(currentAction);
    }


    //Handle Information and make a decision
    public void OnDetectableInRange(IDetectable detectable)
    {
        if (detectable.tag == "Fruit" || detectable.tag == "Boost")
        {
            if (FlipACoinWithProb(myAgent.character.GetExtroversion()))
            {
                AddAction(detectable, ActionTypes.Follow, ActionTypes.Eat);
            }
        }
        else if (detectable.tag == "Alter")
        {
            if (myAgent.character.CanLay() && FlipACoinWithProb(myAgent.character.seekAlterProb))
            {
                AddAction(detectable, ActionTypes.Follow, ActionTypes.Lay);
            }
        }
        else if (detectable.tag == "Ball")
        {

            if ((((Ball)detectable).IsPicked() == false) && FlipACoinWithProb(myAgent.character.GetExtroversion()))
            {
                AddAction(detectable, ActionTypes.Follow, ActionTypes.Pick);
            }
        }
        else if ((detectable.tag == "Player") || (detectable.tag == "NPC"))
        {
            if ((myAgent.handSystem.GetObjectInHand() != null) && (myAgent.handSystem.GetObjectInHand().tag == "Ball"))
            {
                if (FlipACoinWithProb(myAgent.character.GetAggressiveness()))
                    AddAction(detectable, ActionTypes.Throw, ActionTypes.Null);
            }
            else
            {
                if (FlipACoinWithProb(myAgent.character.GetExtroversion()))
                {
                    if (FlipACoinWithProb(myAgent.character.GetAggressiveness()))
                    {
                        AddAction(detectable, ActionTypes.Follow, ActionTypes.Punch);
                    }
                    else
                    {
                        AddAction(detectable, ActionTypes.Follow, ActionTypes.Null);
                    }
                }
            }
        }
        else if ((detectable.tag == "Tree"))
        {
            if (((TreeSystem)detectable).GotFruit() && FlipACoinWithProb(myAgent.character.GetExtroversion()))
            {
                AddAction(detectable, ActionTypes.Follow, ActionTypes.Shake);
            }
        }
    }
    public void OnDetectableExit(IDetectable detectable)
    {
        //Only cancel if it's far away, not in your hand
        if (detectable.GetGameObject() != null)
            if (!myAgent.detector.IsNear(detectable.GetGameObject()))
                AbortAction(detectable);
    }
    public void OnDetectableNear(IDetectable detectable)
    {
        myAgent.handSystem.AddToPickable(detectable);
    }
    public void OnDetectableNearExit(IDetectable detectable)
    {
        myAgent.handSystem.RemoveFromPickables(detectable);
    }
    IEnumerator AiDescrete()
    {
        while (true)
        {
            GenerateActionIfEmptyAction();

            InturrptActionsWithLowPriority();

            yield return new WaitForSecondsRealtime(decisionsDelay);
        }
    }
    IEnumerator AiContinous()
    {
        while (true)
        {
            //Execute Actions
            if (currentAction.actionName == "" && actions.Count > 0 && !myAgent.isPicked)
            {
                currentAction = ReprioritizeActions();

                if (currentAction == null)
                {
                    yield return new WaitForFixedUpdate();
                    continue;
                }
                else if (currentAction.subject == null)
                {
                    currentAction.actionName = "";

                    yield return new WaitForFixedUpdate();
                    continue;
                }
                else if (myAgent.handSystem.GetObjectInHand())
                {
                    if (((Pickable)myAgent.handSystem.GetObjectInHand()) != null)
                    {
                        //currentAction = AbstractAction.ActionFactory(ActionTypes.Drop, handSystem.GetObjectInHand().gameObject, this);
                    }
                }

                if (actions.Contains(currentAction))
                    actions.Remove(currentAction);

                currentAction.Execute();

                while (true)
                {
                    if (currentAction.isDone)
                    {
                        if (currentAction.followUpAction == null)
                            break;
                        else
                        {
                            currentAction = currentAction.followUpAction;
                            currentAction.Execute();
                        }
                    }

                    yield return new WaitForFixedUpdate();
                }


                currentAction.actionName = "";
            }
            yield return new WaitForFixedUpdate();
        }
    }


    //Internal Algorithms
    void AddAction(IDetectable subject, ActionTypes actionType, ActionTypes followUpActionType)
    {
        if (subject != null && subject.GetGameObject() != null)
        {
            AbstractAction newAction = AbstractAction.ActionFactory(actionType, subject.GetGameObject(), myAgent);

            if (followUpActionType != ActionTypes.Null)
            {
                AbstractAction followUpAction = AbstractAction.ActionFactory(followUpActionType, subject.GetGameObject(), myAgent);
                newAction.followUpAction = followUpAction;
            }

            actions.Add(newAction);
        }
    }
    void AbortAction(IDetectable subject)
    {
        if (actions.Count > 0)
        {
            List<AbstractAction> toRemove = new List<AbstractAction>();

            foreach (AbstractAction action in actions)
            {
                if (action.subject != null && subject != null && subject.GetGameObject() != null)
                    if (action.subject == subject.GetGameObject())
                        toRemove.Add(action);
            }

            foreach (AbstractAction action in toRemove)
            {
                actions.Remove(action);
                action.isDone = true;
            }
        }

        if (currentAction.subject != null && subject != null && subject.GetGameObject() != null)
            if (currentAction.subject == subject.GetGameObject())
                AbortAction(currentAction);
    }
    void AbortAction(AbstractAction action)
    {
        if (action != null)
        {
            action.Abort();
            action.actionName = "";
            if (myAgent.navMeshAgent.isActiveAndEnabled)
                myAgent.navMeshAgent.SetDestination(myAgent.transform.position);
        }

    }
    AbstractAction GetRandomIdleAction()
    {
        float randomNum = UnityEngine.Random.Range(0f, 1f);
        AbstractAction action;
        if (randomNum >= 0.67)
            action = new Action_Explore(myAgent.gameObject, myAgent);
        else if (randomNum >= 0.33)
            action = new Action_Sleep(myAgent.gameObject, myAgent);
        else
            action = new Action_Idle(myAgent.gameObject, myAgent);

        return action;
    }
    void GenerateActionIfEmptyAction()
    {
        if ((actions.Count == 0) && currentAction.actionName == "" && !myAgent.isPicked)
        {
            if (myAgent.detector.GetInRange().Count == 0 && myAgent.detector.GetNear().Count == 0)
            {
                actions.Add(GetRandomIdleAction());
            }
            else
            {
                foreach (IDetectable detectable in myAgent.detector.GetInRange())
                    OnDetectableInRange(detectable);

                foreach (IDetectable detectable in myAgent.detector.GetNear())
                    OnDetectableNear(detectable);
            }
        }
    }
    bool FlipACoinWithProb(float prob)
    {
        float random = UnityEngine.Random.Range(0f, 1f);

        if (random <= prob)
            return true;
        else
            return false;
    }
    void CleanFaultyActions()
    {
        List<AbstractAction> faultyActions = new List<AbstractAction>();

        foreach (AbstractAction act in actions)
            if (act == null || act.actionName == "" || act.subject == null)
                faultyActions.Add(act);

        foreach (AbstractAction act in faultyActions)
            actions.Remove(act);
    }
    void InturrptActionsWithLowPriority()
    {
        if (actions.Count > 0)
            if (ReprioritizeActions().GetPriority() > currentAction.GetPriority())
                AbortAction(currentAction);
    }
    AbstractAction ReprioritizeActions()
    {
        CleanFaultyActions();

        if (actions.Count > 0)
        {
            AbstractAction action = actions[0];

            foreach (AbstractAction act in actions)
                if (act.GetPriority() > action.GetPriority())
                    action = act;

            return action;
        }
        else
            return new AbstractAction(myAgent.gameObject, myAgent);

    }

}
