using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Idle : BaseState
{

    public Character_Idle(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {

    }



    public override void Initate()
    {
        base.Initate();
        ableFuncs.Add(PlayerController.pFunc.Move, player.Move);
        ableFuncs.Add(PlayerController.pFunc.Jump, player.Jump);
        ableFuncs.Add(PlayerController.pFunc.ItemReady, player.ItemReady);

    }

    public override void Enter(BaseState BeforeState)
    {
        base.Enter(BeforeState);
        player.SetState(PlayerController.pState.Idle);

    }

    public override void Exit()
    {
        base.Exit();

        base.act?.Invoke();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        ableFuncs[PlayerController.pFunc.Move]();
        player.MoveStateCheck();
    }


    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

    }

}
