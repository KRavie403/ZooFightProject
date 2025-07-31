using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    // 유저 닉네임
    [SerializeField] private TextMeshProUGUI _user1text;
    [SerializeField] private TextMeshProUGUI _user2text;
    [SerializeField] private TextMeshProUGUI _user3text;


    // 준비 버튼
    public GameObject readyBtn;
    // 준비 완료되면 체크 
    public GameObject checkBox;
    public GameObject check2Box;
    public GameObject check3Box;

    // 카운트다운
    [SerializeField] private bool[] _isPlayerReady = new bool[3]; // 플레이어 준비 상태 배열
    [SerializeField] private bool _allPlayersReady = false; // 모든 플레이어가 준비 상태인지
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime = 20f;
    [SerializeField] private float readyTime = 10f;
    [SerializeField] private bool _loadCall = true;


    private void Start()
    {
        LoadUserName();
        InitializeReadyState();
    }

    private void Update()
    {
        // 유저가 엔터(준비)를 눌렀을 경우 //나중에 없앨 예정
        HandleInput();
        HandleCountdown();
    }

    // 유저 닉네임 불러오기
    private void LoadUserName()
    {
        // 유저1의 닉네임
        //_user1text.text = ;
        // 유저2의 닉네임
        //_user2text.text = ;
        // 유저3의 닉네임
        //_user3text.text = ;
    }

    // 초기 준비 상태 전부 비활성화
    private void InitializeReadyState()
    {
        checkBox.SetActive(false);
        check2Box.SetActive(false);
        check3Box.SetActive(false);
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            ClickReady();
        }
    }

    private void HandleCountdown()
    {
        // 모든 플레이어가 준비되면 10초 카운트다운 시작
        if (CheckIfAllPlayersReady() && !_allPlayersReady)
        {
            StartCountdown();
        }
        // 카운트다운 진행
        if (remainingTime > 0)
        {
            timerText.color = !_allPlayersReady ? Color.white 
                : timerText.color = new Color(233 / 255f, 190 / 255f, 85 / 255f, 1);

            remainingTime -= Time.deltaTime;
        }
        else if (!_allPlayersReady)
        {
            remainingTime = 0;
            StartCountdown();
        }
        else
        {
            if (_loadCall)
            {
                LoadScene.Inst.OnStartGameButtonClicked();
                _loadCall = false;
            }
            //OnLoadScene().Forget();
        }
        timerText.text = $"{Mathf.CeilToInt(remainingTime):00}";
    }

    private void StartCountdown()
    {
        // 모든 플레이어가 준비 상태일 때
        _allPlayersReady = true;
        // 카운트다운 10초로 설정
        remainingTime = readyTime;
        checkBox.SetActive(true);
        check2Box.SetActive(true);
        check3Box.SetActive(true);
    }

    public void ClickReady()
    {
        //int n = 0; //임시
        for(int i = 0; i < _isPlayerReady.Length; i++)
        {
            _isPlayerReady[i] = true;
        }
        // 클릭 누른 해당 유저의 이미지만 활성화할 것
        checkBox.SetActive(true);
        check2Box.SetActive(true);
        check3Box.SetActive(true);
    }

    // 플레이어 준비 상태 체크
    private bool CheckIfAllPlayersReady()
    {
        foreach(bool isReady in _isPlayerReady)
        {
            if (!isReady) return false; // 하나라도 준비 상태가 아니라면 false
        }
        return true;
    }

    // 로딩신으로 전환
    private async UniTaskVoid OnLoadScene()
    {
        await UniTask.Yield();

        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("LoadingScene");
        loadSceneAsync.allowSceneActivation = false;

        while (!loadSceneAsync.isDone)
        {
            await UniTask.Yield();

            loadSceneAsync.allowSceneActivation = true;
        }
    }
}
