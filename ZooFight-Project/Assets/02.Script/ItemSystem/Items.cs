using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


interface IItems
{
    public void ItemUse();
}


public class Items : ItemProperty , IItems , IEffect ,IObjectId
{
    public Items(PlayerController curPlayer)
    {
        this.myPlayer = curPlayer;
        Debug.Log($"{this} Load");
    }

    [SerializeField]
    protected PlayerController myPlayer = null;

    public IEnumerator ItemAction;

    public int InputCount = 0;
    protected bool isItemUse = false;
    protected bool isItemUseEnd = false;
    public event Action ItemReady = delegate { };

    [SerializeField]
    protected Vector3 dir = Vector3.zero;

    [SerializeField]
    EffectCode ItemEffect;
    EffectCode IEffect.EffectCode => ItemEffect;



    public int ObjectId => myObjectId;
    ObjectType IObjectId.ObjectType => myType;

    protected virtual void Awake()
    {
        //if(transform.parent == ObjectPoolingManager.instance.gameObject)
        //{

        //}
        
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //curPlayer = Gamemanager.Inst.currentPlayer;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    protected virtual void OnEnable()
    {
        StopAllCoroutines();
        if (myPlayer != null)
        {
            ItemAction = ItemActions();
            if(ItemAction != null)
            {
                StartCoroutine(ItemActions());
            }

        }
    }

    protected virtual void OnDisable()
    {
        if(ItemAction != null)
        {
            StopAllCoroutines();
        }
    }

    

    /// <summary>
    /// 플레이어에게 아이템 지급 상태로 만들기
    /// </summary>
    /// <param name="Values"></param>
    /// <param name="player"></param>
    public virtual void Initate(List<float> Values,PlayerController player)
    {
        if (Values == null) return;
        if (Values.Count > 5) return;

        Value1 = Values[0];
        Value2 = Values[1];
        Value3 = Values[2];
        Value4 = Values[3];
        Value5 = Values[4];
        
        myPlayer = player;
        foreach(var value in Values)
        {
            Debug.Log(value);
        }
        transform.localPosition = Vector3.zero;
        Debug.Log($"{gameObject.name} Initate");
    }


    // 사용자 전달
    public virtual void ItemUse()
    {
        // 오브젝트가 꺼져잇다면
        if (!gameObject.activeSelf)
        {

        }

        // 잔여 작업이 있는지 확인 후 종료
        if (ItemAction != null)
        {
            StopCoroutine(ItemAction);
            ItemAction = ItemActions();
        }
        isItemUse = true;
        Debug.Log($"{gameObject.name} ItemUse");
    }

    public virtual void ItemHitAction()
    {
        isTarget = true;
    }

    // 아이템을 오브젝트 풀에 반납할때
    public virtual void ReturnItem()
    {
        //myPlayer = null;
        //GetComponent<myHitScanner>()?.SetMyTeam(myHitScanner.Team.NotSetting);
        ItemEnd();
        Debug.Log($"{gameObject.name} ObjectReturn");
        ObjectPoolingManager.instance.ReturnObject(gameObject);
    }

    // 아이템의 동작이 종료될때
    public virtual void ItemEnd()
    {
        isItemUseEnd = true;
        StopCoroutine(ItemAction);
        myPlayer.ItemUseEnd();
        myPlayer.curItems = null;
        Debug.Log($"{isItemUseEnd} , {myPlayer.curItems}");
        myPlayer = null;
        ItemAction = null;
        StopAllCoroutines();
        dir = Vector3.zero;
        Targets.Clear();
        Debug.Log($"{myPlayer} , {ItemAction} , {dir} , {Targets} ");
        GetComponent<HitScanner>()?.Reset();
    }

    protected virtual IEnumerator ItemActions()
    {
        Debug.Log($"{gameObject.name} ItemStandby");

        // 사용 시작전 정보갱신
        while (!isItemUse)
        {
            // 어떤 아이템이든 사용방향은 캐릭터의 전방
            SetDir(myPlayer.transform.forward);


            yield return null;
        }
        // 아이템 사용동작의 최근시점
        Debug.Log($"{gameObject.name} Activate");
        // 상속받는 아이템이 사용중 동작 작성
        // 
    }

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

    protected void OnDestroy()
    {
        if (myPlayer != null) 
        {
            myPlayer.curItems = null;
        }
    }


    public PlayerController GetPlayer()
    {
        return myPlayer;
    }

    public void SetDir(Vector3 Dir)
    {
        dir = Dir;
    }


    public void AddTargets(List<GameObject> target)
    {
        if (target == null) return;
        //foreach (GameObject targetItem in target)
        //{
        //    if (!Targets.Contains(targetItem))
        //    {
        //        Targets.Add(targetItem);
                
        //    }
        //}
        Targets.AddRange(target);
    }


    public void AddTargets(GameObject target)
    {
        if (target == null) return;
        Targets.Add(target);
    }



}

