using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ItemCode
{
    // 0   ~  99 전방발사형
    Bomb = 0,
    BananaTrap,

    // 100 ~ 199 자체버프형
    GuardDrink = 100,
    PowerDrink,


    // 200 ~ 299 적대작용형
    BlockChangeScroll = 200,
    CurseScroll,
    SpiderBomb,
    InkBomb,


    // 300 ~ 399 착용장비형
    ToyHammer = 300,


}

// 상태이상 강도가 높을수록 & 등급이 높을수록 높은숫자
public enum StatusCode
{
    // 0 = 기본상태 1 ~ 9 이동방해계열
    Normal = 0,
    Slow,       // 이동속도 감소
    //Minimal,    // 사이즈 감소 (이속,공속,점프력 감소)
    //Cripple,    // 공격속도 감소
    // 10 ~ 19  하급 상태 이상
    Blind = 10, // 시야 감소
    Bind,       // 속박
    //Weak,       // 블럭 잡기 불가
    //Silence,    // 아이템 사용 불가
    // 20 ~ 29  중급 상태 이상
    Stun,       // 기절 


    // 30 ~ 39  상급 상태 이상
    AirBone
}

public class ItemSystem : Singleton<ItemSystem> 
{

    public RangeViewer RangeViewer;

    public GameObject ViewerObj;


    public Dictionary<ItemCode, Items> ItemKeys = new Dictionary<ItemCode, Items>();
    // 정리예정
    public List<GameObject> ItemList = new List<GameObject>();


    // 아이템 내부정보 원형
    public List<Items> ItemBase = new List<Items>();


    public UnityEvent<Items> ItemOrignin;

    Vector3 tmpPos = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        
        //for(int i = 0; i < ItemList.Count; i++)
        //{
        //    GameObject obj = Instantiate(ItemList[i],transform);
        //    Item item = obj.GetComponent<Item>();
        //    ItemKeys.Add(item.myCode, item);
        //}
        for (int i = 0; i < ItemBase.Count; i++)
        {
            ItemKeys.Add(ItemBase[i].myCode,ItemBase[i]);
        }
        ItemKeys[ItemCode.BananaTrap].Value1 = 3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseItem(Items item,PlayerController players)
    {
        //item.ItemUse(players);
        players.curItems.ItemUse();
    }

    public void AddRequest(PlayerController player)
    {

    }
    public void ResetRequest()
    {
        ItemOrignin.RemoveAllListeners();
    }

    public void CallItem()
    {
        ItemOrignin.Invoke(ItemBase[Random.Range(0, ItemList.Count)]);
    }

    public Items RandomItemSelect()
    {
        // 테스트용 고정출력
        return ItemBase[0];// 삭제예정
        return ItemBase[Random.Range(0, ItemList.Count)];

    }

    // 랜덤 아이템을 오브젝트 풀에서 가져와서 입력받은 캐릭터에게 넣어주기
    public Items GiveItem(PlayerController player,Items item)
    {
        if (player.curItems != null) return null;
        //Item item = RandomItemSelect();

        return ObjectPoolingManager.instance.GetObject<Items>(item.name, Vector3.zero, Quaternion.identity, player.ItemPoint,false);
    }

    public Items GetItemOrigin(Items item)
    {

        return null;
    }

}

