using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    Animator myStateMachine;
    IStateMachineController stateMachineController;
    List<StateInfo> myStates = new List<StateInfo>();
    int currentStateHash;
    bool isInitialized = false;
    float timeSinceLastAction = 0;

    //Interface
    public void Initialize(Enum states)
    {
        stateMachineController = GetComponentInParent<IStateMachineController>();
        myStateMachine = GetComponent<Animator>();

        foreach (var state in Enum.GetValues(states.GetType()))
        {
            StateInfo newState = new StateInfo();
            newState.stateName = state.ToString();
            newState.stateHash = Animator.StringToHash(newState.stateName);
            newState.state = (Enum)state;

            myStates.Add(newState);
        }

        isInitialized = true;
    }
    public void SetBool(Enum boolName, bool state)
    {
        myStateMachine.SetBool(boolName.ToString(), state);
    }
    public void SetTrigger(Enum triggerName)
    {
        myStateMachine.SetTrigger(triggerName.ToString());
    }
    public float GetTimeSinceLastChange()
    {
        return timeSinceLastAction;
    }
    public Enum GetCurrentState()
    {
        return (GetEnumByHash(currentStateHash));
    }


    //Helpers
    void Update()
    {
        if(isInitialized)
        {
            if (myStateMachine.GetCurrentAnimatorStateInfo(0).shortNameHash != currentStateHash)
            {
                currentStateHash = myStateMachine.GetCurrentAnimatorStateInfo(0).shortNameHash;
                OnStateChange((GetEnumByHash(currentStateHash)));
            }
            else
            {
                timeSinceLastAction += Time.deltaTime;
            }
        }

    }
    void OnStateChange(Enum triggerStats)
    {
        timeSinceLastAction = 0;
        stateMachineController.ActionExecution(triggerStats);
    }
    Enum GetEnumByHash(int hash)
    {
        foreach (StateInfo state in myStates)
        {
            if(state.stateHash == hash)
            {
                return state.state;
            }
        }

        return myStates[0].state;
    }
}

