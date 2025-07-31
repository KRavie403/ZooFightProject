using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;
using static BackEnd.SendQueue;

public class BackEndServerManager : MonoBehaviour
{
    private static BackEndServerManager instance;   // 인스턴스
    public bool isLogin { get; private set; }   // 로그인 여부

    private string tempNickName;                        // 설정할 닉네임 (id와 동일)
    public string myNickName { get; private set; } = string.Empty;  // 로그인한 계정의 닉네임
    public string myIndate { get; private set; } = string.Empty;    // 로그인한 계정의 inDate
    public Action<bool, string> loginSuccessFunc = null;

    private const string BackendError = "statusCode : {0}\nErrorCode : {1}\nMessage : {2}";

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        DontDestroyOnLoad(this.gameObject);
        BackendSetup();
    }

    public static BackEndServerManager GetInstance()
    {
        if (instance == null)
        {
#if UNITY_EDITOR || DEBUG
            Debug.LogError("BackEndServerManager 인스턴스가 존재하지 않습니다.");
#endif
            return null;
        }

        return instance;
    }

    void Start()
    {
        isLogin = false;

//        var bro = Backend.Initialize(true);

//        if (bro.IsSuccess())
//        {
//#if UNITY_EDITOR || DEBUG
//            Debug.Log($"뒤끝 초기화 성공 : {bro} ");
//#endif
//        }
//        else
//        {
//#if UNITY_EDITOR || DEBUG
//            Debug.LogError($"뒤끝 초기화 실패 : {bro}");
//#endif
//        }

    }

    private void Update()
    {
        //if (Backend.IsInitialized)
        //{
        //    Backend.AsyncPoll();
        //}
        Backend.AsyncPoll();
    }

    private void BackendSetup()
    {
        var bro = Backend.Initialize(true);

        if (bro.IsSuccess())
        {
#if UNITY_EDITOR || DEBUG
            Debug.Log("뒤끝 초기화 성공 : " + bro);
#endif
        }
        else
        {
#if UNITY_EDITOR || DEBUG
            Debug.LogError("뒤끝 초기화 실패: " + bro);
#endif
        }
    }


    // 유저 정보 불러오기 사전작업
    private void OnPrevBackendAuthorized()
    {
        isLogin = true;

        OnBackendAuthorized();
    }

    // 실제 유저 정보 불러오기
    public void OnBackendAuthorized()
    {
        Enqueue(Backend.BMember.GetUserInfo, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError("유저 정보 불러오기 실패\n" + callback);
                loginSuccessFunc(false, string.Format(BackendError,
                callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
                return;
            }
            Debug.Log("유저정보\n" + callback);

            var info = callback.GetReturnValuetoJSON()["row"];
            if (info["nickname"] == null)
            {
                //LoginUI.GetInstance().ActiveNickNameObject();
                return;
            }
            myNickName = info["nickname"].ToString();
            myIndate = info["inDate"].ToString();

            Debug.Log("loginSuccessFunc: " + loginSuccessFunc);
            if (loginSuccessFunc != null)
            {
                BackEndMatchManager.GetInstance().GetMatchList(loginSuccessFunc);
            }
        });
    }


}
