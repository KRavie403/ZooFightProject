using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Anim_Character : MonoBehaviour
{
    PlayerController myPlayer = null;
    private void Start()
    {
        myPlayer = transform.parent.GetComponent<PlayerController>();
    }

    #region 이동 모션 함수

    public void MoveAction()
    {
        // ???0
    }
    public void MoveStartAction()
    {
        // 서버로 이동 시작 전송
    }
    public void MoveEndAction()
    {
        // 서버로 이동 종료 전송
    }


    #endregion


    #region 공격모션 함수
    public void AttackAction()
    {
        // 공격 액션 동작
        myPlayer.PlayerAttack();
    }

    public void AttackStartAction()
    {

    }

    public void AttackEndAction()
    {

    }

    public void AttackCancelAction()
    {

    }

    public void AttackPauseAction()
    {

    }

    public void AttackStopAction()
    {

    }

    

    #endregion

    #region 점프모션 함수
    public void JumpAction()
    {

    }
    public void JumpStartAction()
    {

    }
    public void JumpEndAction()
    {

    }
    #endregion

    #region 아이템사용모션 함수

    public void ItemUseAction()
    {

    }

    public void ItemUseStartAction()
    {

    }

    public void ItemUseEndAction()
    {

        myPlayer.curItems.ItemEnd();
    }

    public void ItemUseCancelAction()
    {
        myPlayer.myAnim.SetTrigger("ItemUseCancel");
    }

    public void ItemReadyAction()
    {
        myPlayer.myAnim.SetTrigger("ItemReady");
    }

    public void ItemReadyStartAction()
    {

    }

    // 타격대상 추출
    public void ItemHitAction()
    {
        //myPlayer.AttackPoint
    }

    #endregion

    #region 승패모션함수

    public void WinActionEnd()
    {
        myPlayer.enabled = false;
    }
    public void LoseActionEnd()
    {
        myPlayer.enabled = false;
    }

    #endregion



}

