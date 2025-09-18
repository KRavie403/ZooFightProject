using BackEnd.Tcp;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 아이템코드 부여 규칙
/// 1~9
/// 10~99
/// 100~999 플레이어
/// 1000~9999 아이템
/// 10000~99999 블럭
/// </summary>
interface IObjectId
{
    int ObjectId
    {
        get;
    }

    ObjectType ObjectType { get; }

}

public enum ObjectType : sbyte
{
    Character,
    Item,
    Effect,
    Block,
    ObjectDir,
    TypeCount
}

// 게임매니저가 두가지의 업데이트를 담당함
// 1.프레임간격의 업데이트
// 2.폴링레이트 간격의 업데이트
public class Gamemanager : MonoBehaviour
{

    private static Gamemanager inst;
    public static Gamemanager Inst => inst;

    #region Scene
    private const string LOGIN = "LoginScene";
    private const string LOBBY = "MainMenuScene";
    private const string READY = "LoadingScene";
    private const string INGAME = "GameScene";
    private const string RESULT = "ResultScene";
    private const string CREDIT = "CreditScene";
    #endregion

    #region Actions-Events
    //public static event Action OnRobby = delegate { };
    public static event Action OnGameReady = delegate { };
    //public static event Action OnGameStart = delegate { };
    public static event Action InGame = delegate { };
    public static event Action AfterInGame = delegate { };
    public static event Action OnGameOver = delegate { };
    public static event Action OnGameResult = delegate { };
    public static event Action OnGameReconnect = delegate { };
    public static event Action OnPollingRate = delegate { };

    private string asyncSceneName = string.Empty;
    private IEnumerator InGameUpdateCoroutine;

    private IEnumerator ClientUpdateCoroutine;

    public float PollingRate = 30.0f;

    public enum GameState { Login, MatchLobby, Ready, Start, InGame, Over, Result, Reconnect };
    private GameState gameState;
    #endregion

    #region 월드매니저로 이관할 정보
    // 현재 플레이중인 캐릭터의 정보
    public PlayerController currentPlayer;
    public int CharacterID = -1;

    public bool IsGameEnd { get; set; } = false;
    public bool IsGameEndSent { get; set; } = false;
    public bool IsGameStart { get; set; } = false;
    public Team VictoryTeam = Team.NotSetting;



    // 팀멤버 <Id,컴포넌트> 조합 
    public int MaxTeamMember = 3;
    public List<int> RedTeamId;
    public Dictionary<int, PlayerController> RedTeamPlayers;
    public List<int> BlueTeamId;
    public Dictionary<int, PlayerController> BlueTeamPlayers;

    public Dictionary<int, GameObject> SpawnPlayers;
    public Dictionary<int, GameObject> SpawnRedTeam;
    public Dictionary<int, GameObject> BlueTeam;
    public Dictionary<int, GameObject> SpawnItems;
    public Dictionary<int, GameObject> SpawnBlocks;
    /// <summary>
    /// 
    /// </summary>
    public List<int> RedTeamid;
    public List<int> BlueTeamid;


    public Dictionary<int, GameObject> SearchObjectType(ObjectType type)
    {
        Dictionary<int, GameObject> temps = new Dictionary<int, GameObject>();
        switch (type)
        {
            case ObjectType.Character:
                foreach (GameObject item in Players)
                {
                    temps.Add(item.GetComponent<IObjectId>().ObjectId, item);
                }
                break;
            case ObjectType.Item:
                break;
            case ObjectType.Effect:
                break;
            case ObjectType.Block:
                break;
            case ObjectType.ObjectDir:
                break;
            case ObjectType.TypeCount:
                break;
            default:
                break;
        }

        return null;
    }

    public Dictionary<int, GameObject> ActiveObjects;

    public List<GameObject> Players;

    public BlockObject RedTeamBlock;
    public BlockObject BlueTeamBlock;

    public Dictionary<int, GameObject> SpawnObject;

    #endregion
    //public WaitForSeconds BasicPollingRate

    public void Awake()
    {
        if (inst == null)
        {
            inst = FindObjectOfType<Gamemanager>();     // 게임 시작 시 자기 자신을 담음

            if (inst == null)
            {
                Debug.LogError("Gamemanager 인스턴스가 존재하지 않습니다.");
            }
        }
        if (inst != this)
        {
            Destroy(this.gameObject);
        }
        // 60프레임 고정
        Application.targetFrameRate = 60;
        // 게임중 슬립모드 해제
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        InGameUpdateCoroutine = InGameUpdate();

        DontDestroyOnLoad(this.gameObject);         // 씬 전환에 영향을 받지 않게 만듬
    }
    public static Gamemanager GetInstance()
    {
        if (inst == null)
        {
            Debug.LogError("Gamemanager 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return inst;
    }


    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.Login;
        //ClientUpdateCoroutine = PollingRateUpdate();
        //StartCoroutine(ClientUpdateCoroutine);

        currentPlayer = FindObjectOfType<PlayerController>();
    }

    float Times = 0;
    // Update is called once per frame
    void Update()
    {
        Times += Time.deltaTime;

    }


    private void FixedUpdate()
    {

    }

    IEnumerator InGameUpdate()
    {
        while (true)
        {
            if (gameState != GameState.InGame)
            {
                Debug.Log("Not InGame State");
                StopCoroutine(InGameUpdateCoroutine);
                yield return null;
            }
            InGame();
            AfterInGame();
            yield return new WaitForSeconds(.1f); //1초 단위
        }
    }

    public IEnumerator PollingRateUpdate()
    {
        float duringTime = 0;
        WaitForSeconds ws = new WaitForSeconds(1 / PollingRate);
        while (true)
        {
            //duringTime += Time.
            //Debug.Log(Times);
            OnPollingRate();
            yield return ws;
        }
    }

    #region scene
    private void Login()
    {
        // OnLogin();
        // ChangeScene(LOGIN);
    }

    private void MatchLobby(Action<bool> func)
    {
        if (func != null)
        {
            ChangeSceneAsync(LOBBY, func);
        }
        else
        {
            ChangeScene(LOBBY);
        }
    }

    private void GameReady()
    {
        Logger.Log("게임 레디 상태 돌입");
        //ChangeScene(READY);
        OnGameReady();
    }

    private void GameStart()
    {
        //delegate 초기화
        InGame = delegate { };
        AfterInGame = delegate { };
        OnGameOver = delegate { };
        OnGameResult = delegate { };

        // 게임씬이 로드되면 Start에서 OnGameStart 호출
        ChangeScene(INGAME);
    }

    private void GameOver()
    {
        OnGameOver();
    }

    private void GameResult()
    {
        OnGameResult();

        ChangeScene(LOBBY);
    }

    private void GameReconnect()
    {
        //delegate 초기화
        InGame = delegate { };
        AfterInGame = delegate { };
        OnGameOver = delegate { };
        OnGameResult = delegate { };

        OnGameReconnect();
        ChangeScene(INGAME);
        ChangeState(Gamemanager.GameState.InGame);
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public void ChangeState(GameState state, Action<bool> func = null)
    {
        gameState = state;
        switch (gameState)
        {
            case GameState.Login:
                Login();
                break;
            case GameState.MatchLobby:
                MatchLobby(func);
                break;
            case GameState.Ready:
                GameReady();
                break;
            case GameState.Start:
                GameStart();
                break;
            case GameState.Over:
                GameOver();
                break;
            case GameState.Result:
                GameResult();
                break;
            case GameState.InGame:
                // 코루틴 시작
                Debug.Log("Start InGameUpdate");
                StartCoroutine(InGameUpdateCoroutine);
                break;
            case GameState.Reconnect:
                GameReconnect();
                break;
            default:
                Debug.Log("알수없는 스테이트입니다. 확인해주세요.");
                break;
        }
    }

    public bool IsLobbyScene()
    {
        return SceneManager.GetActiveScene().name == LOBBY;
    }

    //private void ChangeScene(string scene)
    //{
    //    if (scene != LOGIN && scene != INGAME && scene != LOBBY && scene != READY)
    //    {
    //        Debug.Log("알수없는 씬 입니다.");
    //        return;
    //    }
    //    SceneManager.LoadScene(scene);
    //}
    public async UniTask ChangeScene(string scene)
    {
        if (scene != LOGIN && scene != INGAME && scene != LOBBY && scene != READY)
        {
            Debug.Log("알수없는 씬 입니다.");
            return;
        }

        if (scene == INGAME)
        {
            await LoadSceneAsync(scene);
        }
        else
        {
            SceneManager.LoadScene(scene);
        }
    }

    public async UniTask LoadSceneAsync(string scene)
    {
        LoadingProgressManager.Inst?.Show();

        var op = SceneManager.LoadSceneAsync(scene);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            await UniTask.Yield();
        }
        await UniTask.Delay(5000); // 약간 대기

        op.allowSceneActivation = true;

        while (!op.isDone)
            await UniTask.Yield();

        LoadingProgressManager.Inst?.Hide();
    }


    private void ChangeSceneAsync(string scene, Action<bool> func)
    {
        asyncSceneName = string.Empty;
        if (scene != LOGIN && scene != INGAME && scene != LOBBY && scene != READY)
        {
            Debug.Log("알수없는 씬 입니다.");
            return;
        }
        asyncSceneName = scene;

        StartCoroutine("LoadScene", func);
    }

    private IEnumerator LoadScene(Action<bool> func)
    {
        var asyncScene = SceneManager.LoadSceneAsync(asyncSceneName);
        asyncScene.allowSceneActivation = false;

        bool hasInvoked = false;

        while (asyncScene.progress < 0.9f)
        {
            func?.Invoke(false); // 로딩 중
            yield return null;
        }

        // progress가 0.9f 이상 도달한 후 (로드 완료)
        if (!hasInvoked)
        {
            hasInvoked = true;
            func?.Invoke(true);  // 로딩 완료 시점에 호출

            yield return new WaitForSeconds(20f);  // 5초 대기

            asyncScene.allowSceneActivation = true; // 씬 전환
        }
    }

    #endregion


    #region 정보인출(개선)

    public List<GameObject> GetTeams(Team team)
    {
        List<GameObject> teams = new List<GameObject>();
        switch (team)
        {
            case Team.RedTeam:
                RefreshRedTeam();
                foreach (var item in RedTeamPlayers)
                {
                    teams.Add(item.Value.GetComponent<GameObject>());
                }
                return teams;

                break;
            case Team.NotSetting:
                break;
            case Team.BlueTeam:
                RefreshBlueTeam();
                foreach (var item in BlueTeamPlayers)
                {
                    teams.Add(item.Value.GetComponent<GameObject>());
                }
                return teams;
                break;
            case Team.AllTarget:
                break;
            default:
                break;
        }

        return null;
    }

    public void RefreshPlayer()
    {
        Players.Clear();
        foreach (var item in ActiveObjects)
        {
            if ((item.Key >= 100) && (item.Key <= 999))
            {
                Players.Add(item.Value);
            }
        }

        SpawnPlayers = SpawnObject.Where(x => x.Key >= 100 && x.Key < 1000).ToDictionary(x => x.Key, x => x.Value);
    }

    public void RefreshRedTeam()
    {
        RedTeamPlayers.Clear();
        foreach (var item in Players)
        {
            PlayerController temp = item.GetComponent<PlayerController>();
            if (temp.myTeam == Team.RedTeam)
            {
                RedTeamPlayers.Add(item.GetComponent<IObjectId>().ObjectId, temp);
            }
        }
        SpawnRedTeam.Clear();
        SpawnRedTeam = SpawnObject.Where(x => x.Key >= 100 && x.Key < 1000).ToDictionary(x => x.Key, x => x.Value);
    }

    public void RefreshBlueTeam()
    {
        BlueTeamPlayers.Clear();
        foreach (var item in Players)
        {
            PlayerController temp = item.GetComponent<PlayerController>();
            if (temp.myTeam == Team.BlueTeam)
            {
                BlueTeamPlayers.Add(item.GetComponent<IObjectId>().ObjectId, temp);
            }
        }

        BlueTeam.Clear();
        BlueTeam = SpawnObject.Where(x => x.Key >= 100 && x.Key < 1000).ToDictionary(x => x.Key, x => x.Value);

    }

    public List<int> RedTeamIds()
    {
        return null;
        //return SpawnRedTeam.Where(); 
    }
    //public void Refresh

    public void RefreshItems()
    {
        SpawnItems.Clear();
        SpawnItems = SpawnObject.Where(x => (x.Key >= 1000) && (x.Key < 10000)).ToDictionary(x => x.Key, x => x.Value);
    }

    public void RefreshBlocks()
    {
        SpawnBlocks.Clear();
        SpawnBlocks = SpawnObject.Where(x => (x.Key >= 10000) && x.Key < 100000).ToDictionary(x => x.Key, x => x.Value);
    }

    #endregion

    #region  정보인출

    // 각 팀원 목록
    public Dictionary<int, PlayerController> GetTeam(Team team)
    {
        switch (team)
        {
            case Team.RedTeam:
                return RedTeamPlayers;
            case Team.BlueTeam:
                return BlueTeamPlayers;
            case Team.NotSetting:
                return null;
            default:
                return null;
        }
    }

    public List<PlayerController> GetTeamPlayers(Team team)
    {


        return null;
    }

    public Dictionary<int, PlayerController> GetEnemyTeam(Team team)
    {
        switch (team)
        {
            case Team.RedTeam:
                return BlueTeamPlayers;
            case Team.BlueTeam:
                return RedTeamPlayers;
            case Team.NotSetting:
                return null;
            default: return null;
        }
    }

    // 승패팀 팀원 id값
    public List<int> GetWinnerTeamId()
    {
        switch (VictoryTeam)
        {
            case Team.RedTeam:

            case Team.NotSetting:
            case Team.BlueTeam:
                return BlueTeamId;
            default:
                return null;
        }
    }
    public List<int> GetLoserTeamId()
    {
        switch (VictoryTeam)
        {
            case Team.RedTeam:
            //return BlueTeamId;
            case Team.NotSetting:
                return null;
            case Team.BlueTeam:
            //return RedTeamId;
            default:
                return null;
        }
    }
    // 지정팀 팀원 id값
    public List<int> GetTeamId(Team team)
    {
        switch (team)
        {
            case Team.RedTeam:
                return RedTeamId;
            case Team.NotSetting:
                return null;
            case Team.BlueTeam:
                return BlueTeamId;
            default:
                return null;
        }
    }

    public PlayerController PlayerIdToGetPlayerController(int playerId)
    {
        if (BlueTeamPlayers.ContainsKey(playerId))
        {
            return BlueTeamPlayers[playerId];
        }
        else if (RedTeamPlayers.ContainsKey(playerId))
        {
            return RedTeamPlayers[playerId];
        }
        else
        {
            return null;
        }
    }

    // 지정 팀 블럭
    public BlockObject GetTeamBlock(Team team)
    {
        switch (team)
        {
            case Team.RedTeam:
                if (RedTeamBlock != null) return RedTeamBlock;
                else return null;
            case Team.NotSetting:
                return null;
            case Team.BlueTeam:
                if (BlueTeamBlock != null) return BlueTeamBlock;
                else return null;
            case Team.AllTarget:
                return null;
            default:
                return null;
        }
    }
    public BlockObject GetEnemyBlock(Team team)
    {
        switch (team)
        {
            case Team.RedTeam:
                return BlueTeamBlock;
            case Team.NotSetting:
                return null;
            case Team.BlueTeam:
                return RedTeamBlock;
            case Team.AllTarget:
                return null;
            default:
                return null;
        }
    }
    public void AddBlockObj(BlockObject obj)
    {
        switch (obj.myTeam)
        {
            case Team.RedTeam:
                if (RedTeamBlock != null)
                {
                    RedTeamBlock = obj;
                }
                return;
            case Team.NotSetting:
                return;
            case Team.BlueTeam:
                if (BlueTeamBlock != null)
                {
                    BlueTeamBlock = obj;
                }
                return;
            case Team.AllTarget:
                return;
            default:
                return;
        }
    }
    #endregion



}