using DataScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ItemBase : ItemProperty , IItems , IEffect
{

    // 아이템 사용 주체
    [SerializeField] protected PlayerController myPlayer = null;
    // 아이템 지속 동작
    public IEnumerator ItemAction;
    // 아이템 동작 종료 확인
    public bool isItemUseEnd = false;

    // 방향 지정 필요시 받아두는 칸
    protected Vector3 dir = Vector3.zero;

    EffectCode IEffect.EffectCode => effectCode;

    public ItemData itemData;


    void IItems.ItemUse()
    {

    }

    protected virtual void Awake()
    {

    }


    protected virtual void Start()
    {

    }


    protected virtual void Update()
    {

    }

    #region 아이템 정보관련
    #region 정보주입 

    // 사용 시작 전 정보주입
    public virtual void Initate(List<float> Values, PlayerController player)
    {
        if (Values == null) return;
        if (Values.Count > 5) return;

        Value1 = Values[0];
        Value2 = Values[1];
        Value3 = Values[2];
        Value4 = Values[3];
        Value5 = Values[4];

        myPlayer = player;
        isItemUseEnd = false;
        Debug.Log(Values);
    }

    public void SetTarget(List<GameObject> Target)
    {
        Targets = Target;
    }

    public void SetDir(Vector3 Dir)
    {
        dir = Dir;
    }

    #endregion
    #region 정보 추출
    // 모체에서 정보 추출할때 사용
    public List<float> GetValues()
    {
        List<float> Values = new List<float>
        {
            Value1,
            Value2,
            Value3,
            Value4,
            Value5
        };
        return Values;
    }
    public EffectCode GetEffectCode () { return effectCode; }
    public SoundCode GetSoundCode () { return soundCode; }

    #endregion
    #endregion


    #region 아이템 사용 동작

    // 아이템 발동시 동작
    public virtual void ItemUse()
    {
        if (myPlayer == null) return;
        


    }

    // 타격판정이 있는 아이템의 타격시 동작
    public virtual void ItemHitAction()
    {

    }

    // 아이템의 동작이 끝날때 실행
    public virtual void ItemEnd()
    {
        // 지속 동작 종료
        isItemUseEnd = true;

        // 데이터 리셋
        myPlayer.curItems = null;
        myPlayer = null;
        ItemAction = null;
        dir = Vector3.zero;
        GetComponent<HitScanner>().Reset();


    }
    
    public virtual IEnumerator ItemActions()
    {
        bool A = false;

        // 사용 시작전
        while (!A)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            { 
                A = true;
            }


            yield return null;
        }

    }



    #endregion
}
