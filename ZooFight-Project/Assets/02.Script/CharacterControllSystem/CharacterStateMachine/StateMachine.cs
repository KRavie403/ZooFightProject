using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StateMachine 
{
    
    public BaseState CurrentState { get; protected set; }
    public BaseState BeforeState { get; protected set; }

    public void Initalize(BaseState startState)
    {
        CurrentState = startState;
        BeforeState = startState;
        startState.Enter(startState);
    }

    public void ChangeState(BaseState newState)
    {
        //Debug.Log($"{CurrentState} Exit Start");
        BeforeState = CurrentState;
        CurrentState.Exit();

        CurrentState = newState;
        newState.Enter(BeforeState);
        Debug.Log($"{BeforeState} Exit , {CurrentState} Enter");
    }
    



}
