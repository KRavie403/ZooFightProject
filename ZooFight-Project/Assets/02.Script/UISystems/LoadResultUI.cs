using BackEnd;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BackEnd.Tcp;

public class LoadResultUI : MonoBehaviour
{
    private static LoadResultUI inst;

    // Eff-카메라 조정
    [SerializeField] private Camera EFFCamera;
    [SerializeField] private float _winFOV = 50f;
    [SerializeField] private float _loseFOV = 111f;

    public GameObject eff1;
    public GameObject eff2;

    // 결과 이미지
    public Image text;
    public Image t1;
    public Image t2;
    public Image[] t1Img;
    public Image[] t2Img;

    public Sprite[] textSprite;
    public Sprite[] redSprite;
    public Sprite[] blueSprite;

    public TMP_Text[] t1UserName;
    public TMP_Text[] t2UserName;

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

    private void Start()
    {
        eff1.SetActive(false);
        eff2.SetActive(false);
    }

    public void LoadResultBGM()
    {
        AudioManager.Inst.PlayBackgroundMusic("GameResultScene");
    }


    public void LoadResultImg(/*myHitScanner.Team BeaconTeam*/)
    {
        Logger.Log("playerTeam: LoadResultImg 실행");

        var sessionId = Backend.Match.GetMySessionId();
        var team = BackEndMatchManager.GetInstance().GetTeamInfo(sessionId);
        Team playerTeam = BackEndMatchManager.GetInstance().ConvertTeamNumberToEnum(team);

        Team winningTeam = Gamemanager.Inst.VictoryTeam;                   // 승리팀 정보 가져오기
 
        Logger.Log($"playerTeam:: playerTeam: {playerTeam}");
        Logger.Log($"playerTeam:: winningTeam: {winningTeam}");

        //Team playerTeam = Gamemanager.Inst.currentPlayer.myTeam;      // 플레이어의 팀 정보 가져오기


        // 승리
        if (winningTeam == playerTeam)
        {
            text.sprite = textSprite[0];
            if (playerTeam == Team.RedTeam)
            {
                t1.sprite = redSprite[0];
                t2.sprite = blueSprite[2];
                
                t1Img[0].color = new Color(0.549f, 0.086f, 0.122f, 1f);
                t1Img[1].color = new Color(0.427f, 0.067f, 0.094f, 1f);
                t1Img[2].color = new Color(0.361f, 0.059f, 0.078f, 1f);

                t2Img[0].color = new Color(0.059f, 0.118f, 0.294f, 1f);
                t2Img[1].color = new Color(0.075f, 0.141f, 0.349f, 1f);
                t2Img[2].color = new Color(0.176f, 0.251f, 0.463f, 1f);
            }
            else if (playerTeam == Team.BlueTeam)
            {
                t1.sprite = blueSprite[0];
                t2.sprite = redSprite[2];

                t1Img[0].color = new Color(0.176f, 0.251f, 0.463f, 1f);
                t1Img[1].color = new Color(0.075f, 0.141f, 0.349f, 1f);
                t1Img[2].color = new Color(0.059f, 0.118f, 0.294f, 1f);

                t2Img[0].color = new Color(0.361f, 0.059f, 0.078f, 1f);
                t2Img[1].color = new Color(0.427f, 0.067f, 0.094f, 1f);
                t2Img[2].color = new Color(0.549f, 0.086f, 0.122f, 1f);
            }
        }
        // 무승부
        else if (winningTeam == Team.NotSetting)
        {
            text.sprite = textSprite[1];
            t1.sprite = blueSprite[1];
            t2.sprite = redSprite[1];

            t1Img[0].color = new Color(0.176f, 0.251f, 0.463f, 1f);
            t1Img[1].color = new Color(0.067f, 0.157f, 0.427f, 1f);
            t1Img[2].color = new Color(0.059f, 0.137f, 0.361f, 1f);

            t2Img[0].color = new Color(0.361f, 0.059f, 0.078f, 1f);
            t2Img[1].color = new Color(0.427f, 0.067f, 0.094f, 1f);
            t2Img[2].color = new Color(0.549f, 0.086f, 0.122f, 1f);
        }
        // 패배
        else  
        {
            text.sprite = textSprite[2];
            if (playerTeam == Team.RedTeam)
            {
                t1.sprite = blueSprite[0];
                t2.sprite = redSprite[2];

                t1Img[0].color = new Color(0.176f, 0.251f, 0.463f, 1f);
                t1Img[1].color = new Color(0.075f, 0.141f, 0.349f, 1f);
                t1Img[2].color = new Color(0.059f, 0.118f, 0.294f, 1f);

                t2Img[0].color = new Color(0.361f, 0.059f, 0.078f, 1f);
                t2Img[1].color = new Color(0.427f, 0.067f, 0.094f, 1f);
                t2Img[2].color = new Color(0.549f, 0.086f, 0.122f, 1f);
            }
            else if (playerTeam == Team.BlueTeam)
            {
                t1.sprite = redSprite[0];
                t2.sprite = blueSprite[2];

                t1Img[0].color = new Color(0.549f, 0.086f, 0.122f, 1f);
                t1Img[1].color = new Color(0.427f, 0.067f, 0.094f, 1f);
                t1Img[2].color = new Color(0.361f, 0.059f, 0.078f, 1f);

                t2Img[0].color = new Color(0.059f, 0.118f, 0.294f, 1f);
                t2Img[1].color = new Color(0.075f, 0.141f, 0.349f, 1f);
                t2Img[2].color = new Color(0.176f, 0.251f, 0.463f, 1f);
            }
        }
    }

    public void LoadUserName(MatchGameResult matchGameResult)
    {
        Logger.Log("playerTeam: LoadUserName 실행");

        Team winningTeam = Gamemanager.Inst.VictoryTeam;

        var matchInstance = BackEndMatchManager.GetInstance();

        string nickName = "";

        if (winningTeam == Team.NotSetting)
        {
            for (int i = 0; i < matchGameResult.m_draws.Count / 2; i++)
            {
                nickName = matchInstance.GetNickNameBySessionId(matchGameResult.m_draws[i]);
                t1UserName[i].text = nickName;
                nickName = matchInstance.GetNickNameBySessionId(matchGameResult.m_draws[i + 3]);
                t2UserName[i].text = nickName;
            }
        }
        else
        {
            for (int i = 0; i < matchGameResult.m_winners.Count; i++)
            {
                nickName = matchInstance.GetNickNameBySessionId(matchGameResult.m_winners[i]);
                t1UserName[i].text = nickName;
            }


            for (int i = 0; i < matchGameResult.m_losers.Count; i++)
            {
                nickName = matchInstance.GetNickNameBySessionId(matchGameResult.m_losers[i]);
                t2UserName[i].text = nickName;
            }
        }
    }

    public void LoadEff()
    {
        Logger.Log("playerTeam: LoadEff 실행");
        eff1.SetActive(true);
        eff2.SetActive(true);

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
        //Team playerTeam = Gamemanager.Inst.currentPlayer.myTeam;      // 플레이어의 팀 정보 가져오기
        //Team winningTeam = Gamemanager.Inst.VictoryTeam;                  // 승리팀 정보 가져오기


        var sessionId = Backend.Match.GetMySessionId();
        var team = BackEndMatchManager.GetInstance().GetTeamInfo(sessionId);
        Team playerTeam = BackEndMatchManager.GetInstance().ConvertTeamNumberToEnum(team);
        Team winningTeam = Gamemanager.Inst.VictoryTeam;                   // 승리팀 정보 가져오기


        // 승리
        if (winningTeam == playerTeam)
        {

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
    }

    private void CanvasGroupOn(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
    private void CanvasGroupOff(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
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

}
