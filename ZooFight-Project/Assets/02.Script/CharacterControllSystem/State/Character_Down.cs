using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Down : BaseState
{
    public Character_Down(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {

    }


    public override void Initate()
    {

        base.Initate();
        //ableFuncs.Add(PlayerController.pFunc.PlayersMove, player.CurAxisMove);

    }

    public override void Enter(BaseState BeforeState)
    {
        base.Enter(BeforeState);
        player.SetState(PlayerController.pState.Down);

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

    IEnumerator DownStateAction()
    {
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;

            yield return null;
        }
    }

}
