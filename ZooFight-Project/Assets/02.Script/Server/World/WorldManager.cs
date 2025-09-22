using BackEnd.Tcp;
using Protocol;
using DataScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class WorldManager : MonoBehaviour
{
    static public WorldManager Inst;

    const int START_COUNT = 10;
    const int GAME_TIMER = 12;    //1200

    private SessionId myPlayerIndex = SessionId.None;

    #region 플레이어
    public GameObject playerPool;
    public GameObject[] playerPrefeb;
    public int numOfPlayer = 0;
    //public GameObject particle;               // (수정)
    private const int MAXPLAYER = 6;
    public int alivePlayer { get; set; }
    //private Dictionary<SessionId, PlayerController> players;
    private Dictionary<SessionId, Player> players;  // (수정필요)
    private Dictionary<int, string> playersList;
    private Dictionary<int, int> playersModelNum;
    private Dictionary<SessionId, int> playerSessionId;
    private HashSet<SessionId> commanderSessions;
    public GameObject startPointObject;
    private List<Vector4> statringPoints;
    #endregion

    #region 결과값
    private List<int> escapeRecord;
    private int result = 0;         // 0: None, 1: Win, 2: Lose, 3: Draw
    private int winnerTeam;     // 0: Red, 1: Blue
    private int endTime;

    private Stack<SessionId> gameRecord;
    public delegate void PlayerDie(SessionId index);
    public PlayerDie dieEvent;

    #endregion

    void Awake()
    {
        Inst = this;
    }
    void Start()
    {
        InitializeGame();
        var matchInstance = BackEndMatchManager.GetInstance();
        if (matchInstance == null)
        {
            return;
        }
        if (matchInstance.isReconnectProcess)
        {
            GameSceneManager.GetInstance().SetStartCount(0, false);
            //GameSceneManager.GetInstance().SetReconnectBoard(BackEndServerManager.GetInstance().myNickName);
        }
    }

    /*
	 * 플레이어 설정
	 * 게임 상태 함수 설정
	 */
    public bool InitializeGame()
    {
        if (!playerPool)
        {
            Debug.Log("Player Pool Not Exist!");
            return false;
        }
        Debug.Log("게임 초기화 진행");
        gameRecord = new Stack<SessionId>();
        Gamemanager.OnGameOver += OnGameOver;
        Gamemanager.OnGameResult += OnGameResult;
        myPlayerIndex = SessionId.None;
        Gamemanager.Inst.IsGameEnd = false;
        Gamemanager.Inst.IsGameEndSent = false;
        //Gamemanager.Inst.IsGameStart = true;
        SetPlayerAttribute();
        OnGameStart();
        return true;
    }

    public void SetPlayerAttribute()
    {
        // 시작점
        statringPoints = new List<Vector4>();

        int num = startPointObject.transform.childCount;
        for (int i = 0; i < num; ++i)
        {
            var child = startPointObject.transform.GetChild(i);
            Vector4 point = child.transform.position;
            point.w = child.transform.rotation.eulerAngles.y;
            statringPoints.Add(point);
        }

        //dieEvent += PlayerDieEvent;
    }

    private void PlayerEndEvent(SessionId index) // 캐릭터 죽음 이벤트를 종료 이벤트로 바꿔야하나? <<
    {
        alivePlayer -= 1;
        //players[index].gameObject.SetActive(false);

        //파티클
        //var expObject = Instantiate(particle, players[index].GetPosition(), Quaternion.identity);
        //Destroy(expObject, 5);

        //InGameUiManager.GetInstance().SetScoreBoard(alivePlayer);
        gameRecord.Push(index);

        //Debug.Log(string.Format("Player Die : " + players[index].GetNickName()));

        // 호스트가 아니면 바로 리턴
        if (!BackEndMatchManager.GetInstance().IsHost())
        {
            return;
        }

        if (BackEndMatchManager.GetInstance().nowModeType == MatchModeType.TeamOnTeam)
        {
            //if (alivePlayer == 2)
            //{
            //    int remainTeamNumber = -1;
            //    SessionId remainSession = SessionId.None;
            //    foreach (var player in players)
            //    {
            //        if (player.Value.GetIsLive() == false)
            //        {
            //            continue;
            //        }
            //        if (remainTeamNumber == -1)
            //        {
            //            remainTeamNumber = BackEndMatchManager.GetInstance().GetTeamInfo(player.Key);
            //            remainSession = player.Key;
            //        }
            //        else if (remainTeamNumber == BackEndMatchManager.GetInstance().GetTeamInfo(player.Key))
            //        {
            //            //남은 플레이어들이 같은편이면 그대로 게임종료 메시지를 보냄
            //            gameRecord.Push(remainSession);
            //            gameRecord.Push(player.Key);
            //            SendGameEndOrder();
            //            return;
            //        }
            //    }
            //}
        }
        // 1명 이하로 플레이어가 남으면 바로 종료 체크
        if (alivePlayer <= 1)
        {
            SendGameEndOrder();
        }
    }

    /// <summary>
    /// 게임 종료 조건
    ///  1) 팀 블록이 먼저 탈출한 경우
    ///  2) 탈출구에 더 가까운 경우
    /// </summary>
    /// 
    private void OnEnable()
    {
        VictoryDecision.OnGameVictory += HandleGameVictory;
    }

    private void OnDisable()
    {
        VictoryDecision.OnGameVictory -= HandleGameVictory;
    }

    private void HandleGameVictory(Team winnerTeam)
    {
        Logger.Log($"게임 승리 팀: {winnerTeam}");

        // 서버에 게임 종료 전송
        SendGameEndOrder();
    }

    private void SendGameEndOrder()
    {
        if (Gamemanager.Inst.IsGameEndSent) return;
        Gamemanager.Inst.IsGameEndSent = true;

        // 게임 종료 전환 메시지는 호스트에서만 보냄
        Logger.Log("Make GameResult & Send Game End Order");
        List<SessionId> sessions = BackEndMatchManager.GetInstance().sessionIdList;

        Logger.Log($"!!GameResult: {result}");

        foreach (SessionId session in sessions)
        {
            //if(BackEndMatchManager.GetInstance().GetTeamInfo(session) ==)
            gameRecord.Push(session);
        }
        GameEndMessage message = new GameEndMessage(gameRecord);

        //GameEndMessage message = new GameEndMessage(result, winnerTeam, endTime, seesionList);
        BackEndMatchManager.GetInstance().SendDataToInGame<GameEndMessage>(message);
    }

    public SessionId GetMyPlayerIndex()
    {
        return myPlayerIndex;
    }

    public void SetPlayerInfo()
    {
        Logger.Log("SetPlayerInfo");
        if (BackEndMatchManager.GetInstance().sessionIdList == null)
        {
            // 현재 세션ID 리스트가 존재하지 않으면, 0.5초 후 다시 실행
            Invoke("SetPlayerInfo", 0.5f);
            return;
        }
        Logger.Log("세션 리스트 존재");
        var gamers = BackEndMatchManager.GetInstance().sessionIdList;
        int size = gamers.Count;
        if (size <= 0)
        {
            Logger.Log("No Player Exist!");
            return;
        }
        if (size > MAXPLAYER)
        {
            Logger.Log("Player Pool Exceed!");
            return;
        }

        //players = new Dictionary<SessionId, PlayerController>();
        players = new Dictionary<SessionId, Player>();
        playersList = new Dictionary<int, string>();
        playersModelNum = new Dictionary<int, int>();
        playerSessionId = new Dictionary<SessionId, int>();
        commanderSessions = new HashSet<SessionId>();
        BackEndMatchManager.GetInstance().SetPlayerSessionList(gamers);

        int index = 0;
        int startPointIndex = 0;
        int modelNum = BackendGameData.Inst.UserGameData.character;
        foreach (var sessionId in gamers)
        {
            var teamNumber = BackEndMatchManager.GetInstance().GetTeamInfo(sessionId);


            //임시
            var nick = BackEndMatchManager.GetInstance().GetNickNameBySessionId(sessionId);
            Logger.Log($"!sessionId: {index} : {sessionId}");
            Logger.Log($"!index: {index}, teamNumber:  {teamNumber}, nickname: {nick}");
            Logger.Log($"!sessionId startingPoint: {statringPoints.Count}");

            if (index >= MAXPLAYER) break;

#if 지시자보유
            //if (index == 0 || index == 3)
            //{
            //    commanderSessions.Add(sessionId);
            //}
#endif

            GameObject player = Instantiate(playerPrefeb[modelNum], new Vector3(statringPoints[index].x, statringPoints[index].y, statringPoints[index].z), Quaternion.identity, playerPool.transform);
            //players.Add(sessionId, player.GetComponent<PlayerController>());
            players.Add(sessionId, player.GetComponent<Player>());

            CameraController cam = player.GetComponentInChildren<CameraController>();

            if (BackEndMatchManager.GetInstance().IsMySessionId(sessionId))
            {
                Logger.Log($"!IsMySessionId: {sessionId}");

                myPlayerIndex = sessionId;
                int teamType = BackEndMatchManager.GetInstance().GetTeamInfo(sessionId);
                Team playerTeam = BackEndMatchManager.GetInstance().ConvertTeamNumberToEnum(teamType);

                players[sessionId].Initialize(true, myPlayerIndex, BackEndMatchManager.GetInstance().GetNickNameBySessionId(sessionId), statringPoints[index].w);
                Gamemanager.Inst.currentPlayer = FindObjectOfType<PlayerController>();

                //cam.EnableListener();   // 내 캐릭터 → 카메라 ON

                // 회의 이후 주석 해제
                //PlayerController.Inst.CharacterInitalize(playerTeam, myPlayerIndex, index);


                //var team = BackEndMatchManager.GetInstance().GetTeamInfo(sessionId);
                //var teamType = BackEndMatchManager.GetInstance().ConvertTeamNumberToEnum(team);
                //Gamemanager.Inst.currentPlayer.myTeam = teamType;
                //Logger.Log($"myTeamType: {teamType}");
            }
            else
            {
                Logger.Log($"!JustSessionId: {sessionId}");
                players[sessionId].Initialize(false, sessionId, BackEndMatchManager.GetInstance().GetNickNameBySessionId(sessionId), statringPoints[index].w);
               
                //cam.DisableListener();  // 다른 캐릭터 → 카메라 OFF
            }

            index++;

            Logger.Log($"!!num: {index} - modelId: {modelNum}");
            PlayerModelIdMessage msg = new PlayerModelIdMessage(sessionId, modelNum);
            Logger.Log("!!msg-modelId: " + msg.modelId);
            BackEndMatchManager.GetInstance().SendDataToInGame<PlayerModelIdMessage>(msg);

            playersList[index] = BackEndMatchManager.GetInstance().GetNickNameBySessionId(sessionId);
            playerSessionId[sessionId] = index;
            playersModelNum[index] = modelNum;
            Logger.Log($"!!playersModelNum[{index}] = {modelNum}");
            Logger.Log($"!!playerSessionId[{sessionId}]: index: {index} - nickname: {playersList[index]}");
        }

        // 필요없으면 지우기
#region players 0,3/1,2,4,5 분리

        //foreach (var sessionId in gamers)
        //{
        //    Logger.Log($"!sessionId: {index} : {sessionId}");
        //    Logger.Log($"!sessionId startingPoint: {statringPoints.Count}");

        //    Logger.Log($"Current index: {index}");
        //    if (index >= MAXPLAYER) break;

        //    Logger.Log($"num: {index + 1} - modelId: {modelNum}");
        //    PlayerModelIdMessage msg = new PlayerModelIdMessage(sessionId, modelNum);
        //    Logger.Log("!!msg-modelId: " + msg.modelId);
        //    BackEndMatchManager.GetInstance().SendDataToInGame<PlayerModelIdMessage>(msg);

        //    if (index == 0 || index == 3)
        //    {
        //        index++;
        //        commanderSessions.Add(sessionId);
        //        players.Add(sessionId, null); // null로 저장
        //        playersList[index] = BackEndMatchManager.GetInstance().GetNickNameBySessionId(sessionId);
        //        playerSessionId[sessionId] = index;
        //        Logger.Log($"playerSessionId[{sessionId}]: index: {index} - nickname: {playersList[index]}");
        //        continue;
        //    }

        //    GameObject player = Instantiate(playerPrefeb[modelNum], new Vector3(statringPoints[startPointIndex].x, statringPoints[startPointIndex].y, statringPoints[startPointIndex].z), Quaternion.identity, playerPool.transform);
        //    //players.Add(sessionId, player.GetComponent<PlayerController>());
        //    players.Add(sessionId, player.GetComponent<Player>());

        //    if (BackEndMatchManager.GetInstance().IsMySessionId(sessionId))
        //    {
        //        Logger.Log($"!IsMySessionId: {sessionId}");
        //        myPlayerIndex = sessionId;
        //        players[sessionId].Initialize(true, myPlayerIndex, BackEndMatchManager.GetInstance().GetNickNameBySessionId(sessionId), statringPoints[startPointIndex].w);
        //    }
        //    else
        //    {
        //        Logger.Log($"!JustSessionId: {sessionId}");
        //        players[sessionId].Initialize(false, sessionId, BackEndMatchManager.GetInstance().GetNickNameBySessionId(sessionId), statringPoints[startPointIndex].w);
        //    }

        //    index++;
        //    startPointIndex++;



        //    playersList[index] = BackEndMatchManager.GetInstance().GetNickNameBySessionId(sessionId);
        //    playerSessionId[sessionId] = index;
        //    playersModelNum[index] = modelNum;
        //    Logger.Log($"playersModelNum[{index}] = {modelNum}");
        //    Logger.Log($"playerSessionId[{sessionId}]: index: {index} - nickname: {playersList[index]}");
        //}
#endregion // <<<<필요없으면 지우기

        Logger.Log("Num Of Current Player : " + size);

        // 스코어 보드 설정
        alivePlayer = size;
        //InGameUiManager.GetInstance().SetScoreBoard(alivePlayer);
    }

    public void OnGameStart()
    {
        if (BackEndMatchManager.GetInstance() == null)
        {
            // 카운트 다운 : 종료
            //InGameUiManager.GetInstance().SetStartCount(0, false);
            return;
        }
        if (BackEndMatchManager.GetInstance().IsHost())
        {
            Logger.Log("플레이어 세션정보 확인");

            if (BackEndMatchManager.GetInstance().IsSessionListNull())
            {
                Logger.Log("Player Index Not Exist!");
                // 호스트 기준 세션데이터가 없으면 게임을 바로 종료한다.
                List<SessionId> seesionList = BackEndMatchManager.GetInstance().sessionIdList;

                GameEndMessage gameEndMessage = new GameEndMessage(gameRecord);
                //GameEndMessage gameEndMessage = new GameEndMessage(result, winnerTeam, endTime, seesionList);
                BackEndMatchManager.GetInstance().SendDataToInGame<GameEndMessage>(gameEndMessage);
                return;
            }
        }
        SetPlayerInfo();
    }

#region 타이머 설정
    public IEnumerator StartCount()
    {
        StartCountMessage msg = new StartCountMessage(START_COUNT);

        // 카운트 다운
        for (int i = 0; i < START_COUNT + 1; ++i)
        {
            msg.time = START_COUNT - i;
            BackEndMatchManager.GetInstance().SendDataToInGame<StartCountMessage>(msg);
            yield return new WaitForSeconds(1); //1초 단위
        }

        // 게임 시작 메시지를 전송
        GameStartMessage gameStartMessage = new GameStartMessage();
        BackEndMatchManager.GetInstance().SendDataToInGame<GameStartMessage>(gameStartMessage);

        StartCoroutine(GameTimer());
    }

    public IEnumerator GameTimer()
    {

        GameTimerMessage msg = new GameTimerMessage(GAME_TIMER);

        // 카운트 다운
        for (int i = 0; i < GAME_TIMER + 1; ++i)
        {
            if (Gamemanager.Inst.IsGameEnd) yield break;

            msg.time = GAME_TIMER - i;
            endTime = GAME_TIMER - i;
            BackEndMatchManager.GetInstance().SendDataToInGame<GameTimerMessage>(msg);
            yield return new WaitForSeconds(1); //1초 단위
        }

        if (Gamemanager.Inst.IsGameEnd) yield break;

        // 게임 타이머 메시지를 전송
        GameStartMessage gameTimerMessage = new GameStartMessage();
        BackEndMatchManager.GetInstance().SendDataToInGame<GameStartMessage>(gameTimerMessage);
    }

#endregion


    public void PreInGame()
    {
        foreach (var player in players)
        {
            player.Value.SetMoveVector(Vector3.zero);
        }
    }

    public void OnGameOver()
    {
        Logger.Log("Game End");
        if (BackEndMatchManager.GetInstance() == null)
        {
            Logger.LogError("매치매니저가 null 입니다.");
            return;
        }
        BackEndMatchManager.GetInstance().MatchGameOver(gameRecord);
    }

    public void OnGameEnd()
    {
        BackEndMatchManager.GetInstance().MatchEnd();
    }

    public void OnGameResult()
    {
        Debug.Log("Game Result");

        BackEndMatchManager.GetInstance().LeaveInGameRoom();

        if (Gamemanager.GetInstance().IsLobbyScene())
        {
            Gamemanager.GetInstance().ChangeState(Gamemanager.GameState.MatchLobby);
        }
    }

    public void OnRecieve(MatchRelayEventArgs args)
    {
        if (args.BinaryUserData == null)
        {
            Debug.LogWarning(string.Format("빈 데이터가 브로드캐스팅 되었습니다.\n{0} - {1}", args.From, args.ErrInfo));
            // 데이터가 없으면 그냥 리턴
            return;
        }
        Message msg = DataParser.ReadJsonData<Message>(args.BinaryUserData);
        BasicData bda = DataParser.ReadJsonData<BasicData>(args.BinaryUserData);
        if (msg == null)
        {
            return;
        }
        if (BackEndMatchManager.GetInstance().IsHost() != true && args.From.SessionId == myPlayerIndex)
        {
            return;
        }
        if (players == null)
        {
            Debug.LogError("Players 정보가 존재하지 않습니다.");
            return;
        }
        switch (msg.type)
        {
            case Protocol.Type.PlayerModelId:
                PlayerModelIdMessage modelIdMessage = DataParser.ReadJsonData<PlayerModelIdMessage>(args.BinaryUserData);
                Debug.Log("!!m" + modelIdMessage.modelId);
                ProcessPlayerData(modelIdMessage);
                break;
            case Protocol.Type.StartCount:
                StartCountMessage startCount = DataParser.ReadJsonData<StartCountMessage>(args.BinaryUserData);
                Logger.Log("wait second : " + (startCount.time));
                GameSceneManager.GetInstance().SetStartCount(startCount.time);
                break;
            case Protocol.Type.GameTimer:
                GameTimerMessage gameTimer = DataParser.ReadJsonData<GameTimerMessage>(args.BinaryUserData);
                GameSceneManager.GetInstance().SetGameTimer(gameTimer.time);
                break;
            case Protocol.Type.GameTimeOver:
                GameTimerMessage gameTimeOver = DataParser.ReadJsonData<GameTimerMessage>(args.BinaryUserData);
                result = 3;
                Logger.Log("확인: 타임 오버");
                SendGameEndOrder();
                break;
            case Protocol.Type.GameStart:
                GameSceneManager.GetInstance().SetStartCount(0, false);
                Gamemanager.GetInstance().ChangeState(Gamemanager.GameState.InGame);
                break;
            case Protocol.Type.GameEnd:
                GameEndMessage endMessage = DataParser.ReadJsonData<GameEndMessage>(args.BinaryUserData);
                Logger.Log("확인: 게임 오버");
                SetGameRecord(endMessage.count, endMessage.sessionList);
                Gamemanager.GetInstance().ChangeState(Gamemanager.GameState.Over);
                break;

            case Protocol.Type.Key:
                KeyMessage keyMessage = DataParser.ReadJsonData<KeyMessage>(args.BinaryUserData);
                ProcessKeyEvent(args.From.SessionId, keyMessage);
                break;
            //case Protocol.Type.PlayerMove:
            //    PlayerMoveMessage moveMessage = DataParser.ReadJsonData<PlayerMoveMessage>(args.BinaryUserData);
            //    ProcessPlayerData(moveMessage);
            //    break;
            case Protocol.Type.PlayerAttack:
                PlayerAttackMessage attackMessage = DataParser.ReadJsonData<PlayerAttackMessage>(args.BinaryUserData);
                ProcessPlayerData(attackMessage);
                break;
            case Protocol.Type.PlayerDamaged:
                PlayerDamegedMessage damegedMessage = DataParser.ReadJsonData<PlayerDamegedMessage>(args.BinaryUserData);
                ProcessPlayerData(damegedMessage);
                break;
            case Protocol.Type.PlayerNoMove:
                PlayerNoMoveMessage noMoveMessage = DataParser.ReadJsonData<PlayerNoMoveMessage>(args.BinaryUserData);
                ProcessPlayerData(noMoveMessage);
                break;
            //case DataScripts.Type.BlockData:
            //    BlockData_Class blockDataMessage = DataParser.ReadJsonData<BlockData_Class>(args.BinaryUserData);
            //    break;
            case Protocol.Type.GameSync:
                GameSyncMessage syncMessage = DataParser.ReadJsonData<GameSyncMessage>(args.BinaryUserData);
                ProcessSyncData(syncMessage);
                break;
            case Protocol.Type.BlockMove:
                BlockMoveMessage blockMoveMessage = DataParser.ReadJsonData<BlockMoveMessage>(args.BinaryUserData);
                Logger.Log($"{blockMoveMessage.xPos}");
                Logger.Log($"{blockMoveMessage.yPos}");
                Logger.Log($"{blockMoveMessage.zPos}");
                break;
            default:
                Logger.Log("Unknown protocol type");
                return;
        }
        switch (bda.type)
        {
            case DataScripts.DataTypes.CharacterData:
                CharacterData_Class moveMessage = DataParser.ReadJsonData<CharacterData_Class>(args.BinaryUserData);
                //ProcessPlayerData(moveMessage);
                break;
            case DataScripts.DataTypes.ItemData:
                break;
            case DataScripts.DataTypes.BlockData:
                break;
            default:
                Logger.Log("Unknown datascript type");

                return;
        }
        //switch (bda.type)
        //{
        //    case DataScripts.DataTypes.CharacterData:
        //        CharacterData_Class moveMessage = DataParser.ReadJsonData<CharacterData_Class>(args.BinaryUserData);
        //        ProcessPlayerData(moveMessage);
        //        break;
        //    case DataScripts.DataTypes.ItemData:
        //        break;
        //    case DataScripts.DataTypes.BlockData:
        //        break;
        //    default:
        //        Logger.Log("Unknown datascript type");
        //        return;
        //}
    }

    public void OnRecieveForLocal(KeyMessage keyMessage)
    {
        ProcessKeyEvent(myPlayerIndex, keyMessage);
    }

    public void OnRecieveForLocal(PlayerNoMoveMessage message)
    {
        ProcessPlayerData(message);
    }

    private void ProcessKeyEvent(SessionId index, KeyMessage keyMessage)
    {
        if (BackEndMatchManager.GetInstance().IsHost() == false)
        {
            //호스트만 수행
            return;
        }
        bool isMove = false;
        bool isAttack = false;
        bool isNoMove = false;

        int keyData = keyMessage.keyData;

        Vector3 moveVector = Vector3.zero;
        Vector3 attackPos = Vector3.zero;
        Vector3 playerPos = players[index].GetPosition();
        if ((keyData & KeyEventCode.MOVE) == KeyEventCode.MOVE)
        {
            moveVector = new Vector3(keyMessage.x, keyMessage.y, keyMessage.z);
            moveVector = Vector3.Normalize(moveVector);
            isMove = true;
        }
        //if ((keyData & KeyEventCode.ATTACK) == KeyEventCode.ATTACK)
        //{
        //    attackPos = new Vector3(keyMessage.x, keyMessage.y, keyMessage.z);
        //    players[index].Attack(attackPos);
        //    isAttack = true;
        //}

        if ((keyData & KeyEventCode.NO_MOVE) == KeyEventCode.NO_MOVE)
        {
            isNoMove = true;
        }

        if (isMove)
        {
            players[index].SetMoveVector(moveVector);
            PlayerMoveMessage msg = new PlayerMoveMessage(index, playerPos, moveVector);
            BackEndMatchManager.GetInstance().SendDataToInGame<PlayerMoveMessage>(msg);
        }
        if (isNoMove)
        {
            PlayerNoMoveMessage msg = new PlayerNoMoveMessage(index, playerPos);
            BackEndMatchManager.GetInstance().SendDataToInGame<PlayerNoMoveMessage>(msg);
        }
        //if (isAttack)
        //{
        //    PlayerAttackMessage msg = new PlayerAttackMessage(index, attackPos);
        //    BackEndMatchManager.GetInstance().SendDataToInGame<PlayerAttackMessage>(msg);
        //}
    }

    private void ProcessAttackKeyData(SessionId session, Vector3 pos)
    {
        //players[session].Attack(pos);
        PlayerAttackMessage msg = new PlayerAttackMessage(session, pos);
        BackEndMatchManager.GetInstance().SendDataToInGame<PlayerAttackMessage>(msg);
    }

    private void ProcessPlayerData(PlayerMoveMessage data)
    {
        if (BackEndMatchManager.GetInstance().IsHost() == true)
        {
            //호스트면 리턴
            return;
        }
        Vector3 moveVector = new Vector3(data.xDir, data.yDir, data.zDir);
        //moveVector가 같으면 방향 & 이동량 같으므로 적용 굳이 안함
        if (!moveVector.Equals(players[data.playerSession].moveVector))
        {
            players[data.playerSession].SetPosition(data.xPos, data.yPos, data.zPos);
            players[data.playerSession].SetMoveVector(moveVector);
        }
    }

    //private void ProcessPlayerData(CharacterData_Class data)
    //{
    //    //if (BackEndMatchManager.GetInstance().IsHost() == true)
    //    //{
    //    //    //호스트면 리턴
    //    //    return;
    //    //}
    //    //Vector3 moveVector = new Vector3(data.xDir, data.yDir, data.zDir);
    //    ////moveVector가 같으면 방향 & 이동량 같으므로 적용 굳이 안함
    //    //if (!moveVector.Equals(players[data.playerSession].moveVector))
    //    //{
    //    //    players[data.playerSession].SetPosition(data.xPos, data.yPos, data.zPos);
    //    //    players[data.playerSession].SetMoveVector(moveVector);
    //    //}
    //}
    private void ProcessPlayerData(PlayerNoMoveMessage data)
    {
        players[data.playerSession].SetPosition(data.xPos, data.yPos, data.zPos);
        players[data.playerSession].SetMoveVector(Vector3.zero);
    }


    private void ProcessPlayerData(PlayerAttackMessage data)
    {
        if (BackEndMatchManager.GetInstance().IsHost() == true)
        {
            //호스트면 리턴
            return;
        }
        players[data.playerSession].Attack(new Vector3(data.dir_x, data.dir_y, data.dir_z));
    }
    private void ProcessPlayerData(PlayerDamegedMessage data)
    {
        //players[data.playerSession].Damaged();
        //EffectManager.instance.EnableEffect(data.hit_x, data.hit_y, data.hit_z);
    }

    private void ProcessPlayerData(PlayerModelIdMessage data)
    {
        int index = playerSessionId[data.playerSession];

        GameSceneManager.GetInstance().SetPlayerProfile(index, playersList[index], data.modelId);
        GameSceneManager.GetInstance().GetUser(index, data.modelId);
        GameSceneManager.GetInstance().UpdateCharacterUI(index, playersModelNum);
    }

    private void ProcessSyncData(GameSyncMessage syncMessage)
    {
        // 플레이어 데이터 동기화
        int index = 0;
        if (players == null)
        {
            Debug.LogError("Player Poll is null!");
            return;
        }
        foreach (var player in players)
        {
            var y = player.Value.GetPosition().y;
            player.Value.SetPosition(new Vector3(syncMessage.xPos[index], y, syncMessage.zPos[index]));
            //player.Value.SetHP(syncMessage.hpValue[index]);
            index++;
        }
        BackEndMatchManager.GetInstance().SetHostSession(syncMessage.host);
    }

    public bool IsMyPlayerMove()
    {
        return players[myPlayerIndex].isMove;
    }

    public bool IsMyPlayerRotate()
    {
        return players[myPlayerIndex].isRotate;
    }

    private void SetGameRecord(int count, int[] arr)
    {
        gameRecord = new Stack<SessionId>();
        // 스택에 넣어야 하므로 제일 뒤에서 부터 스택에 push
        for (int i = count - 1; i >= 0; --i)
        {
            gameRecord.Push((SessionId)arr[i]);
        }
    }

    public GameSyncMessage GetNowGameState(SessionId hostSession)
    {
        int numOfClient = players.Count;

        float[] xPos = new float[numOfClient];
        float[] zPos = new float[numOfClient];
        float[] hp = new float[numOfClient];
        bool[] online = new bool[numOfClient];
        int index = 0;
        //foreach (var player in players)
        //{
        //    xPos[index] = player.Value.GetPosition().x;
        //    zPos[index] = player.Value.GetPosition().z;
        //    //hp[index] = player.Value.hp;
        //    index++;
        //}
        foreach (var player in players)
        {
            if (player.Value != null)
            {
                xPos[index] = player.Value.GetPosition().x;
                zPos[index] = player.Value.GetPosition().z;
                //hp[index] = player.Value.hp;
                //online[index] = true;
            }
            else
            {
                // 지시자 → 좌표는 0
                xPos[index] = 0f;
                zPos[index] = 0f;
                hp[index] = 0f;
                //online[index] = true;
            }
            index++;
        }
        return new GameSyncMessage(hostSession, numOfClient, xPos, zPos, hp, online);
    }

    public Vector3 GetMyPlayerPos()
    {
        return players[myPlayerIndex].GetPosition();
    }


}