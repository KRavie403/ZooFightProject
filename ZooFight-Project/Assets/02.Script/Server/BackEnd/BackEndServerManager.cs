using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System;
using static BackEnd.SendQueue;
using Cysharp.Threading.Tasks;
using System.Threading;

public class BackEndServerManager : MonoBehaviour
{
    private static BackEndServerManager instance;   // 인스턴스
    public bool isLogin { get; private set; }   // 로그인 여부
    public bool isConnected { get; private set; }   // 서버 연결 상태

    private string tempNickName;                        // 설정할 닉네임 (id와 동일)
    public string myNickName { get; private set; } = string.Empty;  // 로그인한 계정의 닉네임
    public string myIndate { get; private set; } = string.Empty;    // 로그인한 계정의 inDate
    public Action<bool, string> loginSuccessFunc = null;
    public Action<bool> OnConnectionStatusChanged; // 연결 여부 이벤트

    private CancellationTokenSource monitorCTS;

    private const string BackendError = "statusCode : {0}\nErrorCode : {1}\nMessage : {2}";

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        DontDestroyOnLoad(this.gameObject);
        //BackendSetup();
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

    private async void Start()
    {
        await InitializeBackendLoop();
        StartConnectionMonitorLoop();
    }

    private void Update()
    {
        //if (Backend.IsInitialized)
        //{
        //    Backend.AsyncPoll();
        //}
        Backend.AsyncPoll();
    }

    private async UniTask<bool> InitializeBackendLoop()
    {
        while (true)
        {
            var bro = Backend.Initialize(true);
            if (bro.IsSuccess())
            {
                Debug.Log("뒤끝 초기화 성공: " + bro);
                isConnected = true;
                OnConnectionStatusChanged?.Invoke(true);
                return true;
            }
            else
            {
                Debug.LogWarning("뒤끝 초기화 실패, 재시도 대기중: " + bro);
                isConnected = false;
                OnConnectionStatusChanged?.Invoke(false);
                await UniTask.Delay(TimeSpan.FromSeconds(5f));
            }
        }
    }

    //private void BackendSetup()
    //{
    //    var bro = Backend.Initialize(true);

    //    if (bro.IsSuccess())
    //    {
    //        Logger.Log("뒤끝 초기화 성공 : " + bro);
    //        isConnected = true;
    //    }
    //    else
    //    {
    //        Logger.Log("뒤끝 초기화 실패: " + bro);
    //        isConnected = false;
    //    }
    //}

    private void StartConnectionMonitorLoop()
    {
        monitorCTS?.Cancel();
        monitorCTS = new CancellationTokenSource();

        MonitorConnectionAsync(monitorCTS.Token).Forget();
    }

    private async UniTaskVoid MonitorConnectionAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            bool currentStatus = Backend.IsInitialized;

            if (isConnected != currentStatus)
            {
                isConnected = currentStatus;
                OnConnectionStatusChanged?.Invoke(isConnected);
                Debug.Log($"[BackEnd] 네트워크 상태 변경: {(isConnected ? "연결됨" : "끊김")}");
            }

            if (!isConnected)
            {
                await InitializeBackendLoop(); // 재초기화 재시도
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);
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
