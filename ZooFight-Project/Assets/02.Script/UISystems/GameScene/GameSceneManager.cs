using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Protocol;
using BackEnd.Tcp;
using BackEnd;
using Cysharp.Threading.Tasks;

public class GameSceneManager : MonoBehaviour
{
    private static GameSceneManager instance;

    public int curUser = 0;

    private SessionId index = 0;
    private string nickName = string.Empty;
    private int modelNum = 0;
    private int characterNum = 0;
    private bool isMe = false;


    // UI
    //public GameObject[] overlayImage;
    public GameObject matchingImage;
    public Image[] characterImages = new Image[6];                      // 캐릭터 프로필 이미지
    private Image[] modelImages = new Image[3];                         // 모델 이미지
    public Image[] characterStatImages = new Image[2];                // 게임 시작 후 상태 프로필 이미지
    public TMP_Text[] nameObjects = new TMP_Text[6];                // 유저 닉네임 텍스트
    public GameObject[] characterStats = new GameObject[2];     // 캐릭터 상태 
    public GameObject minimapUI;                                                // 미니맵

    //public GameObject hpObject;
    public GameObject startCountObject;
    public GameObject reconnectBoardObject;

    //[SerializeField] private Sprite[] _textSprite = new Sprite[2];
    [SerializeField] private string _character = "";         // 유저 캐릭터 종류

    private Text startCountText;
    private Text reconnectBoardText;
    const string HostOfflineMsg = "호스트와의 연결이 끊어졌습니다.\n연결 대기중";
    const string PlayerReconnectMsg = "{0} 플레이어 재접속중...";

    public static GameSceneManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("GameSceneManager 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }

    private void Awake()
    {
        if (matchingImage == null)
        {
            //matchingImage = GameObject.Find("matchingImage");
        }
        if (minimapUI == null)
        {
            minimapUI = GameObject.Find("MinimapUI");
        }
        if(characterStats[0] == null)
        {
            characterStats[0] = GameObject.Find("User0Stat");
        }
        if (characterStats[1] == null)
        {
            characterStats[1] = GameObject.Find("User1Stat");
        }

        startCountText = startCountObject.GetComponentInChildren<Text>();
        startCountObject.SetActive(true);

        GetUser(curUser);

#if DEBUG || UNITY_EDITOR
        Debug.Log("인게임 UI 설정 완료");
#endif

    }

    private void Start()
    {
        ToggleImagesAsync().Forget();
    }

    private void GetUser(int curUser)
    {
        // 유저 번호 불러오기
        UpdateCharacterUI(curUser);
    }

    public void SetPlayerProfile(int playerNum, string nickName, int modelNum)
    {
        this.nickName = nickName;
        this.modelNum = modelNum;

        nameObjects[playerNum - 1].text = nickName;
        characterImages[playerNum - 1] = modelImages[modelNum];
    }

    private void UpdateCharacterUI(int curUser)
    {
        switch (curUser)
        {
            case 0:
                minimapUI.SetActive(true);
                characterStats[0].SetActive(true);
                characterStats[1].SetActive(false);
                // Gamemanager에서 유저 1, 2의 캐릭터 종류 받아오는 코드 추가
                //_textSprite[0] = Resources.Load<Sprite>(_character);
                //_textSprite[1] = Resources.Load<Sprite>(_character);
                characterStatImages[0].sprite = Resources.Load<Sprite>(_character);
                characterStatImages[1].sprite = Resources.Load<Sprite>(_character);
                break;
            case 1:
                minimapUI.SetActive(false);
                characterStats[0].SetActive(false);
                characterStats[1].SetActive(true);
                // Gamemanager에서 유저 1의 캐릭터 종류 받아오는 코드 추가
                //_textSprite[0] = Resources.Load<Sprite>(_character);
                characterStatImages[2].sprite = Resources.Load<Sprite>(_character);
                break;
            case 2:
                minimapUI.SetActive(false);
                characterStats[0].SetActive(false);
                characterStats[1].SetActive(true);
                // Gamemanager에서 유저 2의 캐릭터 종류 받아오는 코드 추가
                //_textSprite[1] = Resources.Load<Sprite>(_character);
                characterStatImages[2].sprite = Resources.Load<Sprite>(_character);
                break;
            default:
                Debug.LogError("유저 번호가 할당되지 않았습니다.");
                break;
        }
    }

    private async UniTask ToggleImagesAsync()
    {
        // 게임 오브젝트 활성화
        //overlayImage[0].SetActive(true);
        //overlayImage[1].SetActive(true);
        matchingImage.SetActive(true);

        // 10초 대기
        await UniTask.Delay(10000);

        // 게임 오브젝트 비활성화
        //overlayImage[0].SetActive(false);
        //overlayImage[1].SetActive(false);
        matchingImage.SetActive(false);
    }

    public void SetStartCount(int time, bool isEnable = true)
    {
        startCountObject.SetActive(isEnable);
        if (isEnable)
        {
            if (time == 0)
            {
                startCountText.text = "시작";
            }
            else
            {
                startCountText.text = string.Format("{0}", time);
            }
        }
    }

    public void SetHostWaitBoard()
    {
        reconnectBoardText.text = HostOfflineMsg;
        reconnectBoardObject.SetActive(true);
        // 5초 후 재접속 메시지 닫음
        Invoke("ReconnectBoardClose", 5.0f);
    }

    public void SetReconnectBoard(string playerName)
    {
        reconnectBoardText.text = string.Format(PlayerReconnectMsg, playerName);
        reconnectBoardObject.SetActive(true);
        // 5초 후 재접속 메시지 닫음
        Invoke("ReconnectBoardClose", 5.0f);
    }

    private void ReconnectBoardClose()
    {
        reconnectBoardObject.SetActive(false);
    }
}
