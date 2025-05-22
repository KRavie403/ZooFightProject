using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Create : BaseState
{

    public Character_Create(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {

    }



    // 이 상태가 동작하기 위해 필요한 정보 세팅
    public override void Initate()
    {
        base.Initate();
        //ableFuncs.Add(PlayerController.pFunc.PlayersMove,player.CurAxisMove); 

    }

    public override void Enter(BaseState BeforeState)
    {
        base.Enter(BeforeState);
        player.SetState(PlayerController.pState.Create);
        //player.

    }

    public override void Exit()
    {

        base.Exit();
        base.act?.Invoke();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

    }

    public override void PhysicsUpdate()
    {

        base.PhysicsUpdate();
    }



}
