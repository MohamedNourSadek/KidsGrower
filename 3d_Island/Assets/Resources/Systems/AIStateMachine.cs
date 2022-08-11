using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    Animator _myStateMachine;
    IStateMachineController _stateMachineController;
    List<StateInfo> _myStates = new List<StateInfo>();
    int _currentStateHash;
    bool IsInitialized = false;
    float _timeSinceLastAction = 0;

    //Interface
    public void Initialize(Enum states)
    {
        _stateMachineController = GetComponentInParent<IStateMachineController>();
        _myStateMachine = GetComponent<Animator>();

        foreach (var state in Enum.GetValues(states.GetType()))
        {
            StateInfo _state = new StateInfo();
            _state.stateName = state.ToString();
            _state.stateHash = Animator.StringToHash(_state.stateName);
            _state.state = (Enum)state;

            _myStates.Add(_state);
        }

        IsInitialized = true;
    }
    public void SetBool(Enum _boolName, bool state)
    {
        _myStateMachine.SetBool(_boolName.ToString(), state);
    }
    public void SetTrigger(Enum _triggerName)
    {
        _myStateMachine.SetTrigger(_triggerName.ToString());
    }
    public float GetTimeSinceLastChange()
    {
        return _timeSinceLastAction;
    }
    public Enum GetCurrentState()
    {
        return (GetEnumByHash(_currentStateHash));
    }


    //Helpers
    void Update()
    {
        if(IsInitialized)
        {
            if (_myStateMachine.GetCurrentAnimatorStateInfo(0).shortNameHash != _currentStateHash)
            {
                _currentStateHash = _myStateMachine.GetCurrentAnimatorStateInfo(0).shortNameHash;
                OnStateChange((GetEnumByHash(_currentStateHash)));
            }
            else
            {
                _timeSinceLastAction += Time.deltaTime;
            }
        }

    }
    void OnStateChange(Enum triggerStats)
    {
        _timeSinceLastAction = 0;
        _stateMachineController.ActionExecution(triggerStats);
    }
    Enum GetEnumByHash(int hash)
    {
        foreach (StateInfo _state in _myStates)
        {
            if(_state.stateHash == hash)
            {
                return _state.state;
            }
        }

        return _myStates[0].state;
    }
}



[Serializable] public class StateInfo
{
    public string stateName;
    public int stateHash;
    public Enum state;
}
public interface IStateMachineController
{
    public void ActionExecution(Enum trigger);
}

