using UnityEngine;
using UnityEngine.UI;

public class LoadResultUI : MonoBehaviour
{
    private static LoadResultUI inst;
    // Eff-카메라 조정
    [SerializeField] private Camera EFFCamera;
    [SerializeField] private float _winFOV = 50f;
    [SerializeField] private float _loseFOV = 111f;

    // 결과 이미지
    public Image text;
    public Image t1;
    public Image t2;
    public Image BGImage1;
    public Image BGImage2;

    public Sprite[] textSprite;
    public Sprite[] t1Sprite;
    public Sprite[] t2Sprite;

    private void Awake()
    {
        if (inst != null)
        {
            Destroy(inst);
        }
        inst = this;
    }

    public static LoadResultUI GetInstance()
    {
        if (inst == null)
        {
            Logger.LogError("LoadResultUI 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return inst;
    }

    public void LoadResultBGM()
    {
        AudioManager.Inst.PlayBackgroundMusic("GameResultScene");
    }

    private void Start()
    {
        // 승/패/무승부
        LoadResultImg();
        //LoadUserName();
        LoadEff();
    }

    public void LoadResultImg(/*myHitScanner.Team BeaconTeam*/)
    {
        Team playerTeam = Gamemanager.Inst.currentPlayer.myTeam;      // 플레이어의 팀 정보 가져오기
        Team winningTeam = Gamemanager.Inst.VictoryTeam; // 승리팀 정보 가져오기

        // 승리
        if (winningTeam == playerTeam)
        {
            text.sprite = textSprite[0];
            t1.sprite = t1Sprite[0];
            t2.sprite = t2Sprite[0];
            BGImage1.color = new Color(51 / 255f, 63 / 255f, 94 / 255f, 1);
            BGImage2.color = new Color(41 / 255f, 44 / 255f, 60 / 255f, 1);
        }
        // 무승부
        else if (winningTeam == Team.NotSetting)
        {
            text.sprite = textSprite[1];
            t1.sprite = t1Sprite[1];
            t2.sprite = t2Sprite[1];
            BGImage1.color = new Color(51 / 255f, 63 / 255f, 94 / 255f, 1);
            BGImage2.color = new Color(41 / 255f, 44 / 255f, 60 / 255f, 1);
        }
        // 패배
        else  
        {
            text.sprite = textSprite[2];
            t1.sprite = t1Sprite[2];
            t2.sprite = t2Sprite[2];
            BGImage1.color = new Color(94 / 255f, 51 / 255f, 52 / 255f, 1);
            BGImage2.color = new Color(60 / 255f, 41 / 255f, 42 / 255f, 1);
        }
    }

    private void LoadUserName()
    {

    }

    private void LoadEff()
    {
        // EFF 카메라
        //EFFCamera = GetComponent<Camera>();   // 다른 씬에서 실행하면 null
        // 차안
        if (EFFCamera == null)
        {
            // "MyCamera"라는 태그를 가진 GameObject에서 Camera 컴포넌트를 찾음
            GameObject cameraGameObject = GameObject.FindGameObjectWithTag("EffCamera");
            if (cameraGameObject != null)
            {
                EFFCamera = GetComponent<Camera>();
            }
            else
            {
                Debug.LogError("카메라를 찾을 수 없습니다.");
            }
        }

        // Field of View 값을 변경
        Team playerTeam = Gamemanager.Inst.currentPlayer.myTeam;      // 플레이어의 팀 정보 가져오기
        Team winningTeam = Gamemanager.Inst.VictoryTeam;                  // 승리팀 정보 가져오기

        // 승리
        if (winningTeam == playerTeam)
        {
            EFFCamera.fieldOfView = _winFOV;
        }
        // 무승부
        else if (winningTeam == Team.NotSetting)
        {
            EFFCamera.fieldOfView = _winFOV;
        }
        // 패배
        else
        {
            EFFCamera.fieldOfView = _loseFOV;
        }
    }

    //myHitScanner.Team FindPlayerTeam()
    //{
    //    // 플레이어의 팀 정보 가져오기
    //    foreach (var player in Gamemanager.Inst.GetTeam(HitScanner.Team.RedTeam).Values)
    //    {
    //        if (player.gameObject == Gamemanager.Inst.currentPlayer.gameObject)
    //        {
    //            return myHitScanner.Team.RedTeam;
    //        }
    //    }
    //    return myHitScanner.Team.BlueTeam;
    //}


    /// <summary>
    /// 10초 후 로비
    /// </summary>
    public void ReturnToMatchRobby()
    {
        //if (fadeObject != null)
        //{
        //    fadeObject.ProcessFadeOut(() =>
        //    {
        //        GameManager.GetInstance().ChangeState(GameManager.GameState.MatchLobby);
        //    });
        //}
        //else
        //{
        //    GameManager.GetInstance().ChangeState(GameManager.GameState.MatchLobby);
        //}
    }
}
