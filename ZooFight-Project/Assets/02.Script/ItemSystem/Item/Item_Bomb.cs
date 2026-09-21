using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템명 : 폭탄
/// Value 1 데미지
/// Value 2 폭탄 유지 시간
/// Value 3 폭발범위
/// Value 4 폭탄 속도
/// Value 5 사거리
/// 캐릭터만 밀려남 블럭은 밀려나지않음
/// <param name="Value1">데미지</param>
/// </summary>
public class Item_Bomb : Items ,IHitScanner
{
    public Item_Bomb(PlayerController player) : base(player)
    {

    }


    [SerializeField]
    EffectPlayer myEffect;
    [SerializeField]
    GameObject NonActiveObj;
    [SerializeField]
    GameObject ActivedObj;

    public HitScanner myHitScanner;
    
    bool isGroundCrash = false;

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

    protected override void Awake()
    {
        base.Awake();
        myCode = ItemCode.Bomb;

        //GetComponent<Rigidbody>().useGravity = false;
    }

    protected override void Start()
    {
        base.Start(); 

    }

    protected override void Update()
    {
        base.Update();

    }
    public void BombSetting(Vector3 pos)
    {
        //ItemAction = BombActive(pos);
        ItemAction = ItemActions();
    }



    bool isDone = false;

    protected override IEnumerator ItemActions()
    {
        yield return base.ItemActions();
        transform.SetParent(null);

        myEffect = EffectManager.Inst.effectPool.GetEffectObject<EffectPlayer>(EffectCode.E_Bomb_Explose, Value2, null, false);


        bool isMoveEnd = false;

        //transform.localPosition = Vector3.zero;
        // 특정 경로로 이동 시작
        BattleSystems.Inst.routeMaker.RouteKeys[RouteTypes.Arc].
            GetComponent<IRoute>().RouteStart(transform, dir, Value5, Value4, () =>
            {
                //NonActiveObj.SetActive(false);
                //ActivedObj.SetActive(true);
                //myHitScanner.Initiate(this, myPlayer.GetEnemyTeam());
                isMoveEnd = true;
                myPlayer.ItemUseEnd();
            });

        float duringTime = 0;

        while (!isDone)
        {
            // 유지시간 초과시 사용 종료
            if(duringTime >= Value2)
            {
                isDone = true;
                continue;
            }

            // 유지시간 체크
            duringTime += Time.deltaTime;


            if (!isMoveEnd)
            {
                yield return null;
                continue;
            }


            // 동작감지시 효과적용 및 이펙트 , 사운드 출력
            //if (Targets.Count != 0) 
            //{
            //    // 타겟이 잡히면 동작시키고 터트림
            //    BombExplosionEffect();
            //    Debug.Log("BombStart");

            //    isDone = false;
            //}

            HitScan.Inst.HitScans(this.gameObject, 0.5f, ScanTarget.Player, ScanType.Sphere);

            if (isHit)
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

        //myHitScanner.SetScanActive(false);

        // 사용이 끝나면 오브젝트 반환
        Debug.Log($"{this} ActiveEnd");
        isDone = false;
        ReturnItem();
    }

    public void BombExplosionEffect()
    {

        myEffect.EffectPlayAll(transform);

    }

    public void BombExplosionSound() 
    {



    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("GroundCollision");
        if(collision.gameObject.layer == LayerMask.GetMask("Ground"))
        {
            Collider[] carshed = Physics.OverlapSphere(transform.position, Value3);
            
            foreach(Collider collider in carshed)
            {
                Debug.Log(collider.gameObject.name);
                Targets.Add(collider.gameObject);
            } 
        }
    }


    // 대상을 넉백시키는 함수 - Transform , 밀려날 거리 , 속도 입력받기
    public void PushOut(Transform[] transform , float Dist , float Speed)
    {

        StartCoroutine(PushedOut(transform, Dist, Speed));
    }

    public IEnumerator PushedOut(Transform[] transform , float Dist , float Speed)
    {
        float duringTime = 0;

        while (duringTime < Dist / Speed) 
        { 
            duringTime += Time.deltaTime;



            yield return null;
        }

    }




}
