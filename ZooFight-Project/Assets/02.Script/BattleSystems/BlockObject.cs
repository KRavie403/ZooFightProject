using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DataScripts;

interface IBlock
{
    public int blockNum { get; } // 블록 번호
    public int type { get; }     // 블록 타입
    public Vector3 position { get; } // 초기 위치

    // 초기화 함수
    public abstract void Initialize(int blockNum, int type, Vector3 position);


    public abstract void SendPositionToServer();


}


public class BlockObject : MonoBehaviour , IBlock , IObjectId
{

    protected bool isGrab = false;
    protected PlayerController myPlayer=null;

    public Team myTeam = Team.NotSetting;

    float BlockMoveSpeed
    {
        get => myPlayer.MoveSpeed;
    }

    public Transform myBlockObj;

    public Vector2 curDir = Vector2.zero;

    public int BlockId = -1;

    #region Blocks 인터페이스 관련

    int blockNum;
    int type;
    Vector3 position;

    int IBlock.blockNum => blockNum;

    int IBlock.type => type;

    Vector3 IBlock.position => position;

    #region IObjectId
    int myObjectId = -1;
    ObjectType myObjType = ObjectType.Block;

    int IObjectId.ObjectId => myObjectId;

    ObjectType IObjectId.ObjectType => myObjType;

    #endregion

    public void Initialize(int blockNum, int type, Vector3 position)
    {
        this.blockNum = blockNum;
        this.type = type;
        this.position = position;

        // 블록의 실제 위치를 설정
        transform.position = position;
    }
    public void SendPositionToServer()
    {

    }

    #endregion

    [SerializeField] protected GameObject RedBlock;
    [SerializeField] protected GameObject BlueBlock;
    [SerializeField] protected GameObject[] DefaultBlock;

    protected Vector2[] JudgeVector = new Vector2[2] {new(1,1),new(-1,1) };
    protected Vector3[] myDirs = new Vector3[5] {Vector3.zero , Vector3.forward, Vector3.left, Vector3.back, Vector3.right };

    NormalBlockdata myBlockData;    

    public bool isChangeActive = false;

    protected virtual void Awake()
    {
        

    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
        // 오브젝트가 생성될때 이미 존재하는 블럭이 있으면 삭제
        if(Gamemanager.Inst.GetTeamBlock(myTeam) != null)
        {
            if(Gamemanager.Inst.GetTeamBlock(myTeam) != this)
            {
                Destroy(gameObject);
            }
        }
        else
        {
                Gamemanager.Inst.AddBlockObj(this);
        }
        if(myTeam != Team.NotSetting)
        {
            Initate(myTeam);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }

    public void Initate(Team myteam)
    {
        myTeam = myteam;
        if (myTeam == Team.BlueTeam)
        {
            RedBlock.SetActive(false);
            BlueBlock.SetActive(true);
            Gamemanager.Inst.BlueTeamBlock = this;
        }
        if (myTeam == Team.RedTeam)
        {
            RedBlock.SetActive(true);
            BlueBlock.SetActive(false);
            Gamemanager.Inst.RedTeamBlock = this;
        }
    }

    public int CreateBlockId()
    {

        while (BlockId != -1)
        {

        }

        return -1;
    }

    public void IdInsert(int id)
    {
        BlockId = id;
    }

    #region 잡기관련
    public void Grab(PlayerController player)
    {
        // 블럭교환 동작 중 잡기 불가능
        if(isChangeActive)
        {
            DeGrab(player);
            return;
        }
        myPlayer = player;


        myPlayer.grabPoint.curGrabBlock = this;
        myPlayer.isGrab = true;
        myBlockData.isGrab = true;
        Debug.Log($"{player.name} Grab");
        StartCoroutine(BlockMove());
    }

    public void DeGrab(PlayerController player)
    {
        myPlayer.isGrab = false;
        
        myPlayer.grabPoint.curGrabBlock = null;

        
        StopCoroutine(BlockMove());
        Debug.Log($"{player.name} DeGrab");

        myPlayer = null;
        myBlockData.isGrab = false;

    }
    #endregion

    #region 블럭 교환 관련
    public void ChangeBlockTeam()
    {
        // 블럭의 팀을 변경
        switch (myTeam) 
        {
            case Team.RedTeam:
                myTeam = Team.BlueTeam;
                Initate(myTeam);
                Debug.Log($"{this.gameObject.name} ChangeTeam");
                break;
            case Team.NotSetting:
                return;
            case Team.BlueTeam:
                myTeam= Team.RedTeam;
                Initate(myTeam);
                Debug.Log($"{this.gameObject.name} ChangeTeam");
                break;
            case Team.AllTarget:
                return;
            default:
                break;
        }
        
    }

    public void ForceDeGrab()
    {
        if(myPlayer != null)
        {
            myPlayer.isGrab = false;
            myPlayer.grabPoint.curGrabBlock = null;

            StopCoroutine (BlockMove());

            myPlayer = null;
            myBlockData.isGrab = false;
            myBlockData.isMoving = false;
            myBlockData.dirPos = transform.position;
            myBlockData.curPos = transform.position;
        }
        else
        {

        }


    }

    #endregion

    #region 블럭이동관련
    IEnumerator BlockMove()
    {
        Vector3 curdir = Vector3.zero;

        Debug.Log("MoveStart");

        while (myBlockData.isGrab)
        {
            if (myPlayer.GetIsMoving() == true)
            {
                myBlockData.isMoving = true;
                curdir.x = curDir.x;
                curdir.z = curDir.y;
                transform.position += curdir * Time.deltaTime * BlockMoveSpeed;
            }
            else
            {
                myBlockData.isMoving = false;
            }
            yield return null;
        }
        if(myPlayer != null)
        {
            myPlayer.isGrab = false;
        }
    }

    public void SetcurDir(Vector2 dir,Vector3 curForward)
    {
        curDir = dir;

    }

    // 진행 가능 방향 결정함수 - 목표 방향 , 현재 전방
    public Vector2 DistSelect(Vector3 pos,Vector3 curForward)
    {
        if(pos == Vector3.zero) return Vector2.zero;
        Quaternion rot =  Quaternion.FromToRotation(Vector3.forward, curForward);

        Vector3 vector3 = rot * pos;

        Vector2 dir = new Vector2(vector3.x, vector3.z);
        


        if (Vector2.Dot(JudgeVector[0],dir) > 0)
        {
            if (Vector2.Dot(JudgeVector[1], dir) > 0)
            {
                return Vector2.up;
            }
            else if (Vector2.Dot(JudgeVector[1], dir) < 0)
            {
                return Vector2.right;
            }
            else
            {
                return Vector2.zero;
            }

        }
        else if (Vector2.Dot(JudgeVector[0],dir)< 0)
        {
            if (Vector2.Dot(JudgeVector[1], dir) > 0)
            {
                return Vector2.left;
            }
            else if (Vector2.Dot(JudgeVector[1], dir) < 0)
            {
                return Vector2.down;
            }
            else
            {
                return Vector2.zero;
            }
        }
        else
        {
            return Vector2.zero;
        }


    }

    #endregion

    #region 승리판정관련

    public void VictoryDecide(Team BeaconTeam)
    {
        if (BeaconTeam == Team.NotSetting) return;

        if(myTeam == BeaconTeam)
        {
            // 승리시 동작할것 실행

            Debug.Log($"{myTeam} Victory!!");
        }

    }

    void IBlock.Initialize(int blockNum, int type, Vector3 position)
    {
        throw new System.NotImplementedException();
    }




    #endregion



}
