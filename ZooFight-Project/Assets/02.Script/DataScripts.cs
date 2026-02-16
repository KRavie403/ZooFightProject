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


    public class GameData : BasicData
    {


        public SessionId SessionId;
        public GameData(SessionId SessionId) : base(DataTypes.GameData)
        {
            this.SessionId = SessionId;
        }

        public int GameId;

        public PlayerProfiles[] PlayerInfo;


        // 호스트의 여부를 불값이 아닌 ID값을 기준으로 변경
        public SessionId hostId;
        public bool isHost;


        public float PlayTIme;

    }





    /// <summary>
    /// 사용자의 정보를 담는 클래스
    /// 사용자의 id , 대기실 , 인게임 정보를 담음
    /// </summary>
    public class PlayerProfiles : BasicData
    {
        
        public SessionId sessionId;

        public PlayerProfiles(SessionId sessionid) : base(DataTypes.PlayerData)
        {
            sessionId = sessionid;
        }

        bool isGameStart = false;

        public string PlayerName;
        public string PlayerIP;


        public int ModelId;

        public bool isSeverConnect;

        public int PlayerId;

        public CharacterData mycharacter
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


    }


    /// <summary>
    /// 플레이어의 위치, 이동, 회전 등 무브먼트에 관한 정보를 담는 클래스
    /// 
    /// </summary>
    public class C_MovementData : BasicData
    {

        public SessionId playerId;
        public C_MovementData(SessionId playerID) : base(DataTypes.PlayerBasicData)
        {
            playerId = playerID;
        }


        #region 이동 위치관련 데이터
        public Vector3 curPos;

        public Vector3 curRot;
        public float curRotX;
        public float curRotZ;

        public Vector3 curAxis;
        public float curDist;

        #endregion

    }


    /// <summary>
    /// 게임 내부에서 호스트와 교환할 정보
    /// 
    /// </summary>
    public class CharacterData : BasicData
    {

        public CharacterData(SessionId PlayerId) : base(DataTypes.CharacterData)
        {

            playerSession = PlayerId;

        }


        public SessionId playerSession;
        // 세션 id 값으로의 

        // 캐릭터의 상태변화값


        #region 스테이터스 데이터
        public float curHp;
        public float SetHp(float hp)
        {
            curHp = hp;
            return curHp;
        }

        public float curStamina;
        public float SetStamina(float stamina)
        {
            curStamina = stamina;
            return curStamina;
        }

        // 오브젝트 id 값으로의 변환 필요
        public BlockObject myBlock;
        public int blockid;

        public ItemCode curItem;

        #endregion

        #region 애매한 데이터



        #endregion


        #region 삭제할 데이터

        #endregion

    }

    public class ItemData : BasicData
    {
        public ItemCode itemCode;

        public int itemID;

        public ItemData(int itemId) : base(DataTypes.ItemData)
        {
            itemID = itemId;
        }

      
        /// <summary>
        /// 아이템의 목적지
        /// Zero = 비 이동형 아이템                  
        /// </summary>
        public Vector3 dirPos;
        
        public Team curTeam;
        public SessionId playerSession;

        //  중복동작일지 구별 필요

    }

    public class BlockData : BasicData
    {
        public Team curTeam;
        public int blockid;

        public BlockData(int blockId) : base(DataTypes.BlockData)
        {
            blockid = blockId;
        }

        public Vector3 curPos;

        public Vector3 dirPos;

        public SessionId playerSession;

        public bool isGrab = false;
        public bool isMoving = false;

        #region 삭제예정라인



        #endregion

    }

    #endregion
}