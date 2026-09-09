using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템명 : 파워 드링크
/// Value 1 이동 속도 증가량 퍼센트
/// Value 2 동작 시간
/// 
/// </summary>


public class Item_PowerDrink : Items
{
    public Item_PowerDrink(PlayerController player) : base(player)
    {

    }

    EffectPlayer myEffect = null;
    GameObject myEffectObj = null;

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void ItemUse()
    {
        base.ItemUse();


    }

    protected override IEnumerator ItemActions()
    {

        yield return base.ItemActions();

        float duringTime = 0;

        // 기본 속도값 저장
        float BaseSpeedRate = 1.0f;

        myPlayer.BaseSpeedRate = myPlayer.BaseSpeedRate * (100 + Value1) / 100;

        // 이펙트 출력

        // 마시는 모션 출력
        myPlayer.myAnim.SetBool("isDrink", true);
        myPlayer.myAnim.SetTrigger("Drinking");

        myPlayer.ItemUseEnd();

        myEffectObj = EffectManager.Inst.effectPool.GetEffectObject(effectCode);
        //myEffect = EffectManager.Inst.effectPool.GetEffectObject<EffectPlayer>(EffectCode.E_IncreseStatus, myPlayer.BuffEffectPoint, true);
        myEffect = myEffectObj.GetComponent<EffectPlayer>();

        myEffect.EffectPlayAll(myPlayer.BuffEffectPoint, 0, true);
        // 지속시간 체크
        while (duringTime < Value2)
        {
            duringTime += Time.deltaTime;


            // UI 추가할시 남은값전달은 여기서

            yield return null;

        }

        myEffect.EffectEndAll();
        EffectManager.Inst.effectPool.ReturnObject(myEffectObj);

        // 지속시간 종료 후 속도 복귀
        myPlayer.BaseSpeedRate = BaseSpeedRate;
        ReturnItem();
    }


}
