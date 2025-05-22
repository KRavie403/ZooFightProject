using BackEnd.Tcp;
using UnityEngine;
using System.Collections.Generic;

namespace Protocol
{
    // 이벤트 타입
    public enum Type : sbyte
    {
        Key = 0,        // 키(가상 조이스틱) 입력
        PlayerMove,     // 플레이어 이동
        PlayerRotate,   // 플레이어 회전
        PlayerAttack,   // 플레이어 공격
        PlayerDamaged,  // 플레이어 데미지 받음
        PlayerCrowdControl,     // 플레이어 군중제어기 받음
        PlayerDowned,   // 플레이어가 다운 됨
        PlayerNoMove,   // 플레이어 이동 멈춤
        PlayerNoRotate, // 플레이어 회전 멈춤
        PlayerHoldItem, // 플레이어가 아이템을 들고있는 상태.
        
        PlayerModelId,

        bulletInfo, 
        //ItemInfo,       //플레이어가 가지고 있는 아이템 정보

        ItemDrop,   //아이템 드랍
        ItemGet,    //아이템 얻음

        ImmediateUseItem,   //즉시 사용 아이템 사용
        ProjectileUseItem,     //투사체 아이템 사용

        BlockMove,      //블럭 이동
        BlockNoMove,    //블럭 이동 멈춤


        AIPlayerInfo,   // AI가 존재하는 경우 AI 정보
        LoadRoomScene,      // 룸 씬으로 전환
        LoadGameScene,      // 인게임 씬으로 전환
        StartCount,     // 시작 카운트
        GameStart,      // 게임 시작
        GameEnd,        // 게임 종료
        GameSync,       // 플레이어 재접속 시 게임 현재 상황 싱크
        Max
    }

    // 애니메이션 싱크는 사용하지 않습니다.
    /*
    public enum AnimIndex
    {
        idle = 0,
        walk,
        walkBack,
        stop,
        max
    }
    */

    //조이스틱 키 이벤트 코드
    public static class KeyEventCode
    {
        public const int NONE = 0;
        public const int MOVE = 1;      // 이동 메시지
        public const int ATTACK = 2;    // 공격 메시지
        public const int NO_MOVE = 4;   // 이동 멈춤 메시지
    }



    public class Message
    {
        public Type type;

        public Message(Type type)
        {
            this.type = type;
        }
    }

    public class KeyMessage : Message
    {
        public int keyData;
        public float x;
        public float y;
        public float z;

        public KeyMessage(int data, Vector3 pos) : base(Type.Key)
        {
            this.keyData = data;
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
        }
    }

    public class PlayerMoveMessage : Message
    {
        public SessionId playerSession;
        public float xPos;
        public float yPos;
        public float zPos;
        public float xDir;
        public float yDir;
        public float zDir;
        public PlayerMoveMessage(SessionId session, Vector3 pos, Vector3 dir) : base(Type.PlayerMove)
        {
            this.playerSession = session;
            this.xPos = pos.x;
            this.yPos = pos.y;
            this.zPos = pos.z;
            this.xDir = dir.x;
            this.yDir = dir.y;
            this.zDir = dir.z;
        }
    }

    public class PlayerNoMoveMessage : Message
    {
        public SessionId playerSession;
        public float xPos;
        public float yPos;
        public float zPos;
        public PlayerNoMoveMessage(SessionId session, Vector3 pos) : base(Type.PlayerNoMove)
        {
            this.playerSession = session;
            this.xPos = pos.x;
            this.yPos = pos.y;
            this.zPos = pos.z;
        }
    }

    public class PlayerAttackMessage : Message
    {
        public SessionId playerSession;
        public float dir_x;
        public float dir_y;
        public float dir_z;
        public PlayerAttackMessage(SessionId session, Vector3 pos) : base(Type.PlayerAttack)
        {
            this.playerSession = session;
            dir_x = pos.x;
            dir_y = pos.y;
            dir_z = pos.z;
        }
    }

    public class PlayerDamegedMessage : Message
    {
        public SessionId playerSession;
        public float hit_x;
        public float hit_y;
        public float hit_z;
        public PlayerDamegedMessage(SessionId session, float x, float y, float z) : base(Type.PlayerDamaged)
        {
            this.playerSession = session;
            this.hit_x = x;
            this.hit_y = y;
            this.hit_z = z;
        }
    }

    public class PlayerDownedMessage : Message
    {
        public SessionId playerSession;
        public float down_x;
        public float down_y;
        public float down_z;
        //enum 타입 추가
        public PlayerDownedMessage(SessionId session, float x, float y, float z) : base(Type.PlayerDowned)
        {
            this.playerSession = session;
            this.down_x = x;
            this.down_y = y;
            this.down_z = z;
        }
    }

    public class PlayerCrowdControlMessage : Message
    {
        public SessionId playerSession;
        public float hit_x;
        public float hit_y;
        public float hit_z;
        //enum 타입 추가
        public PlayerCrowdControlMessage(SessionId session, float x, float y, float z) : base(Type.PlayerCrowdControl)
        {
            this.playerSession = session;
            this.hit_x = x;
            this.hit_y = y;
            this.hit_z = z;
        }
    }

    public class ItemDrop : Message
    {
        public SessionId playerSession;
        public float xPos;
        public float yPos;
        public float zPos;
        public ItemDrop(SessionId session,Vector3 pos):base(Type.ItemDrop)
        {
            this.playerSession = session;
            this.xPos = pos.x;
            this.yPos= pos.y;
            this.zPos= pos.z;
        }
    }

    public class ItemGet : Message
    {
        public SessionId playerSession;
        public float xPos;
        public float yPos;
        public float zPos;
        public ItemGet(SessionId session, Vector3 pos) : base(Type.ItemGet)
        {
            this.playerSession = session;
            this.xPos = pos.x;
            this.yPos = pos.y;
            this.zPos = pos.z;
        }
    }

    public class ImmediateUseItem : Message
    {
        public SessionId playerSession;
        PlayerController player;
        public ImmediateUseItem(SessionId session,PlayerController p) : base(Type.ImmediateUseItem)
        {
            this.playerSession = session;
            this.player = p;
        }
    }

    public class ProjectileUseItem : Message
    {
        public SessionId playerSession;
        PlayerController player;
        public float xDes;
        public float yDes;
        public float zDes;
        public float xDir;
        public float yDir;
        public float zDir;
        public ProjectileUseItem(SessionId session, PlayerController p,Vector3 destination) : base(Type.ProjectileUseItem)
        {
            this.playerSession = session;
            this.player = p;
            this.xDes = destination.x;
            this.yDes = destination.y;
            this.zDes = destination.z;
        }
    }

    public class BlockMove : Message
    {
        public SessionId playerSession;
        public byte blockId; 
        public float xPos;
        public float yPos;
        public float zPos;
        public float xDir;
        public float yDir;
        public float zDir;
        public BlockMove(SessionId session,byte blockId,Vector3 pos,Vector3 dir) : base(Type.BlockMove)
        {
            this.playerSession = session;
            this.blockId = blockId;
            this.xPos = pos.x;
            this.yPos = pos.y;
            this.zPos = pos.z;
            this.xDir = dir.x;
            this.yDir = dir.y;
            this.zDir = dir.z;
        }
    }

    public class BlockNoMove : Message
    {
        public SessionId playerSession;
        public byte blockId;
        public float xPos;
        public float yPos;
        public float zPos;
        public BlockNoMove(SessionId session, byte blockId,Vector3 pos) : base(Type.BlockNoMove)
        {
            this.playerSession = session;
            this.blockId = blockId;
            this.xPos = pos.x;
            this.yPos = pos.y;
            this.zPos = pos.z;
        }
    }

    public class AIPlayerInfo : Message
    {
        public SessionId m_sessionId;
        public string m_nickname;
        public byte m_teamNumber;
        public int m_numberOfMatches;
        public int m_numberOfWin;
        public int m_numberOfDraw;
        public int m_numberOfDefeats;
        public int m_points;
        public int m_mmr;

        public AIPlayerInfo(MatchUserGameRecord gameRecord) : base(Type.AIPlayerInfo)
        {
            this.m_sessionId = gameRecord.m_sessionId;
            this.m_nickname = gameRecord.m_nickname;
            this.m_teamNumber = gameRecord.m_teamNumber;
            this.m_numberOfWin = gameRecord.m_numberOfWin;
            this.m_numberOfDraw = gameRecord.m_numberOfDraw;
            this.m_numberOfDefeats = gameRecord.m_numberOfDefeats;
            this.m_points = gameRecord.m_points;
            this.m_mmr = gameRecord.m_mmr;
            this.m_numberOfMatches = gameRecord.m_numberOfMatches;
        }

        public MatchUserGameRecord GetMatchRecord()
        {
            MatchUserGameRecord gameRecord = new MatchUserGameRecord();
            gameRecord.m_sessionId = this.m_sessionId;
            gameRecord.m_nickname = this.m_nickname;
            gameRecord.m_numberOfMatches = this.m_numberOfMatches;
            gameRecord.m_numberOfWin = this.m_numberOfWin;
            gameRecord.m_numberOfDraw = this.m_numberOfDraw;
            gameRecord.m_numberOfDefeats = this.m_numberOfDefeats;
            gameRecord.m_mmr = this.m_mmr;
            gameRecord.m_points = this.m_points;
            gameRecord.m_teamNumber = this.m_teamNumber;

            return gameRecord;
        }
    }

    public class LoadRoomSceneMessage : Message
    {
        public LoadRoomSceneMessage() : base(Type.LoadRoomScene)
        {

        }
    }

    public class LoadGameSceneMessage : Message
    {
        public LoadGameSceneMessage() : base(Type.LoadGameScene)
        {

        }
    }

    public class StartCountMessage : Message
    {
        public int time;
        public StartCountMessage(int time) : base(Type.StartCount)
        {
            this.time = time;
        }
    }

    public class GameStartMessage : Message
    {
        public GameStartMessage() : base(Type.GameStart) { }
    }

    public class GameEndMessage : Message
    {
        public int count;
        public int[] sessionList;
        public GameEndMessage(Stack<SessionId> result) : base(Type.GameEnd)
        {
            count = result.Count;
            sessionList = new int[count];
            for (int i = 0; i < count; ++i)
            {
                sessionList[i] = (int)result.Pop();
            }
        }
    }

    public class GameSyncMessage : Message
    {
        public SessionId host;
        public int count = 0;
        public float[] xPos = null;
        public float[] zPos = null;
        public float[] hpValue = null;
        public bool[] onlineInfo = null;

        public GameSyncMessage(SessionId host, int count, float[] x, float[] z, float[] hp, bool[] online) : base(Type.GameSync)
        {
            this.host = host;
            this.count = count;
            this.xPos = new float[count];
            this.zPos = new float[count];
            this.hpValue = new float[count];
            this.onlineInfo = new bool[count];

            for (int i = 0; i < count; ++i)
            {
                xPos[i] = x[i];
                zPos[i] = z[i];
                hpValue[i] = hp[i];
                onlineInfo[i] = online[i];
            }
        }
    }

    public class PlayerModelIdMessage : Message
    {
        public SessionId playerSession;
        public int modelId;

        public PlayerModelIdMessage(SessionId session, int num) : base(Type.PlayerModelId)
        {
            this.playerSession = session;
            this.modelId = num;
        }
    }
}
