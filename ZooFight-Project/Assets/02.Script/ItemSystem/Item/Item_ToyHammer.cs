using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템명 : 뿅망치
/// Value 1 데미지 -  미사용 가능성 O
/// Value 2 스턴 시간
/// Value 3 발동 범위 - 타격지점주위 범위
/// Value 4 
/// Value 5 사거리 - 휘두를수 있는 범위
/// </summary>

public class Item_ToyHammer : Items
{
    public Item_ToyHammer(PlayerController player) : base(player)
    {

    }


    [SerializeField]
    EffectPlayer myEffect;
    [SerializeField]
    SoundSpeaker mySound;

    public enum HammerAction
    {
        Smash = 0, hit , disappear
    }
    

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
    public override void Initate(List<float> Values, PlayerController player)
    {
        base.Initate(Values, player);
    }



    public void HammerSetting(Vector3 pos)
    {
        ItemAction = HammerActive(pos);
    }

    public IEnumerator HammerActive(Vector3 pos)
    {
        // 방향 설정
        Vector3 dir = Vector3.Normalize(pos - myPlayer.transform.position);

        // 해머 이펙트 출력대기
        HammerEffectSetting();
        // 해머 사운드 재생대기
        HammerSoundSetting();

        //myPlayer.
        // 해머 휘두르기
        while (!isItemUseEnd)
        {

            // 타격 감지시 이펙트 사운드 출력 & 타격대상 스턴

            
            yield return null;
        }
    }

    public void HammerEffectSetting()
    {

    }

    public void HammerSoundSetting()
    {

    }

    public void HammerEffectPlay(HammerAction action)
    {
        switch (action)
        {
            case HammerAction.Smash:
                //Effectmanager.Inst.effectPool.
                break;
            case HammerAction.hit:
                break;
            case HammerAction.disappear:
                break;
            default:
                break;
        }
    }

    public void HammerSoundPlay(HammerAction action)
    {
        switch (action)
        {
            case HammerAction.Smash:
                break;
            case HammerAction.hit:
                break;
            case HammerAction.disappear:
                break;
            default:
                break;
        }
    }










}
