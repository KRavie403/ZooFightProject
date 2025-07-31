using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 아이템명 : 저주 스크롤
/// Value 1 지속시간
/// 
/// </summary>


public class Item_CurseScroll : Items
{
    public Item_CurseScroll(PlayerController player) : base(player)
    {

    }


    public EffectPlayer effectPlayer;

    Team TargetTeam = 0;

    List<PlayerController> myTargets = new List<PlayerController>();

    protected override void Awake()
    {
        base.Awake();
        myCode = ItemCode.CurseScroll;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void ItemHitAction()
    {
        base.ItemHitAction();

    }

    public override void Initate(List<float> Values, PlayerController player)
    {
        base.Initate(Values, player);
    }

    protected override IEnumerator ItemActions()
    {
        yield return base.ItemActions();

        float duringTime = 0.0f;

        // 대상 캐릭터 확정
        //myCharacter.myTeam
        PlayerController[] TargetCharcaters = new PlayerController[] { };


        // 대상 캐릭터 키 반전
        foreach (PlayerController c in TargetCharcaters)
        {
            c.isKeyReverse = true;
        }

        // 키반전 테스트용 자체 반전코드(삭제예정)
        myPlayer.isKeyReverse = true;

        // 저주 이펙트 출력 대기
        //CuresEffectSetting();
        // 저주 사운드 재생 대기
        //CurseSoundSetting();

        myPlayer.ItemUseEnd();


        while (duringTime < Value1)
        {
            duringTime += Time.deltaTime;

            Gamemanager.Inst.GetEnemyTeam(myPlayer.myTeam);


            yield return null;

        }

        // 대상 캐릭터 키 반전 해제
        foreach (PlayerController c in TargetCharcaters)
        {
            c.isKeyReverse = false;
        }

        // 테스트 코드
        myPlayer.isKeyReverse = false;
        // 아이템 반환
        ReturnItem();
    }

    public void CuresTargetSetting()
    {
        if(myPlayer.myTeam != Team.NotSetting)
        {
            TargetTeam = (Team)((int)myPlayer.myTeam * -1);
            // 타겟팀의 팀원들 인출해오기

        }
    }

    public void CuresEffectSetting()
    {
        for (int i = 0; i < myTargets.Count; i++)
        {
            // 추후 이펙트 매니저를 통해서 발동할수있도록 수정예정
            effectPlayer.EffectPlay(0, myTargets[i].transform);
        }
    }

    public void CurseSoundSetting()
    {
        // 사운드버전 제작 필요
        for (int i = 0; i < myTargets.Count; i++)
        {
            // 추후 사운드 매니저를 통해서 발동할수있도록 수정예정

        }
    }


}
