using BackEnd.Tcp;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace DataScripts
{

    #region 게임 

    /// <summary>
    /// 생성된 게임 룸의 정보
    /// </summary>
    public struct GameInfo
    {

        public int GameId;

        public CharacterData[] PlayerInfo;
        public int myPlayerNum;


        public bool isHost;
        public float PlayTIme;

        public SessionId SessionId;


    }


    /// <summary>
    /// 사용자의 아이덴티티
    /// 유저의 이름 , ip  등 고유정보를 가짐
    /// </summary>
    public struct PlayerInfo
    {

        public string PlayerName;
        public int PlayerId;

        public string PlayerIP;
        public bool isSeverConnect;

        /// <summary>
        /// 생성된 게임 내의 플레이어 번호
        /// -1 = 게임밖 , 0 = 호스트 , 1 ~ N = 플레이어 넘버
        /// </summary>
        public int PlayerNum;
        public void InsertPlayerInfo()
        {

        }
    }


    /// <summary>
    /// 게임 외적으로 서버와 교환할 정보
    /// 호스트 -> 서버 전송
    /// </summary>
    public struct SeverData
    {
        //public PlayerInfo myPlayer;

        public PlayerInfo[] GamePlayerList;

    }

    #region 캐릭터 데이터

    /// <summary>
    /// 게임 내부에서 호스트와 교환할 정보
    /// 
    /// </summary>
    public struct CharacterData
    {
        public PlayerInfo myPlayer;



        public int ModelId;

        public PlayerController myController;
        // 캐릭터의 상태변화값
        public PlayerController.pState dirState;

        public float curHp;

        public float curStamina;

        public float CurSp;
        public bool isShield;

        public BlockObject myBlock;
        public bool isGrab;

        public bool isGameStart;
        public bool isSuperArmor;
        public bool isAbleMove;
        public bool isCrashed;
        public bool isKeyReverse;
        public bool isDenial;

        public ItemCode curItem;

        public bool isMoving;
        public bool isRunning;
        public bool isJump;



        public Vector3 curPos;
        public Vector3 curRot;


        public void SetDir(Vector3 pos, Vector3 rot, bool isStatic = true)
        {
            this.curPos = pos;
            this.curRot = rot;
        }

    }


    public struct CharacterStatus
    {
        public float curHp;

        public float curStamina;

        public float CurSp;
        public bool isShield;
    }

    public struct CharacterMovement
    {
        // 캐릭터의 목표지점
        public Vector3 dirPos;
        public bool isDynamic;
        // 캐릭터의 목표회전값
        public quaternion dirRot;
    }

    #endregion

    public struct ItemData
    {
        public ItemCode itemCode;

        /// <summary>
        /// 아이템의 목적지
        /// Zero = 비 이동형 아이템
        /// </summary>
        public Vector3 dirPos;
        public Team curTeam;
        public PlayerController ItemOwner;

    }



    #region 블럭 데이터

    public struct TeamBlockData
    {
        public NormalBlockdata curData;


    }

    public struct NormalBlockdata
    {
        public Team curTeam;

        public Vector3 curPos;

        public bool isMoving;
        public bool isGrab;
        public Vector3 dirPos;
    }

    #endregion


    #endregion


    #region 데이터 클래스버전

    public enum DataTypes
    {
        CharacterData = 0,
        GameData,
        ItemData,
        BlockData,
        PlayerData,
        PlayerBasicData,
        PlayerState,
        Types
    }


    /// <summary>
    /// 데이터의 형식을 담는 클래스
    /// </summary>
    public class BasicData
    {
        public DataTypes type;

        public BasicData(DataTypes type)
        {
            this.type = type;
        }
    }


    public class GameData_Class : BasicData
    {
        public PlayerInfo playerInfo;

        public SessionId SessionId;
        public GameData_Class(SessionId SessionId) : base(DataTypes.GameData)
        {
            this.SessionId = SessionId;
        }

        public int GameId;

        public CharacterData[] PlayerInfo;
        public int myPlayerNum;


        public bool isHost;
        public float PlayTIme;



    }





    /// <summary>
    /// 사용자의 정보를 담는 클래스
    /// 사용자의 id , 대기실 , 인게임 정보를 담음
    /// </summary>
    public class PlayerProfiles_Class : BasicData
    {
        public PlayerInfo_Class myPlayer;


        bool isGameStart = false;


        public CharacterData_Class mycharacter
        {
            get
            {
                if (isGameStart)
                {
                    return mycharacter;
                }
                else
                {
                    return null;
                }
            }
        }

        public PlayerProfiles_Class(PlayerInfo_Class playerInfo) : base(DataTypes.PlayerData)
        {
            this.myPlayer = playerInfo;
        }
    }


    public class PlayerInfo_Class
    {
        public string PlayerName;
        public int PlayerId;

        public string PlayerIP;
        public bool isSeverConnect;

        public SessionId sessionId;



        public void InsertPlayerInfo()
        {

        }
    }

    /// <summary>
    /// 플레이어의 위치, 이동, 회전 등 무브먼트에 관한 정보를 담는 클래스
    /// 
    /// </summary>
    public class CharacterBasicSetting : BasicData
    {
        public CharacterData_Class myPlayer;

        // 가지고있는 아이템
        public ItemCode curItem;


        public CharacterBasicSetting(CharacterBasicSetting setData) : base(DataTypes.PlayerBasicData)
        {

        }
    }

    /// <summary>
    /// 플레이어의 스테이트머신에서의 상태변화 및 각종 상태에 관한 정보를 담는 클래스
    /// </summary>
    public class CharacterState : BasicData
    {

        public CharacterState() : base(DataTypes.PlayerState)
        {

        }
    }

    /// <summary>
    /// 게임 내부에서 호스트와 교환할 정보
    /// 
    /// </summary>
    public class CharacterData_Class : BasicData
    {

        public CharacterData_Class(CharacterBasicSetting basicData) : base(DataTypes.CharacterData)
        {

            BasicData = basicData;

        }


        public CharacterBasicSetting BasicData;

        public int ModelId;

        public PlayerController myController;

        // 캐릭터의 상태변화값
        public PlayerController.pState dirState;


        float curHp;
        public float SetHp(float hp)
        {
            curHp = hp;
            return curHp;
        }

        float curStamina;
        public float SetStamina(float stamina)
        {
            curStamina = stamina;
            return curStamina;
        }

        public float CurSp;
        public bool isShield;

        public BlockObject myBlock;
        public bool isGrab;

        public bool isGameStart;
        public bool isSuperArmor;
        public bool isAbleMove;
        public bool isCrashed;
        public bool isKeyReverse;

        public ItemCode curItem;

        public bool isMoving;
        public bool isRunning;
        public bool isJump;


        public Vector3 curPos;
        public Vector3 curRot;


        public void SetDir(Vector3 pos, Vector3 rot, bool isStatic = true)
        {
            this.curPos = pos;
            this.curRot = rot;
        }

    }

    public class ItemData_Class : BasicData
    {
        public SessionId playerSession;
        public ItemCode itemCode;
  

        public ItemData_Class(ItemCode itemCode) : base(DataTypes.ItemData)
        {
            this.itemCode = itemCode;
        }


        /// <summary>
        /// 아이템의 목적지
        /// Zero = 비 이동형 아이템                  
        /// </summary>
        public Vector3 dirPos;
        public Team curTeam;
        public PlayerController ItemOwner;


    }

    public class BlockData_Class : BasicData
    {
        public Team curTeam;

        public BlockData_Class(Team curTeam) : base(DataTypes.BlockData) 
        {
            this.curTeam = curTeam;
        }

        public Vector3 curPos;

        public bool isMoving;
        public bool isGrab;
        public Vector3 dirPos;


    }

#endregion
}