using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 아이템명 : 바나나
/// Value 1 밀려나는 속도
/// Value 2 함정 유지시간
/// Value 3 밀려나는 거리
/// Value 4 투사체 속도 
/// Value 5 투척 사거리
/// </summary>

public class Item_BananaTrap : Items , IHitScanner
{
    public Item_BananaTrap(PlayerController player) : base(player) 
    {
        
    }

    //
    [SerializeField]
    GameObject NonActiveObj;
    [SerializeField]
    GameObject ActivedObj;


    EffectCode myEffectCode = EffectCode.E_BananaTrap;

    [SerializeField]
    EffectPlayer myEffect;
    GameObject myEffectObj;

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

    // 혼란 가중으로 인한 재작성중

    protected override void Awake()
    {
        base.Awake();

        //신규코드
        myCode = ItemCode.BananaTrap;

        //기존코드
        // 히트스캐너 생성
        if(GetComponent<HitScanner>() != null )
        {
            myHitScanner = GetComponent<HitScanner>();
            myHitScanner.enabled = false;
        }
        else
        {
            // 미실행 상태로 생성
            myHitScanner = transform.AddComponent<HitScanner>();
            myHitScanner.enabled = false;
        }

        myCode = ItemCode.BananaTrap;

        
    }

    protected override void Start()
    {
        base.Start();

        
        Transform[] objs = GetComponentsInChildren<Transform>();
        foreach( Transform obj in objs )
        {
            if( obj.gameObject.name == "NonActiveObj")
            {
                NonActiveObj = obj.gameObject;
            }
            if( obj.gameObject.name == "ActivedObj")
            {
                ActivedObj = obj.gameObject;
            }
        }

        myHitScanner.SetMyTeam(myPlayer.myTeam);
        
    }

    protected override void Update()
    {
        base.Update();


    }

    protected override void OnEnable()
    {
        base.OnEnable();
        
    }


    public override void Initate(List<float> Values, PlayerController player)
    {
        base.Initate(Values, player);


        myHitScanner.Initiate(this, myPlayer.GetEnemyTeam());

    }



    public override void ItemUse()
    {
        base.ItemUse();

        // 아이템 발동시작시 필요한 동작

        // 코루틴의 동작을 본체로 가져오기
        isItemUse = true;
    }

    public override void ItemHitAction()
    {
        base.ItemHitAction();

    }


    protected override IEnumerator ItemActions()
    {
        // 정보갱신 돌리기
        yield return base.ItemActions();
        transform.SetParent(null);

        bool isMoveEnd = false;

        //transform.localPosition = Vector3.zero;
        // 특정 경로로 이동 시작
        BattleSystems.Inst.routeMaker.RouteKeys[RouteTypes.Arc].
            GetComponent<IRoute>().RouteStart(transform, dir, Value5, Value4, () =>
            {
                NonActiveObj.SetActive(false);
                ActivedObj.SetActive(true);
                myHitScanner.Initiate(this, myPlayer.GetEnemyTeam());
                isMoveEnd = true;
                myPlayer.ItemUseEnd();
            });
        //myPlayer.SetState()
        float duringTime = 0;
        myHitScanner.SetScanActive(true);

        myEffectObj = EffectManager.Inst.effectPool.GetEffectObject(effectCode);
        myEffect = myEffectObj.GetComponent<EffectPlayer>();
        //EffectManager.Inst.

        myEffect.EffectPlayAll(myPlayer.StunEffectPoint, 0, true);

        // 바나나 지속시간동안 동작
        while (duringTime < Value2)
        {
            // 던지는동안 지속시간 감소 X

            if (!isMoveEnd)
            {
                yield return null;
                continue;
            }
            duringTime += Time.deltaTime;
            // 유지시간 체크

            HitScan.Inst.HitScans(this.gameObject, 0.5f, ScanTarget.Player, ScanType.Sphere);

            if(isHit)
            {
                myTarget[0].GetComponent<IHitScanTarget>().Hit(this);
                break;
            }

            // 발동시 오브젝트 작동 불능처리

            if (isItemUse)
            {
                yield return null;
            }
            else
            {
                yield return base.ItemActions();
            }
            yield return null;
        }
        Debug.Log($"{this} ActiveEnd");

        myEffect.EffectEndAll();
        EffectManager.Inst.effectPool.ReturnObject(myEffectObj);

        // 사용이 끝나면 오브젝트 반환
        ReturnItem();

    }

    public override void ItemEnd()
    {
        base.ItemEnd();
        
        NonActiveObj.SetActive (true);
        ActivedObj.SetActive (false);

    }



    private void OnCollisionEnter(Collision collision)
    {

        //HitScan.Inst.HitScans(this.gameObject, 0.1,PlayerController as Component ,ScanType.Sphere);



        //PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        //if(player != null)
        //{

        //    Targets.Add(player.gameObject);

        //}
    }


}
