using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Character_Recovery : BaseState
{
    public Character_Recovery(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {

    }


    public override void Initate()
    {

        base.Initate();
        //ableFuncs.Add(PlayerController.pFunc.PlayersMove, player.CurAxisMove);
        player.SetState(PlayerController.pState.Recovery);

    }

    public override void Enter(BaseState BeforeState)
    {
        base.Enter(BeforeState);
        if(BeforeState == player.p_States[PlayerController.pState.Down])
        {

        }
        player.SetState(PlayerController.pState.Recovery);

    }

    public override void Exit()
    {
        base.Exit();

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
