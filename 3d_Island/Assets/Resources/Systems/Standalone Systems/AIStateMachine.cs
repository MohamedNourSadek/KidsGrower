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
    public void Initialize(Enum _states)
    {
        stateMachineController = GetComponentInParent<IStateMachineController>();
        myStateMachine = GetComponent<Animator>();

        foreach (var _state in Enum.GetValues(_states.GetType()))
        {
            StateInfo _newState = new StateInfo();
            _newState.stateName = _state.ToString();
            _newState.stateHash = Animator.StringToHash(_newState.stateName);
            _newState.state = (Enum)_state;

            myStates.Add(_newState);
        }

        isInitialized = true;
    }
    public void SetBool(Enum _boolName, bool _state)
    {
        myStateMachine.SetBool(_boolName.ToString(), _state);
    }
    public void SetTrigger(Enum _triggerName)
    {
        myStateMachine.SetTrigger(_triggerName.ToString());
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
    void OnStateChange(Enum _triggerStats)
    {
        timeSinceLastAction = 0;
        stateMachineController.ActionExecution(_triggerStats);
    }
    Enum GetEnumByHash(int _hash)
    {
        foreach (StateInfo _state in myStates)
        {
            if(_state.stateHash == _hash)
            {
                return _state.state;
            }
        }

        return myStates[0].state;
    }
}

