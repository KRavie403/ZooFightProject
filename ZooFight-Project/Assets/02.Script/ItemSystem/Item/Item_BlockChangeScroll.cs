using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템명 : 블럭교환스크롤
/// Value 1 
/// Value 2 이펙트 지속시간
/// Value 3 ???
/// </summary>

public class Item_BlockChangeScroll : Items
{
    public Item_BlockChangeScroll(PlayerController player) : base(player)
    {

    }

    [SerializeField]
    EffectPlayer myEffect;
    bool isEffectPlay = false;
    EffectCode myEffectCode = EffectCode.E_BlockChangeScroll;

    [SerializeField]
    SoundSpeaker mySound;
    bool isSoundPlay = false;   


    //public GameObject myBlockObj;
    public BlockObject myBlock;
    //public GameObject enemyBlockObj;
    public BlockObject enemyBlock;



    protected override void Awake()
    {
        base.Awake();

        myEffectCode = EffectCode.E_BlockChangeScroll;
        myCode = ItemCode.BlockChangeScroll;
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



        // 코루틴의 동작을 본체로 가져오기
        isItemUse = true;

    }
    protected override IEnumerator ItemActions()
    {
        yield return base.ItemActions();

        Debug.Log(EffectCode.E_BlockChangeScroll.ToString());
        // 정지 상태의 이펙트를 인출해오기
        myEffect = Effectmanager.Inst.effectPool.GetEffectObject<EffectPlayer>(EffectCode.E_BlockChangeScroll, Value2, null, false);


        float duringTime = 0;
        // 아군 , 상대편 블럭정보 가져오기
        if (myPlayer.myTeam == Team.RedTeam)
        {
            myBlock = Gamemanager.Inst.RedTeamBlock;
            enemyBlock = Gamemanager.Inst.BlueTeamBlock;
        }
        else if (myPlayer.myTeam == Team.BlueTeam)
        {
            myBlock = Gamemanager.Inst.BlueTeamBlock;
            enemyBlock = Gamemanager.Inst.RedTeamBlock;
        }

        // 잡혀있는 상태 해제
        myBlock.isChangeActive = true;
        enemyBlock.isChangeActive = true;
        myBlock.ForceDeGrab();
        enemyBlock.ForceDeGrab();
        // 블럭 교환
        myBlock.ChangeBlockTeam();
        enemyBlock.ChangeBlockTeam();


        // 블럭교환 이펙트 출력준비
        ChangeEffect();
        // 블럭교환 사운드 출력준비


        myBlock.isChangeActive = false;
        enemyBlock.isChangeActive = false;

        myPlayer.ItemUseEnd();

        // 블럭교환 실행
        while (duringTime < Value2)
        {
            duringTime += Time.deltaTime;


            if (isItemUse)
            {
                yield return null;
            }
            else
            {
                yield return base.ItemActions();
            }

        }

        Debug.Log($"{this} ActiveEnd");
        // 아이템 반환
        ReturnItem();
    }

    public override void ReturnItem()
    {
        myBlock = null;
        enemyBlock = null;

        // 교체 예정 
        //myEffect.gameObject.SetActive(false);
        //myEffect.transform.SetParent(transform, false);

        ObjectPoolingManager.instance.ReturnObject(myEffect.gameObject);

        // 교체 코드
        myEffect = null;

        base.ReturnItem();
    }

    public void ChangeEffect()
    {
        myEffect.gameObject.SetActive(true);

        // 기본이 최상위노드로 인출되게 변경 예정이라 삭제 예정

        myEffect.EffectPlayAll(0, 0, myBlock.transform);
        myEffect.EffectPlayAll(1, 0, enemyBlock.transform);
        
    }

    public IEnumerator ScrollActive()
    {
        float duringTime = 0;
        // 아군 , 상대편 블럭정보 가져오기
        if(myPlayer.myTeam == Team.RedTeam)
        {
            myBlock = Gamemanager.Inst.RedTeamBlock;
            enemyBlock = Gamemanager.Inst.BlueTeamBlock;
        }
        else if (myPlayer.myTeam == Team.BlueTeam)
        {
            myBlock = Gamemanager.Inst.BlueTeamBlock;
            enemyBlock = Gamemanager.Inst.RedTeamBlock;
        }


        // 블럭교환 이펙트 출력준비

        // 블럭교환 사운드 출력준비


        // 블럭교환 실행
        while (duringTime < Value2)
        {
            duringTime += Time.deltaTime;

            yield return null;
        }

    }



}
