using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 아이템명 : 소형화 광선
/// Value 1 효과 지속 시간
/// Value 2 사출시간
/// Value 3 레이저 길이
/// Value 4 사출속도
/// Value 5 레이저 사거리
/// </summary>


public class Item_MinimalRazer : Items ,IHitScanner
{
    public Item_MinimalRazer(PlayerController player) : base(player)
    {

    }

    public EffectPlayer effectPlayer;
    // 방향테스트용
    public GameObject Test1;

    public GameObject RazerGunObj;

    public Transform RazerStartPoint;

    public HitScanner myHitScanner;

    #region 히트스캔 코드
    bool isHit = false;

    public Component[] myTarget;

    Component IHitScanner.myComp => this as Component;

    Component[] IHitScanner.myTargets
    {
        get => myTarget;
    }

    void IHitScanner.AddTarget(Component[] target)
    {
        myTarget = target;
    }

    void IHitScanner.Hit()
    {
        //foreach (Component target in myTarget)
        //{
        //    target.GetComponent<IHitScanTarget>().Hit(this as Component);
        //}
        isHit = true;
    }

    #endregion


    Vector3 Dir = Vector3.zero;


    protected override void Awake()
    {
        base.Awake();
        //EffectManager.Inst.CreateEffectObj(effectPlayer, transform);\
        // 히트스캐너 생성
        if (GetComponent<HitScanner>() != null)
        {
            myHitScanner = GetComponent<HitScanner>();

            // 테스트 구간 시작
            if(myHitScanner == null )
            {
                gameObject.AddComponent<HitScanner>();
            }
            // 테스트 구간 종료

            myHitScanner.enabled = false;
        }
        else
        {
            // 미실행 상태로 생성
            myHitScanner = transform.AddComponent<HitScanner>();
            myHitScanner.enabled = false;
        }
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
       
        //effectPlayer = EffectSetting.keys[EffectCode.Item_MinimalRazer];

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Initate(List<float> Values, PlayerController player)
    {
        base.Initate(Values, player);
    }


    protected override IEnumerator ItemActions()
    {
        yield return base.ItemActions();

        float duringTime = 0;

        List<GameObject> curTargets = new List<GameObject>();

        effectPlayer.EffectPlayAll();
        // 발동시 히트스캐너 가동
        myHitScanner.SetScanActive(true);

        while (duringTime < Value2)
        {
            duringTime += Time.deltaTime;

            HitScan.Inst.HitScans(this.gameObject, 0.5f, ScanTarget.Player, ScanType.Sphere);

            if (isHit)
            {
                myTarget[0].GetComponent<IHitScanTarget>().Hit(this);
                break;
            }
            // 타겟 검출이 됫을 경우 
            //if(Targets != null)
            //{
            //    foreach (GameObject T in Targets)
            //    {
            //        if (curTargets.Contains(T)) continue;

            //        if (T.GetComponent<PlayerController>() != null)
            //        {
            //            T.GetComponent<PlayerController>().PlayerSizeChange(0.5f);
            //            curTargets.Add(T);
            //        }
            //    }
            //}


        }

        // 사용 종료시 크기 원상복구
        foreach (GameObject T in curTargets)
        {
            T.GetComponent<PlayerController>().PlayerSizeChange(1.0f);
        }


        ReturnItem();

    }

    
    
    public void RazerEffectSetting()
    {
        // 1번이펙트 간단실행

        // 2번이펙트 사출거리만큼 실행

    }

    public void RazerSoundSetting()
    {
        //Soundmanager.Inst.SoundPool.
        // 사출 사운드 재생
    }

    public void RazerShoot()
    {

    }

    public override void ItemUse()
    {
        base.ItemUse();
        //effectPlayer.
    }


}
