using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_GameEnd : BaseState
{
    public Character_GameEnd(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {

    }


    public override void Initate()
    {

        base.Initate();
        //ableFuncs.Add(PlayerController.pFunc.PlayersMove, player.CurAxisMove);
        ableFuncs.Add(PlayerController.pFunc.AllStop, player.ActionAllStop);
    }

    public override void Enter(BaseState BeforeState)
    {
        base.Enter(BeforeState);
        player.SetState(PlayerController.pState.GameEnd);
        ableFuncs[PlayerController.pFunc.AllStop]();
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
