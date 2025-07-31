using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

using Battlehub.Dispatcher;
using BackEnd;
using static BackEnd.SendQueue;

public partial class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager instance;

    // 플레이
    public GameObject playBtn;

    // 설정
    public Button settingsBtn;

    // 게임 종료
    public GameObject exitBtn;
    private bool _isESC = true;

    // 캐릭터 선택
    public GameObject switchBtn;
    public GameObject selectBtn;
    public GameObject[] arrowBtns;
    private bool _isSelect = true;

    // 매칭 유저
    public ToggleGroup tabObject;
    private TabUI[] matchInfotabList;

    // 매칭 이미지
    [SerializeField] private GameObject _matchingUI;
    [SerializeField] private GameObject _matchDoneObject;
    [SerializeField] private Animator _circleAnim;
    [SerializeField] private Animator _spinAnim;
    [SerializeField] private GameObject _uiRawImage;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _matchingText;

    private bool _isMatchmaking;
    private bool _isMatchDone;
    private bool _isReconnect;
    private CancellationTokenSource _cts;

    private Action<bool, string> loginSuccessFunc = null;
    private const string BackendError = "statusCode : {0}\nErrorCode : {1}\nMessage : {2}";

    public static MainMenuManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("MainMenuManager 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        // 재접속 로직 제외
        BackEndMatchManager.GetInstance().IsMatchGameActivate();
    }

    private void Start()
    {
        _isReconnect = false;
        _matchingText.text = "플레이";

        _matchingUI.SetActive(false);
        switchBtn.SetActive(true);
        selectBtn.SetActive(false);
        foreach (var btn in arrowBtns) btn.SetActive(false);
        settingsBtn.onClick.AddListener(OnSettingsButtonClick);

        matchInfotabList = tabObject.GetComponentsInChildren<TabUI>();
        //matchRecordTabList = recordObject.GetComponentsInChildren<TabUI>();
        int index = 0;
        foreach (var info in BackEndMatchManager.GetInstance().matchInfos)
        {
            matchInfotabList[index].SetTabText(info.title);
            matchInfotabList[index].index = index;
            //matchRecordTabList[index].SetTabText(info.title);
            //matchRecordTabList[index].index = index;
            index += 1;
        }

        for (int i = BackEndMatchManager.GetInstance().matchInfos.Count; i < matchInfotabList.Length; ++i)
        {
            matchInfotabList[i].gameObject.SetActive(false);
            //matchRecordTabList[i].gameObject.SetActive(false);
        }
    }

    private void OnSettingsButtonClick()
    {
        if (SettingsController.Inst != null)
        {
            SettingsController.Inst.ClickSetting();
        }
#if DEBUG
        else
        {
            Debug.LogWarning("SettingsController 인스턴스를 찾을 수 없습니다.");
        }
#endif
    }

    private void OnDestroy()
    {
        if (_cts != null)
        {
            _cts.Cancel();
        }
    }

    /// <summary>
    /// 게임 플레이
    /// </summary>
    public void ClickPlay()
    {
        if (_isMatchmaking)
        {
            StopMatchmaking();
        }
        else
        {
            StartMatchmaking();
        }
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void ClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                                Application.Quit();     // 어플리케이션 종료
#endif
    }

    /// <summary>
    /// 캐릭터 변경
    /// </summary>
    public void ClickSwitch()
    {
        switchBtn.SetActive(false);
        selectBtn.SetActive(true);
        foreach (var btn in arrowBtns) btn.SetActive(true);
    }

    /// <summary>
    /// 캐릭터 적용
    /// </summary>
    public void ClickSelect()
    {
        switchBtn.SetActive(true);
        selectBtn.SetActive(false);
        foreach (var btn in arrowBtns) btn.SetActive(false);
    }

    public async void StartMatchmaking()
    {
        _isMatchmaking = true;

        var matchManager = BackEndMatchManager.GetInstance();
        if (matchManager == null)
        {
            Debug.LogError("BackEndMatchManager instance is null.");
            return;
        }

        playBtn.GetComponent<Button>().interactable = false;

        if (matchManager.CreateMatchRoom())
        {
            SetLoadingObjectActive(true);
            _matchingText.text = "매칭 취소";
        }

        await UniTask.Delay(200);

        RequestMatch();

        await UniTask.Delay(1500);
        playBtn.GetComponent<Button>().interactable = true;
    }


    private async UniTask StartMatchmakingTimer()
    {
        _cts = new CancellationTokenSource();

        _uiRawImage.SetActive(true);
        _matchDoneObject.SetActive(false);

        int elapsedTime = 0;

        try
        {
            while (_isMatchmaking)
            {
                int minutes = elapsedTime / 60;
                int seconds = elapsedTime % 60;

                _timerText.text = $"{minutes:00}:{seconds:00}";

                await UniTask.Delay(TimeSpan.FromSeconds(1), DelayType.Realtime, PlayerLoopTiming.Update, _cts.Token);

                elapsedTime++;
            }
        }
        catch (OperationCanceledException)
        {
            Logger.Log("매칭 종료");
        }
    }

    // 매칭 실패
    public void MatchRequestCallback(bool result)
    {
        if (!result)
        {
            _uiRawImage.SetActive(true);
            _matchDoneObject.SetActive(false);
            _matchingText.text = "플레이";
            return; 
        }
    }

    // 매칭 잡힐 때 호출
    public void MatchDoneCallback()
    {
        _isMatchDone = true;
        _uiRawImage.SetActive(false);
        _matchDoneObject.SetActive(true);
    }


    // 매칭을 종료할 때 호출
    public async void StopMatchmaking()
    {
        if (!_isMatchmaking)
            return;

        _cts.Cancel();
        _cts?.Dispose();
        _cts = null;

        playBtn.GetComponent<Button>().interactable = false;

        if (_isMatchmaking && !_isMatchDone) {
            BackEndMatchManager.GetInstance().CancelRegistMatchMaking();
            BackEndMatchManager.GetInstance().LeaveMatchRoom();
        }
            
        else if (_isMatchmaking && _isMatchDone)
            BackEndMatchManager.GetInstance().LeaveInGameRoom();

        _isMatchmaking = false;
        _isMatchDone = false;


        SetLoadingObjectActive(false);
        _matchingText.text = "플레이";

        await UniTask.Delay(1500);
        playBtn.GetComponent<Button>().interactable = true;
    }

    public void SetLoadingObjectActive(bool isActive)
    {
        _matchingUI.SetActive(isActive);
        LoadCircleImg();

        if (isActive)
        {
            StartMatchmakingTimer().Forget();   
        }
    }

    // 원형 로딩 이미지
    public void LoadCircleImg()
    {
        _circleAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        if (!_circleAnim.GetBool("IsRotating"))
        {
            _circleAnim.SetBool("IsRotating", true);
        }
    }

    // 회오리 로딩 이미지
    public void LoadSpinImg()
    {
        _circleAnim.updateMode = AnimatorUpdateMode.UnscaledTime;
        if (!_circleAnim.GetBool("IsRotating"))
        {
            _circleAnim.SetBool("IsRotating", true);
        }
    }


    public void OpenGameRecord()
    {
        //if (loadingObject.activeSelf || errorObject.activeSelf || requestProgressObject.activeSelf || matchDoneObject.activeSelf || reconnectObject.activeSelf)
        //{
        //    return;
        //}
        GetGameRecord();
    }

    public void GetGameRecord()
    {
        //loadingObject.SetActive(true);

        //int index = -1;
        //foreach (var tab in matchRecordTabList)
        //{
        //    if (tab.IsOn() == true)
        //    {
        //        index = tab.index;
        //        break;
        //    }
        //}

        //if (index < 0)
        //{
        //    Debug.Log("활성화된 탭이 없습니다.");
        //    return;
        //}

        //BackEndMatchManager.GetInstance().GetMyMatchRecord(index, (MatchRecord record, bool isSuccess) =>
        //{
        //    Dispatcher.Current.BeginInvoke(() =>
        //    {
                //loadingObject.SetActive(false);
                //recordObject.SetActive(true);

                //if (fillWinChart != null)
                //{
                //    StopCoroutine(fillWinChart);
                //}
                //if (fillLoseChart != null)
                //{
                //    StopCoroutine(fillLoseChart);
                //}
                //if (countingWinRate != null)
                //{
                //    StopCoroutine(countingWinRate);
                //}

                //recordContent[0].text = record.matchTitle;
                //recordContent[2].text = record.matchType.ToString();
                //recordContent[3].text = record.modeType.ToString();

                //recordContent[1].text = "0%";
                //recordContent[4].text = "-";
                //recordContent[5].text = "0";
                //recordContent[6].text = "0";

                //recordChart[0].fillAmount = (float)0;
                //recordChart[1].fillAmount = 0;

                //if (isSuccess == false)
                //{
                //    // 조회 실패
                //    SetErrorObject("매칭 기록 조회에 실패하였습니다.\n\n잠시 후 다시 시도해주세요.");
                //    return;
                //}

                //if (record.win == -1)
                //{
                //    // 매칭 기록이 없음
                //    SetErrorObject("매칭 기록이 존재하지 않습니다.\n\n해당 매칭을 먼저 시도해주세요.");
                //    return;
                //}

                //recordContent[4].text = record.score;
                //recordContent[5].text = record.win.ToString();
                //recordContent[6].text = (record.numOfMatch - record.win).ToString();
                //float winRate = (float)record.win / record.numOfMatch;
                //fillWinChart = StartCoroutine(FillPieChart(recordChart[0], winRate));
                //fillLoseChart = StartCoroutine(FillPieChart(recordChart[1], 1 - winRate));
                //countingWinRate = StartCoroutine(FillWinRate((int)record.winRate));
        //    });
        //});
    }

    public void SetErrorObject(string error)
    {
        //errorObject.SetActive(true);
        //errorText.text = error;
    }

    public void EnableReconnectObject()
    {
        Dispatcher.Current.BeginInvoke(() =>
        {
            SetReconnectObject().Forget();
        });
    }

    private async UniTaskVoid SetReconnectObject()
    {
        await UniTask.Delay(1000); // 1초 대기

        _isReconnect = true;
        _matchingText.text = "경기 재접속하기";
    }

    public void ReconnectInGameProcess()
    {
        if (_isReconnect)
        {
            MatchDoneCallback();
            BackEndMatchManager.GetInstance().ProcessReconnect();

            _isReconnect = false;
        }
    }

    public void JoinMatchProcess()
    {
        BackEndMatchManager.GetInstance().JoinMatchServer();
    }

    public void ChangeTab()
    {
        int index = 0;
        foreach (var tab in matchInfotabList)
        {
            if (tab.IsOn() == true)
            {
                break;
            }
            index += 1;
        }
        var matchInfo = BackEndMatchManager.GetInstance().matchInfos[index];
        //matchInfoText.text = string.Format(matchInfoStr, matchInfo.headCount, matchInfo.isSandBoxEnable.Equals(true) ? "활성화" : "비활성화",
            //matchInfo.matchType, matchInfo.matchModeType);
    }

    public void RequestMatch(int index)
    {
        //if (loadingObject.activeSelf || recordObject.activeSelf || errorObject.activeSelf || requestProgressObject.activeSelf || matchDoneObject.activeSelf)
        //{
        //    return;
        //}

        // 매칭 요청 보내기
        foreach (var tab in matchInfotabList)
        {
            if (tab.IsOn() == true)
            {
                BackEndMatchManager.GetInstance().RequestMatchMaking(tab.index);
                return;
            }
        }

        Logger.Log("활성화된 탭이 존재하지 않습니다.");

        // 로딩 씬을 비동기적으로 로드하고, 이후 게임 씬을 로드
        //LoadLoadingScene().Forget();
    }

    public void RequestGroupMatch(int index)
    {
        //if (loadingObject.activeSelf || recordObject.activeSelf || errorObject.activeSelf || requestProgressObject.activeSelf || matchDoneObject.activeSelf)
        //{
        //    return;
        //}

        //// 현재 매칭 인원 확인 (예: 그룹 매칭 필요)
        //int requiredPlayers = int.Parse(matchInfos[index].headCount);
        //int currentPlayers = GetCurrentMatchPlayers(matchInfos[index].matchType);

        //if (currentPlayers < requiredPlayers)
        //{
        //    Debug.Log("매칭을 시작하기에 필요한 인원이 부족합니다.");
        //    return;
        //}

        // 매칭 요청 보내기
        foreach (var tab in matchInfotabList)
        {
            if (tab.IsOn() == true)
            {
                BackEndMatchManager.GetInstance().RequestMatchMaking(tab.index);
                return;
            }
        }

        Logger.Log("활성화된 탭이 존재하지 않습니다.");

        // 로딩 씬을 비동기적으로 로드하고, 이후 게임 씬을 로드
        //LoadLoadingScene().Forget();
    }

    //private void ClearReadyUserList()
    //{
    //    readyUserList = new List<string>();
    //    var parent = readyUserListParent.transform;

    //    while (parent.childCount > 0)
    //    {
    //        var child = parent.GetChild(0);
    //        GameObject.DestroyImmediate(child.gameObject);
    //    }
    //}

    public void OnMatchLeave(string reason)
    {
        Logger.Log($"매칭 이탈 감지: {reason}");

        StopMatchmaking();

        // 자동 재매칭 조건
        if (reason == "상대방 나감" || reason.Contains("disconnect"))
        {
            RetryMatchmaking(); // 자동 재매칭 시도
        }
    }

    private async void RetryMatchmaking()
    {
        Logger.Log("자동 재매칭 시도 중...");

        await UniTask.Delay(TimeSpan.FromSeconds(1)); // 약간의 딜레이 후 재시도

        StartMatchmaking();
    }

}
