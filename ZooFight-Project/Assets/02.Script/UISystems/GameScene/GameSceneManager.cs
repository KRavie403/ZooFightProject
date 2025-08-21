using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Protocol;
using BackEnd.Tcp;
using BackEnd;
using Cysharp.Threading.Tasks;
using UnityEngine.EventSystems;
using System;

public class GameSceneManager : MonoBehaviour
{
    static private GameSceneManager instance;

    public int curUser = 0;

    private SessionId index = 0;
    private string nickName = string.Empty;
    private int modelNum = 0;
    private int characterNum = 0;
    private bool isMe = false;
    private bool _isGameStart = false;


    // UI
    //public GameObject[] overlayImage;
    public GameObject matchingImage;
    public Image[] characterImages = new Image[6];                      // 캐릭터 프로필 이미지
    public Sprite[] modelImages = new Sprite[6];                           // 모델 이미지
    public Image[] characterStatImages = new Image[2];                // 게임 시작 후 상태 프로필 이미지
    public TMP_Text[] nameObjects = new TMP_Text[6];                // 유저 닉네임 텍스트
    public GameObject[] characterStats = new GameObject[2];     // 캐릭터 상태 
    public GameObject minimapUI;                                                // 미니맵

    //public GameObject hpObject;
    public GameObject startCountObject;
    public GameObject gameTimerObject;
    public GameObject gameResultObject;
    public GameObject gameResultBlur;
    public GameObject reconnectBoardObject;

    //[SerializeField] private Sprite[] _textSprite = new Sprite[2];
    [SerializeField] private int _character = 0;         // 유저 캐릭터 종류

    private TMP_Text startCountText;
    private TMP_Text gameTimerText;
    private TMP_Text reconnectBoardText;
    const string HostOfflineMsg = "호스트와의 연결이 끊어졌습니다.\n연결 대기중";
    const string PlayerReconnectMsg = "{0} 플레이어 재접속중...";
    const string PlayerExitMsg = "{0}님이 나갔습니다";


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        if (matchingImage == null)
        {
            matchingImage = GameObject.Find("MatchingImages");
        }
        if (minimapUI == null)
        {
            minimapUI = GameObject.Find("MinimapUI");
        }
        if (characterStats[0] == null)
        {
            characterStats[0] = GameObject.Find("User0Stat");
        }
        if (characterStats[1] == null)
        {
            characterStats[1] = GameObject.Find("User1Stat");
        }

        startCountText = startCountObject.GetComponentInChildren<TMP_Text>();
        gameTimerText = gameTimerObject.GetComponentInChildren<TMP_Text>();
        startCountObject.SetActive(true);
        gameTimerObject.SetActive(true);
        reconnectBoardText = reconnectBoardObject.GetComponentInChildren<TMP_Text>();

        Logger.Log($"인게임 UI 설정 완료");

        //GetUser(curUser);
    }

    public static GameSceneManager GetInstance()
    {
        if (instance == null)
        {
            Logger.LogError("GameSceneManager 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }

    private void Start()
    {
        ToggleImagesAsync().Forget();

    }

    public void GetUser(int curUser, int modelNum)
    {
        Logger.Log($"GetUser");
        // 유저 번호 불러오기
        //UpdateCharacterUI(curUser);
        // 유저 모델 번호 불러오기
        _character = modelNum;
    }

    public void SetPlayerProfile(int playerNum, string nickName, int modelNum)
    {
        nameObjects[playerNum - 1].text = nickName;

        int spriteIndex = (playerNum % 2 == 0) ? modelNum + 3 : modelNum;
        characterImages[playerNum - 1].sprite = modelImages[spriteIndex];
        
        Logger.Log($"!!playerNum: {playerNum} name: {nickName} modelNum: {modelNum} cI: {playerNum-1} spIndex: {spriteIndex}");
    }


    /// <summary>
    /// 1번 유저 - 맵X / 2번, 3번 유저 - 맵O
    /// </summary>
    /// <param name="curUser"></param>
    public void UpdateCharacterUI(int curUser, Dictionary<int, int> modelNum)
    {
        Debug.Log($"!! curUser: {curUser} ModelNum: {modelNum[curUser]}");

        switch (curUser % 3)
        {
            case 0:     // 3번 유저
                minimapUI.SetActive(true);
                characterStats[0].SetActive(true);
                characterStats[1].SetActive(false);
                //_textSprite[0] = Resources.Load<Sprite>(_character);
                characterStatImages[0].sprite = modelImages[modelNum[3]];
                break;
            case 1:     // 1번 유저
                minimapUI.SetActive(false);
                characterStats[0].SetActive(true);
                characterStats[1].SetActive(true);
                //_textSprite[0] = Resources.Load<Sprite>(_character);
                //_textSprite[1] = Resources.Load<Sprite>(_character);
                characterStatImages[0].sprite = modelImages[modelNum[2]];
                characterStatImages[1].sprite = modelImages[modelNum[3]];
                break;
            case 2:     // 2번 유저
                minimapUI.SetActive(true);
                characterStats[0].SetActive(true);
                characterStats[1].SetActive(false);
                //_textSprite[1] = Resources.Load<Sprite>(_character);
                characterStatImages[0].sprite = modelImages[modelNum[2]];
                break;
            default:
                Debug.LogError("유저 번호가 할당되지 않았습니다.");
                break;
        }
    }

    private async UniTask ToggleImagesAsync()
    {
        matchingImage.SetActive(true);

        await UniTask.Delay(TimeSpan.FromSeconds(10), DelayType.Realtime);

        matchingImage.SetActive(false);

        if (BackEndMatchManager.GetInstance().IsHost())
        {
            StartCoroutine(WorldManager.instance.StartCount());
            await UniTask.Delay(TimeSpan.FromSeconds(10));
            StartCoroutine(WorldManager.instance.GameTimer());
        }
    }

    public void SetStartCount(int time, bool isEnable = true)
    {
        Logger.Log($"시작하는 시간까지: {time}초");
        startCountObject.SetActive(isEnable);
        if (isEnable)
        {
            if (time == 0)
            {
                startCountText.color = new Color32(255, 187, 0, 255);
                startCountText.text = "시작!";

                HideAfterDelay().Forget();
            }
            else
            {
                if (time == 3) AudioManager.Inst.PlayBasicEffect(0);
                startCountText.text = string.Format("{0}", time);
            }
        }
    }

    private async UniTaskVoid HideAfterDelay()
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(1));
        startCountObject.SetActive(false);
    }

    public void SetGameTimer(float time, bool isEnable = true)
    {
                
        Logger.Log($"남은 시간: {time}초");
        gameTimerObject.SetActive(isEnable);

        if (!isEnable) return;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        // 0초일 경우 강제로 0 처리
        if (Mathf.Approximately(time, 0f) || time <= 0f)
        {
            minutes = 0;
            seconds = 0;
            gameTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            int result = 3;     // draw = 3

            GameTimeOverMessage gameTimeOverMessage = new GameTimeOverMessage(result);
            BackEndMatchManager.GetInstance().SendDataToInGame<GameTimeOverMessage>(gameTimeOverMessage);

        }
        else if (time == 3)
        {
            // 촉박한 BGM 처리
            // AudioManager.Inst.PlayBasicEffect(0);
            gameTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            gameTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        }
    }

    public void SetHostWaitBoard()
    {
        reconnectBoardText.text = HostOfflineMsg;
        reconnectBoardObject.SetActive(true);
        // 5초 후 재접속 메시지 닫음
        ReconnectBoardClose().Forget();
    }

    public void ShowResultBase()
    {
        Logger.Log("playerTeam: ShowResultBase 실행");
        gameResultBlur.SetActive(true);
        gameResultObject.SetActive(true);
    }

    public void SetExitBoard(string playerName)
    {
        reconnectBoardObject.SetActive(true);
        reconnectBoardText.text = string.Format(PlayerExitMsg, playerName);
    }

    public void SetReconnectBoard(string playerName)
    {
        reconnectBoardText.text = string.Format(PlayerReconnectMsg, playerName);
        // 5초 후 재접속 메시지 닫음
        ReconnectBoardClose().Forget();
    }

    private async UniTaskVoid ReconnectBoardClose()
    {
        await UniTask.Delay(5000); // 5초 대기
        reconnectBoardObject.SetActive(false);
    }
}