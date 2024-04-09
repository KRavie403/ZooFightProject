using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템명 : 방어포션
/// Value 1 방어막량
/// Value 2 지속시간
/// Value 3
/// 
/// </summary>


public class Item_GuardDrink : Items
{
    public Item_GuardDrink(PlayerController player) : base(player)
    {

    }


    EffectPlayer myEffect;

    protected override void Update()
    {
        base.Update();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Initate(List<float> Values, PlayerController player)
    {
        base.Initate(Values, player);
        if(Values.Count >= 2)
        {
            Value1 = Values[0];
            Value2 = Values[1];
        }
        myPlayer = player;
    }

    public void ItemSetUp()
    {

    }

    public IEnumerator DrinkActive()
    {
        // 보호막 대상 세팅

        // 보호막 이펙트 출력준비

        // 보호막 사운드 재생준비


        // 보호막 지속동작 실행
        while (true)
        {
            // 지속시간 카운트
            
            // 피격감지시 사운드 이펙트 출력

            // 피격시 보호막량 감소

            yield return null;
        }
    }



}
