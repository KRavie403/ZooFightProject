using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using UnityEngine;


public enum ScanType
{
    Sphere, //구체형
    Circle, //원형

    Square, //정사각형
    Cube, //정육면체
    Box, //직육면체
    Type
}

public enum ScanTarget
{
    Player,
    Block,
    Item,

    Test1,
    TypeCount
}

public enum PlayerTeam
{ 
    RedTeam = -1,
    NotSetting = 0, 
    BlueTeam = 1, 
    AllTarget 
}

interface IHitScanTarget
{
    Component myComp
    {
        get;
    }

    /// <summary>
    /// 피격 판정 내부에 들어온 객체에게 타격주체의 정보를 알려주는 함수
    /// </summary>
    /// <param name="component"></param>
    public virtual void Hit(Component component)
    {

    }
    public virtual Type GetMyType()
    {
        return GetType();
    }

    int testcode { get; }
}

interface IHitScanner
{
    Component myComp
    {
        get;
    }

    Component[] myTargets
    {
        get; 
    }

    /// <summary>
    /// 탐색한 결과를 탐색 주체에게 전달
    /// </summary>
    /// <param name="target"></param>
    public virtual void AddTarget(Component[] target)
    {
       
    }



    /// <summary>
    /// 탐색주체에게 타격명령의 실행을 시키는함수
    /// </summary>
    public virtual void Hit()
    {

    }

}

/// <summary>
/// 특정 오브젝트의 탐색을 하는 스크립트
/// </summary>
public class HitScan : Singleton<HitScan> 
{

    // 테스트용 변수
    Component[] comps;

    private void Update()
    {
        

        // 테스트용 함수
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 넉백 작용 테스트
            //Debug.Log("InputSpace");

            //comps = HitScans(this.gameObject, 2.0f, ScanTarget.Player, ScanType.Sphere);

            //if(comps != null)
            //{
            //    Debug.Log("ScannerScanned");
            //    foreach(var comp in comps)
            //    {
            //        Debug.Log(comp.GetComponent<IHitScanTarget>().testcode);

            //        comp.GetComponent<IHitScanTarget>().Hit(this as Component);
            //    }
                
            //}



            // 기본테스트
            //comps = HitScans(this.gameObject, 2.0f, ScanTarget.Test1,ScanType.Sphere);

            //foreach (var comp in comps)
            //{
            //    Debug.Log(comp.GetComponent<IHitScanTarget>().testcode);
            //    comp.GetComponent<IHitScanTarget>().Hit(this as Component);
            //}
        }

    }

    /// <summary>
    /// 오브젝트(Obj) 의 범위(Range)내부에 있는 대상(Target)을 모양(Type)로 스캔함
    /// </summary>
    /// <param name="obj">스캔의 기준이 되는 오브젝트</param>
    /// <param name="range">스캔의 범위</param>
    /// <param name="targets">탐색을 할 대상</param>
    /// <param name="type">스캔 범위의 모양</param> 
    /// <returns></returns>
    public Component[] HitScans(GameObject obj, float range, ScanTarget targets, ScanType type)
    {

        switch (type)
        {
            case ScanType.Sphere:
                Debug.Log("aa");
                return ScanSphere(obj, range, GetTargets(targets));
            case ScanType.Circle:
                break;
            case ScanType.Square:
                break;
            case ScanType.Cube:
                return ScanCube(obj, range, GetTargets(targets));                
            case ScanType.Type:
                break;
            default:
                break;
        }


        return null;
    }

    /// <summary>
    /// 객체(obj) 을 기준으로 반지름(range)을 갖는 원형 범위 내부에 Targets 을 가진 객채를 스캔하는 함수
    /// </summary>
    /// <param name="obj">스캔의 주체</param>
    /// <param name="range">스캔 범위</param>
    /// <param name="Targets">검출 대상</param>
    /// <returns></returns>
    Component[] ScanSphere(GameObject obj, float range, Type Targets)
    {

        List<Collider> objs = Physics.OverlapSphere(obj.transform.position, range).ToList();
        if(objs.Count == 0)
        {
            return null;
        }
        List<Component> Target= new();
        Debug.Log(Targets);
        int count = 0;

        foreach (Collider collider in objs)
        {
            Debug.Log(collider.gameObject.name);
            if(collider.GetComponent<IHitScanTarget>() != null)
            {
                Debug.Log(collider.GetComponent<IHitScanTarget>().testcode);
                if(collider.gameObject.GetComponent<IHitScanTarget>().GetMyType() == Targets)
                {
                    Target.Add(collider.gameObject.GetComponent<IHitScanTarget>().myComp);
                    count++;
                }
            }

        }
        Debug.Log(count);

        
        if(count == 0)
        {
            return null;
        }
        obj.GetComponent<IHitScanner>().AddTarget(Target.ToArray());

        return Target.ToArray();

    }

    /// <summary>
    /// 객체(obj) 을 기준으로 한 변의 길이(range)을 갖는 정육면체 범위 내부에 Targets 을 가진 객채를 스캔하는 함수
    /// </summary>
    /// <param name="obj">스캔의 주체</param>
    /// <param name="range">스캔 범위</param>
    /// <param name="Targets">검출 대상</param>
    /// <returns></returns>
    Component[] ScanCube(GameObject obj, float range, Type Targets)
    {
        List<Collider> objs = Physics.OverlapBox(obj.transform.position,new Vector3(range/2,range/2,range/2)).ToList();
        List<Component> Target = new List<Component>();
        int count = 0;

        foreach (Collider collider in objs)
        {
            if (collider.GetComponent<IHitScanTarget>() != null)
            {
                if (collider.GetComponent<IHitScanTarget>() != null)
                {
                    if (collider.GetComponent<IHitScanTarget>().myComp.GetType() == Targets)
                    {
                        Target.Add(collider.gameObject.GetComponent<IHitScanTarget>().myComp);
                        count++;
                    }
                }
            }

        }

        if (count == 0)
        {
            return null;
        }
        obj.GetComponent<IHitScanner>().AddTarget(Target.ToArray());

        return Target.ToArray();
    }


    /// <summary>
    /// target 입력에 맞는 Class를 반환해주는 함수
    /// </summary>
    /// <param name="target">필요한 Class</param>
    /// <returns></returns>
    Type GetTargets(ScanTarget target)
    {
        switch (target)
        {
            case ScanTarget.Player:
                return typeof(PlayerController);
            case ScanTarget.Block:
                return typeof(BlockObject);
            case ScanTarget.Item:
                return typeof(Items);
            case ScanTarget.Test1:
                return typeof(TypeTest);
            case ScanTarget.TypeCount:
            default:
                break;
        }
        return null;
    }



}
