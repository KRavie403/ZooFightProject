using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characater_ItemReady : BaseState
{
    public Characater_ItemReady(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {

    }


    public override void Initate()
    {

        base.Initate();
        ableFuncs.Add(PlayerController.pFunc.Move, player.Move);
        ableFuncs.Add(PlayerController.pFunc.ItemUse, player.ItemUse);
        ableFuncs.Add(PlayerController.pFunc.ItemRelease, player.ItemRelease);

    }

    public override void Enter(BaseState BeforeState)
    {
        base.Enter(BeforeState);
        player.SetState(PlayerController.pState.ItemReady);
        //player.curItems.
        //player.curItems = ObjectPoolingManager.instance.GetObject<Item>
        //    (player.curItems.name, Vector3.zero, Quaternion.identity, player.ItemPoint,false);
        player.curItems.Initate(ItemSystem.Inst.ItemKeys[player.curItems.myCode].GetValues(),player);
        player.curItems.gameObject.SetActive(true);
    }

    public override void Exit()
    {
        base.Exit();

    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        ableFuncs[PlayerController.pFunc.Move]();
        //player.curItems.SetDir(player.transform.forward);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

    }
}
