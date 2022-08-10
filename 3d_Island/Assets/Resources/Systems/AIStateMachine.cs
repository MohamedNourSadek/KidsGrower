using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MovementStatus { Move, Picked, Idel, Explore, Sleep, Eat, Lay, Ball};
public enum BooleanStates { bored, tired, fruitNear, ballNear, alterNear, Picked}
public enum TriggerStates { foundTarget, lostTarget, doneEating, doneLaying, doneSleeping, doneBalling, reached}


public class AIStateMachine : MonoBehaviour
{
    Animator _myStateMachine;
    IStateMachineController _stateMachineController;
    List<StateInfo> _myStates = new List<StateInfo>();
    int _currentStateHash;
    public float _timeSinceLastAction = 0;


    private void Awake()
    {
        _stateMachineController = GetComponentInParent<IStateMachineController>();
        _myStateMachine = GetComponent<Animator>();

        foreach (MovementStatus state in Enum.GetValues(typeof(MovementStatus)))
        {
            StateInfo _state = new StateInfo();
            _state.stateName = state.ToString();
            _state.stateHash = Animator.StringToHash(_state.stateName);
            _state.state = state;

            _myStates.Add(_state);
        }
    }
    private void Update()
    {
        if (_myStateMachine.GetCurrentAnimatorStateInfo(0).shortNameHash != _currentStateHash)
        {
            _currentStateHash = _myStateMachine.GetCurrentAnimatorStateInfo(0).shortNameHash;
            OnStateChange(GetEnumByHash(_currentStateHash));
        }
        else
        {
            _timeSinceLastAction += Time.deltaTime;
        }
    }
    void OnStateChange(MovementStatus triggerStats)
    {
        _timeSinceLastAction = 0;
        _stateMachineController.ActionExecution(triggerStats);
    }


    //Interface
    public void SetBool(BooleanStates _stateName, bool state)
    {
        _myStateMachine.SetBool(_stateName.ToString(), state);
    }
    public void SetTrigger(TriggerStates _stateName)
    {
        _myStateMachine.SetTrigger(_stateName.ToString());
    }

    public float GetTimeSinceLastChange()
    {
        return _timeSinceLastAction;
    }


    //Helpers
    MovementStatus GetEnumByHash(int hash)
    {
        foreach (StateInfo _state in _myStates)
        {
            if(_state.stateHash == hash)
            {
                return _state.state;
            }
        }

        return MovementStatus.Idel;
    }
    public MovementStatus GetCurrentState()
    {
        return GetEnumByHash(_currentStateHash);
    }
}



[Serializable] public class StateInfo
{
    public string stateName;
    public int stateHash;
    public MovementStatus state;
}
public interface IStateMachineController
{
    public void ActionExecution(MovementStatus trigger);
}

