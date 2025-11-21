using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataScripts;
using BackEnd;

public class Character_ItemUse : BaseState
{
    public Character_ItemUse(PlayerController player, StateMachine stateMachine) : base(player, stateMachine)
    {

    }


    public override void Initate()
    {

        base.Initate();
        //ableFuncs.Add(PlayerController.pFunc.PlayersMove, player.CurAxisMove);
        ableFuncs.Add(PlayerController.pFunc.ItemUse, player.ItemUse);

    }

    public override void Enter(BaseState BeforeState)
    {
        Logger.Log("아이템 사옹1");
        base.Enter(BeforeState);
        player.SetState(PlayerController.pState.ItemUse);
        //ableFuncs[PlayerController.pFunc.ItemUse]();

        // 아이템 사용상태 진입시 캐릭터 이동모션 취소
        player.AxisX = 0;
        player.AxisY = 0;
        player.myAnim.SetBool("IsMoving", false);
        player.myAnim.SetBool("IsRunning", false);

        // 아이템 사용 모션 출력
        player.myAnim.SetBool("ItemUse",true);

        var item = player.curItems;

        if (item != null)
        {
            var sessionId = Backend.Match.GetMySessionId();
            var teamNumber = BackEndMatchManager.GetInstance().GetTeamInfo(sessionId);

            ItemData_Class message = new ItemData_Class(item.myCode);
            BackEndMatchManager.GetInstance().SendDataToInGame<ItemData_Class>(message);

            message.curTeam = BackEndMatchManager.GetInstance().ConvertTeamNumberToEnum(teamNumber);
            message.ItemOwner = player;

            BackEndMatchManager.GetInstance().SendDataToInGame<ItemData_Class>(message);
        }
    }

    public override void Exit()
    {
        base.Exit();

        player.myAnim.SetBool("ItemUse", false);

        //player.curItems = null;
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
