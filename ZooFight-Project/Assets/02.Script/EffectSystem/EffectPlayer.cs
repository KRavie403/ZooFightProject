using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 이펙트의 실행 , 시작 시간 , 루프 , 출력방향 , 출력위치 를 결정하는 클래스 
/// </summary>
public class EffectPlayer : MonoBehaviour , IEffect
{

    [SerializeField]
    EffectCode myEffectCode;


    public GameObject myObj;

    // 하위 이펙트 재생 시 사용
    public List<ParticleSystem> myEffect;

    // 다수 이펙트 동시 제어시 사용
    public List<EffectPlayer> myEffectPlayers;

    public List<Transform> triggerTarget;

    public event Action EffectAction = delegate { };

    EffectCode IEffect.EffectCode => myEffectCode;

    private void Awake()
    {
        //gameObject.name = myEffectCode.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    EffectPlayAll();
        //}

    }

    #region 이펙트 실행
    // 인덱스 실행
    public void EffectPlay(int index)
    {
        myEffect[index].Play();
    }

    // 인덱스 실행 + 시작지점 지정
    public void EffectPlay(int index,float playTime)
    {
        myEffect[index].time = playTime;
        myEffect[index].Play();
    }

    public void EffectPlay(int index, Transform StartPoint)
    {
        myEffect[index].transform.position = StartPoint.position;
        myEffect[index].Play();
    }

    // 인덱스 실행 , 반복출력
    public void EffectPlay(int index,float playTime,Transform StartPoint, bool isLoop)

    {
        var main = myEffect[index].main;
        main.loop = isLoop;
        myEffect[index].transform.position = StartPoint.position;
        myEffect[index].time = playTime; 
        myEffect[index].Play();
    }
    // 인덱스 실행 , 시작위치 , 바라볼 방향
    public void EffectPlay(int index,float playTime,Transform StartPoint,Vector3 dir)
    {
        myEffect[index].time = playTime;
        myEffect[index].transform.position = StartPoint.position;
        myEffect[index].transform.LookAt(dir);
        myEffect[index].Play();

    }
    // 인덱스 실행 , 시작위치 , 회전
    public void EffectPlay(int index, float playTime,Transform StartPoint,Quaternion rot)
    {
        myEffect[index].time = playTime;
        myEffect[index].transform.position = StartPoint.position;
        myEffect[index].transform.rotation = rot;
        myEffect[index].Play();
    }

    // 인덱스 실행 , 시작위치 , 회전
    public void EffectPlay(int index, float playTime, Transform StartPoint, Quaternion rot,bool isLoop)
    {

        var main = myEffect[index].main;
        main.loop = isLoop;

        myEffect[index].time = playTime;
        myEffect[index].transform.position = StartPoint.position;
        myEffect[index].transform.rotation = rot;
        myEffect[index].Play();
    }

    public void EffectPlay(int index, float playTime, Transform StartPoint, Quaternion rot,Vector3 size)
    {
        myEffect[index].time = playTime;
        
        myEffect[index].transform.position = StartPoint.position;
        myEffect[index].transform.rotation = rot;
        myEffect[index].Play();
    }

    #endregion

    #region 반복실행

    /// <summary>
    /// 일정 시간동안 반복실행
    /// </summary>
    public void EffectPlayT()
    {

    }

    IEnumerator EffectPlayerLoop(float time)
    {
        float duringTime = 0;
        while (duringTime < time)
        {
            duringTime += Time.deltaTime;

            yield return null;
        }
    }

    #endregion


    #region 전체실행

    public void EffectPlayAll()
    {
        for (int i = 0;i < myEffect.Count;i++)
        {
            myEffect[i].Play();
        }
    }

    public void EffectPlayAll(float playTime)
    {
        for (int i = 0; i < myEffect.Count; i++)
        {
            EffectPlay(i,playTime);
        }
    }

    public void EffectPlayAll(float playTime,Transform Target)
    {
        for (int i = 0; i < myEffect.Count; i++)
        {
            EffectPlay(i, playTime,Target,false);
        }
    }

    public void EffectPlayAll(float playTime,Transform Target , Quaternion Rot)
    {
        for (int i = 0; i < myEffect.Count; i++)
        {
            EffectPlay(i, playTime, Target, Rot);
        }
    }
    public void EffectPlayAll(float playTime, Transform Target, Quaternion Rot,Vector3 size)
    {
        for (int i = 0; i < myEffect.Count; i++)
        {
            EffectPlay(i, playTime, Target, Rot,size);
        }
    }

    // 플레이어를 인자로 받을경우
    public void EffectPlayAll(EffectPlayer curPlayer)
    {
        curPlayer.EffectPlayAll();
    }

    public void EffectPlayAll(EffectPlayer curPlayer, float playTime)
    {
        curPlayer.EffectPlayAll(playTime);
    }

    public void EffectPlayAll(EffectPlayer curPlayer, float playTime, Transform Target)
    {
        curPlayer.EffectPlayAll(playTime, Target);
    }

    public void EffectPlayAll(EffectPlayer curPlayer, float playTime, Transform Target, Quaternion Rot)
    {
        curPlayer.EffectPlayAll(playTime, Target, Rot);
    }
    public void EffectPlayAll(EffectPlayer curPlayer, float playTime, Transform Target, Quaternion Rot, Vector3 size)
    {
        curPlayer.EffectPlayAll(playTime, Target, Rot, size);
    }

    public void EffectPlayAll(int index)
    {
        myEffectPlayers[index].EffectPlayAll();
    }

    public void EffectPlayAll(int index, float playTime)
    {
        myEffectPlayers[index].EffectPlayAll(playTime);
    }

    public void EffectPlayAll(int index, float playTime, Transform Target)
    {
        myEffectPlayers[index].EffectPlayAll(playTime, Target);
    }

    public void EffectPlayAll(int index, float playTime, Transform Target, Quaternion Rot)
    {
        myEffectPlayers[index].EffectPlayAll(playTime, Target, Rot);
    }
    public void EffectPlayAll(int index, float playTime, Transform Target, Quaternion Rot, Vector3 size)
    {
        myEffectPlayers[index].EffectPlayAll(playTime, Target, Rot, size);
    }


    #endregion



    private void OnParticleTrigger()
    {
        foreach (var v in triggerTarget)
        {
            Debug.Log(v.name);
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        if (!triggerTarget.Contains(other.transform))
        {
            triggerTarget.Add(other.transform);
            PerformCollisionAction(other);       // 추가적인 충돌 처리
        }
    }

    // 충돌 오브젝트 로그
    public void PerformCollisionAction(GameObject collidedObject)
    {
        Debug.Log($"{ collidedObject.name} 충돌");
    }

    public void EffectEnd(int index,UnityAction e = null)
    {
        if (myEffect[index].isStopped)        {
            e?.Invoke();
            gameObject.SetActive(false);        // 이펙트가 종료되면 게임 오브젝트 비활성화
        }
    }

    public void EffectEndAll(UnityAction e = null)
    {
        for (int i = 0; i < myEffect.Count; i++)
        {
            EffectEnd(i, e);
        }
    }
 


}
