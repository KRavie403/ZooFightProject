using System.Collections.Generic;
using UnityEngine;



// 타격을 받는 대상이 가지고있는 인터페이스
interface IHitBox 
{
    Component myHitBox 
    {
        get; 
    }
    
    public Team Team 
    { 
        get; 
    }

    // 히트박스를 가진 오브젝트가 타격판정이 났을때 해당 컴포넌트 전달
    public void HitAction(Component comp);



}

// 타격을 주는 객채가 가지고 잇는 클래스
public class HitScanner : MonoBehaviour
{
    //public enum Team { RedTeam = -1,NotSetting,BlueTeam,AllTarget}

    // 타격 대상
    Team targetTeam = Team.NotSetting;

    // 충돌 가능 오브젝트 레이어
    public LayerMask hitMask;

    // 사용자의 컴포넌트
    public Component UserComponent;



    List<GameObject> Targets;
    bool isScan = false;
    bool isActive = false;
    float ScanRange = 0.2f;

    // 데미지값
    public float MyDamage = 0.0f ;

    public void SetMyTeam(Team team)
    {
        targetTeam = team;
    }

    private void Awake()
    {

    }

    private void Start()
    {
        Targets = new List<GameObject>();
        //targetTeam = Team.BlueTeam;
        if(!isActive)
        {
            this.enabled = false;
        }
    }

    private void Update()
    {

    }

    // 사용 객체(아이템 or 투사체? or 다른 무언가) 와 대상 팀 주입
    public void Initiate(Component comp,Team TargetTeam)
    {
        if(comp.GetComponent<Component>() != null)
        {
            UserComponent = comp;
            targetTeam = TargetTeam;

        }
    }
                                                                                                                                           
    public void SetScanActive(bool Active)
    {
        isActive = Active;
        if(Active)
        {
            enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 사용 시작시
        if(isActive)
        {
            Debug.Log("HitScanStart");
            // 대상의 히트박스가 없다면 종료
            if (other.gameObject.GetComponent<IHitBox>() == null) return;
            else
            {
                Debug.Log(other.gameObject);
                if(targetTeam == Team.NotSetting)
                {
                    return;
                }
                else if(targetTeam != other.GetComponent<IHitBox>().Team)
                {
                    return;
                }

            
                if(!Targets.Contains(other.gameObject))
                {
                    Targets.Add(other.gameObject);
                    Debug.Log($"{other} Add Target");
                }
            
                if( UserComponent.GetComponent<Items>() != null )
                {
                    if( other.gameObject != UserComponent.GetComponent<Items>().GetPlayer().gameObject)
                    {
                        UserComponent.GetComponent<Items>().AddTargets(other.gameObject); 
                        Targets.Remove(other.gameObject);
                        Debug.Log($"{other} Add Item Target");
                    }


                }
                else if(UserComponent.GetComponent<PlayerController>() != null)
                {

                }


                if (Targets != null) 
                {
                    isScan = true;
                }

            }
            // 스캔하기
            //Scan(ScanRange);

            //Targets.Add(other.gameObject);


            //if ((int)targetTeam * (int)other.gameObject.GetComponent<IHitBox>().Team == 0)
            //{

            //    return;
            //}
            //if (targetTeam != other.gameObject.GetComponent<IHitBox>().Team )
            //{
            //    return;
            //}
            //Debug.Log("Hit");
            //other.GetComponent<IHitBox>().HitAction(UserComponent);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (isActive)
        {
            if(other.gameObject.GetComponent<IHitBox>() != null)
            {

            }
        }
    }


    // 탐색 지정 대상을 탐색
    public void Scan(float Range)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, Range);

        Targets = new List<GameObject>();

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == this.gameObject) continue;

            if (collider.GetComponent<IHitBox>() != null) 
            {
                if(targetTeam == Team.AllTarget)
                {
                    if(!Targets.Contains(collider.gameObject))
                    {
                        Targets.Add(collider.gameObject);
                    }
                    continue;
                }
                if (targetTeam == Team.NotSetting) 
                {
                    continue;
                }
                if (targetTeam != collider.gameObject.GetComponent<IHitBox>().Team)
                {
                    continue;
                }
                Debug.Log(collider.gameObject);
                if (!Targets.Contains(collider.gameObject))
                {
                    Targets.Add(collider.gameObject);
                }
            }
        }
        if (Targets != null)
        {
            if (UserComponent.GetComponent<Items>() != null)
            {
                UserComponent.GetComponent<Items>().AddTargets(Targets);
            }
            else if (UserComponent.GetComponent<PlayerController>() != null) 
            {

            }
            isScan = true;
        }
    }

    public void Reset()
    {
        UserComponent = null;
        targetTeam = Team.NotSetting;
    }
}