using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 아이템명 : ???
/// Value 1  
/// Value 2 
/// Value 3
/// Value 4
/// Value 5
/// </summary>

public class ItemProperty : MonoBehaviour
{

    public ObjectType myType = ObjectType.Item;
    public int myObjectId = -1;

    // 기본 골자
    public float Value1; // 데미지 or 회복량
    public float Value2; // 동작시간
    public float Value3; // 동작범위
    public float Value4; // 투사체속도    
    public float Value5; // 사거리

    public ItemCode myCode;


    public EffectCode effectCode;

    public SoundCode soundCode;

    public GameObject myPrefab;


    protected List<GameObject> _targets = new();
    public List<GameObject> Targets = new();


    public bool isTarget = false;





}
