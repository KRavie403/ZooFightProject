using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DataScripts;
using Protocol;
using BackEnd;
using Unity.Mathematics;
using static UnityEditor.PlayerSettings;

public class PlayerController : MovementController, IHitScanTarget , IHitScanner , IObjectId
{
    
    #region 참조 변수 목록
    public enum pState
    {
        Create = 0, 
        Idle,
        Move,
        Jump,
        ItemReady,
        ItemUse,

        Down,
        Recovery,


        GameReady,
        GameEnd,
        StateCount
    }

    // 캐릭터동작에 필요한 함수들 목록
    public enum pFunc
    {
        Move,
        Jump,
        ItemReady,
        ItemUse,
        ItemRelease,
        BasicAttack,
        EnterSuperArmor,

        AllStop,
        FuncCount
    }

    [SerializeField]
    private pState State;
    protected StateMachine PlayerSM;

    // 기본 모든 상태목록
    public Dictionary<pState, BaseState> p_States = new Dictionary<pState, BaseState>();
    // 슈퍼아머 상태 목록
    public Dictionary<pState, BaseState> S_States = new Dictionary<pState, BaseState>();

    public bool IsDown => PlayerSM.CurrentState != p_States[pState.Down];


    [Range(-1.0f, 1.0f)]
    public float AxisX, AxisY = 0;

    public Vector3 Dir => Vector3.right * AxisX + Vector3.forward * AxisY;
    public Vector3 curNetPos = Vector3.zero;
    public Vector3 curNetRot = Vector3.zero;

    public Vector2 curNetAxis = Vector2.zero;
    public float curNetDist = 0.0f;


    public CharacterCamera TargetCamera;
    public LayerMask groundMask;

    public GrabPoint grabPoint;
    public Transform AttackPoint;
    public Transform ItemPoint;

    public Items curItems;

    CharacterData myData;
    CharacterData_Class C_myData;


    public bool isPlayersConrtol
    {
        get
        {
            if (Gamemanager.Inst.currentPlayer == this)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    bool isMoving 
    {
        get
        {
            if (isPlayersConrtol)
            {
                if (AxisX == 0 && AxisY == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (curNetPos == transform.position)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }


    bool isSuperArmor = false;
    bool isUIOpen = false;
    bool IsRunning = false;
    bool isJump = false;
    bool isAbleMove = false;

    [SerializeField]
    bool isForceMoving = false;
    
    [SerializeField]
    bool isPushing = false;
    bool isSliding = false;
    
    // 사용자의 캐릭터 여부
    bool isOwner 
    { 
        get
        {
            if(Gamemanager.Inst.currentPlayer == this)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    public bool isCrashed = false;
    public bool isKeyReverse
    {
        get => myData.isKeyReverse;
        set => myData.isKeyReverse = value;
    }
    public bool isGrab
    {
        get { return myData.isGrab; }
        set { myData.isGrab = value;}
    }

    #region 히트스캔코드

    #region Object ID

    int IObjectId.ObjectId => myObjectNum;

    ObjectType IObjectId.ObjectType => myObjType;

    #endregion

    Component IHitScanner.myComp => this as Component;
    Component IHitScanTarget.myComp => this as Component;

    int IHitScanTarget.testcode => TestCode;
    public int TestCode = 0;
    Component[] myTarget;
    Component[] IHitScanner.myTargets => myTarget;


    void IHitScanner.AddTarget(Component[] target)
    {
        
    }

    void IHitScanner.Hit()
    {

    }


    /// <summary>
    /// 타격을 전달받은 주체가 플레이어일때 
    /// </summary>
    /// <param name="component"></param>
    void IHitScanTarget.Hit(Component component)
    {
        Debug.Log(component);

        if (component.GetComponent<ScanTester>() != null) 
        {
            Debug.Log("Scanned");
            PushedOut(component.transform.position, 5.5f, 10.0f);
            return;
        }
        // 주체가 플레이어일때
        if (component.GetComponent<PlayerController>() != null)
        {

        }
        // 주체가 아이템일때
        else if (component.GetComponent<Items>() != null) 
        {
            Items item = component.GetComponent<Items>();
            switch (item.myCode)
            {
                case ItemCode.Bomb:
                    PushedOut(component.transform.position, component.GetComponent<Item_Bomb>().Value3, 10.0f);
                    GetDamaged(component.GetComponent<Item_Bomb>().Value1);
                    break;
                case ItemCode.BananaTrap:
                    //Slide()
                    break;
                case ItemCode.BlockChangeScroll:
                    break;
                case ItemCode.CurseScroll:
                    break;
                case ItemCode.SpiderBomb:
                    GetCrowdControl(StatusCode.Slow,component.GetComponent<Item_SpiderBomb>().Value2,component.GetComponent<Item_SpiderBomb>().Value1);
                    break;
                case ItemCode.InkBomb:
                    break;
                case ItemCode.ToyHammer:
                    break;
                default:
                    break;
            }
        }
    }

    System.Type IHitScanTarget.GetMyType() 
    { 
        return GetType();
    }

    #endregion

    #endregion

    protected override void Awake()
    {
        base.Awake();
        AxisX = 0;
        AxisY = 0;
        TargetCamera = GetComponentInChildren<CharacterCamera>();
        grabPoint = GetComponentInChildren<GrabPoint>();

    }

    // Start is called before the first frame update
    protected override void Start()
    {
        // 나중에는 패킷 묶는곳에 던지고 묶인 패킷을 주기적으로 전송하기
        //Gamemanager.OnPollingRate += () => ;
        PlayerSM = new StateMachine();

        p_States.Add(pState.Create, new Character_Create(this, PlayerSM));

        p_States.Add(pState.Idle, new Character_Idle(this, PlayerSM));
        p_States.Add(pState.Move, new Character_Move(this, PlayerSM));
        p_States.Add(pState.Jump, new Character_Jump(this, PlayerSM));

        p_States.Add(pState.ItemReady, new Characater_ItemReady(this, PlayerSM));
        p_States.Add(pState.ItemUse, new Character_ItemUse(this, PlayerSM));

        p_States.Add(pState.Down, new Character_Down(this, PlayerSM));
        p_States.Add(pState.Recovery, new Character_Recovery(this, PlayerSM));


        //CharacterInitalize(myTeam, SessionId, CharacterID);

        StateInitiate();

        PlayerSM.Initalize(p_States[pState.Create]);

        CharacterInitate();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //MoveStateCheck();
        PlayerSM.CurrentState.LogicUpdate();
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    Slide(transform.forward, 1, 0.5f);
        //}
        //CharacterMove(AxisX, AxisY,isDenial);

    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
        //PlayerSM.CurrentState.PhysicsUpdate();


    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    public void StateInitiate()
    {
        for (int i = 0; i < (int)pState.StateCount; i++)
        {
            if (p_States.ContainsKey((pState)i))
            {
                p_States[(pState)i].Initate();
            }
        }
    }

    #region 정보 폴링

    /// <summary>
    /// 현재 이 캐릭터의 상태정보 전송
    /// </summary>
    public void StatusPolling()
    {

    }

    public void StatusRenewal()
    {

    }

    public void InsertPlayerInfo()
    {

    }

    /// <summary>
    /// 플레이어 정보주입 - 세션id , 팀 , 플레이어 id
    /// 정보주입 전까지는 생성상태 유지될수 있도록 유지 필요
    /// </summary>
    /// <param name="PlayerTeam"></param>
    /// <param name="SessionID"></param>
    /// <param name="PlayerID"></param>
    public void CharacterInitalize(Team PlayerTeam,int SessionID,int PlayerID)
    {
        myTeam = PlayerTeam;
        SessionId = SessionID;
        CharacterID = PlayerID;
        Gamemanager.Inst.GetTeam(PlayerTeam).Add(CharacterID, this);
        Gamemanager.Inst.GetTeamId(PlayerTeam).Add(CharacterID);
    }

    public void CharacterInitate()
    {
        PlayerSM.ChangeState(p_States[pState.Idle]);
    }
    public void StateChanged()
    {
        //switch (switch_on)
        //{
        //    default:
        //}
    }

    #endregion

    #region 캐릭터 무브먼트

    public IEnumerator ForceMovement;

    #region 캐릭터 이동

    public void SetIsMoving(bool isMoving)
    {
        myData.isMoving = isMoving;
    }
    public bool GetIsMoving()
    {
        return myData.isMoving;
    }


    public void SetRunning(bool isRunning)
    {
        myData.isRunning = isRunning;
    }
    public bool GetIsRunning()
    {
        return myData.isRunning;
    }

    //작업 필요
    public void MoveStateCheck()
    {
        
        // ismove로 전환 필요
        if(AxisX == 0 && AxisY == 0)
        {
            if (isShield)
            {
                //if(PlayerSM.CurrentState == p_States[p])
                if(isSuperArmor)
                {
                    SetIsMoving(false);
                }
            }
            else
            {
                if (isSuperArmor)
                {
                    SetIsMoving(false);
                }
            }
        }
        else
        {
            if (isShield)
            {
                if (isSuperArmor)
                    ;
                SetIsMoving(true);
            }
            else
            {
                if (isSuperArmor) 
                    ;

                SetIsMoving(true);
            }
        }



    }

    #region 이동관련 신규코드

    public void Move()
    {
        if (!isForceMoving)
        {
            if (isOwner)
            {
                PlayerMove(new Vector2(AxisX, AxisY), transform.forward, Time.deltaTime);
            }
            else
            {
                PlayerMove(curNetPos, curNetRot,curNetDist);
            }
        }
              
    }

    public Vector3 GetMoveVector()
    {
        return Vector3.zero;
    }

    /// <summary>
    /// 플레이어의 이동동작
    /// </summary>
    /// <param name="Axis">플레이어의 전방기준 이동방향</param>
    /// <param name="rot">플레이어의 전방벡터</param>
    /// <param name="dist">플레이어의 이동거리</param>
    /// <param name="e"></param>
    public void PlayerMove(Vector2 Axis, Vector3 rot, float dist, UnityAction e = null)
    {
        BasicMove(Axis, rot, dist);

    }


    public Quaternion GetCharacterRot()
    {
        return transform.rotation;
    }

    public void BasicMove(Vector2 Axis, Vector3 rot, float dist)
    {
        // 이동거리가 0일때 모션정지
        if (dist == 0)
        {
            MoveMotionStop();
            return;
        }
        // Axis가 없을때 정지
        if (Axis == Vector2.zero)
        {
            MoveMotionStop();
            return;
        }


        Vector3 Direction = new Vector3(Axis.x, 0, Axis.y);
        Direction = Vector3.Normalize(Direction);

        transform.position += MakeDir(AxisX, AxisY) * dist;

        if (isGrab)
        {
            Vector2 BlockDir = Vector2.zero;
            BlockDir = grabPoint.curGrabBlock.DistSelect(Direction, transform.forward);
            grabPoint.curGrabBlock.SetcurDir(BlockDir, transform.forward);


            // 그냥 블록
            Vector3 pos = new Vector3(BlockDir.x, 0, BlockDir.y);
            Vector3 dir = new Vector3(0, 0, 0);
            //BlockMoveMessage blockMoveMessage = new BlockMoveMessage(0, 0, pos, dir);
            //BackEndMatchManager.GetInstance().SendDataToInGame<BlockMoveMessage>(blockMoveMessage);

            // 팀 블록
            var sessionId = Backend.Match.GetMySessionId();
            int teamNumber = BackEndMatchManager.GetInstance().GetTeamInfo(sessionId);
            Team playerTeam = BackEndMatchManager.GetInstance().ConvertTeamNumberToEnum(teamNumber);
            BlockData_Class blockDataMessage = new BlockData_Class(playerTeam);
            blockDataMessage.dirPos = pos;
            BackEndMatchManager.GetInstance().SendDataToInGame<BlockData_Class>(blockDataMessage);
        }


        myAnim.SetFloat("MoveAxisX", Mathf.Clamp(AxisX * MotionSpeed, -1.0f, 1.0f));
        myAnim.SetFloat("MoveAxisY", Mathf.Clamp(AxisY * MotionSpeed, -1.0f, 1.0f));

        myAnim.SetBool("IsMoving", true);
        if (myData.isRunning)
        {
            myAnim.SetBool("IsRunning", true);
        }
        else
        {
            myAnim.SetBool("IsRunning", false);
        }

    }
     
    public void BasicMove()
    {
        float Speed = myData.isRunning ? MoveSpeed * RunSpeedRate : MoveSpeed;
        
        BasicMove(curNetAxis, curNetRot, Time.deltaTime * Speed);

    }

    public void MoveMotionStop()
    {
        myAnim.SetBool("IsMoving", false);
        myAnim.SetBool("IsRunning", false);
        myAnim.SetFloat("MoveAxisX", 0);
        myAnim.SetFloat("MoveAxisY", 0);
    }

    public void SendPlayerMove()
    {
        Vector2 Axis = new Vector2(AxisX, AxisY);
        Vector3 rot = transform.forward;
        float dist = Time.deltaTime;
    }

    #endregion
      

    public void CurAxisMove()
    {
        //CharacterMove(AxisX, AxisY, isDenial);
        if(!isForceMoving)
        {
            CharacterMove(myData.isDenial, AxisX, AxisY);
        }
    }

    /// <summary>
    /// 사용자의 직접 조작에 의한 이동함수
    /// </summary>
    /// <param name="AxisX"></param>
    /// <param name="AxisY"></param>
    public void PlayersMove(float AxisX , float AxisY)
    {


        // 입력 이동값이 0일때 아무것도안하기
        if (AxisX == 0 && AxisY == 0)
        {
            MoveMotionStop();
            return;
        }

        Vector3 Direction = Vector3.Normalize(new Vector3 (AxisX,0,AxisY));

        float Speed = myData.isRunning ? MoveSpeed * RunSpeedRate : MoveSpeed;

        // 프로토콜 전송용 벡터
        Vector3 Dir = MakeDir(AxisX, AxisY);
        //transform.Translate(MoveSpeed * time.deltaTime * Direction, Space.Self);
        transform.position += MakeDir(AxisX, AxisY) * Speed * Time.deltaTime;

        if (isGrab)
        {
            Vector2 BlockDir = Vector2.zero;
            BlockDir = grabPoint.curGrabBlock.DistSelect(Direction,transform.forward);
            grabPoint.curGrabBlock.SetcurDir(BlockDir, transform.forward);


            // 그냥 블록
            Vector3 pos = new Vector3(BlockDir.x, 0, BlockDir.y);
            Vector3 dir = new Vector3(0, 0, 0);
            //BlockMoveMessage blockMoveMessage = new BlockMoveMessage(0, 0, pos, dir);
            //BackEndMatchManager.GetInstance().SendDataToInGame<BlockMoveMessage>(blockMoveMessage);

            // 팀 블록
            var sessionId = Backend.Match.GetMySessionId();
            int teamNumber = BackEndMatchManager.GetInstance().GetTeamInfo(sessionId);
            Team playerTeam = BackEndMatchManager.GetInstance().ConvertTeamNumberToEnum(teamNumber);
            BlockData_Class blockDataMessage = new BlockData_Class(playerTeam);
            blockDataMessage.dirPos = pos;
            BackEndMatchManager.GetInstance().SendDataToInGame<BlockData_Class>(blockDataMessage);
        }

        myAnim.SetFloat("MoveAxisX", Mathf.Clamp(AxisX * MotionSpeed, -1.0f, 1.0f));
        myAnim.SetFloat("MoveAxisY", Mathf.Clamp(AxisY * MotionSpeed, -1.0f, 1.0f));

        myAnim.SetBool("IsMoving", true);
        if(myData.isRunning)
        {
            myAnim.SetBool("IsRunning", true);
        }
        else
        {
            myAnim.SetBool("IsRunning", false);
        }

    }

    public Vector3 MakeDir(float AxisX,float AxisY)
    {
        Vector3 dir = Vector3.Normalize(new Vector3(AxisX,0,AxisY));

        return transform.rotation * dir;
    }

    public void CharacterMove(bool denial , float AxisX = 0,float AxisY = 0)
    {
        if (!denial)
        {
            PlayersMove(AxisX, AxisY);
        }
        else
        {
            myAnim.SetBool("IsMoving", false);
            myAnim.SetBool("IsRunning", false);
        }
    }

    /// <summary>
    /// 캐릭터를 특정 방향(dir)으로 일정 거리(dist)만큼 speed 만큼의 속도로 미끄러지게 하는 함수
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="dist"></param>
    /// <param name="Speed"></param>
    /// <param name="e"></param>
    public void Slide(Vector3 dir, float dist, float Speed, UnityAction e = null)
    {
        if (!isSliding)
        {
            if (ForceMovement != null)
            {
                StopCoroutine(ForceMovement);
            }
            ForceMovement = CharacterSlide(dir, dist, Speed, () => { if (e != null)  e?.Invoke();  isForceMoving = false; });
            StartCoroutine(ForceMovement);
            isSliding = true;
        }
        else
        {
            StopCoroutine(ForceMovement);
            ForceMovement = CharacterSlide(dir, dist, Speed, () => { if (e != null)  e?.Invoke();  isForceMoving = false; });
            StartCoroutine(ForceMovement);
            isSliding = true;
        }
        //myAnim.SetBool("");
        //StartCoroutine(CharacterSlide(dir, dist, Speed, e));   
    }

    /// <summary>
    /// Slide 함수의 동적 연출을 담당하는 함수
    /// </summary>
    /// <param name="dir">밀려나는 방향</param>
    /// <param name="Dist">밀려나는 거리</param>
    /// <param name="Speed">밀려나는 속도</param>
    /// <param name="e">행동종료후 동작</param>
    /// <returns></returns>
    public IEnumerator CharacterSlide(Vector3 dir, float Dist,float Speed, UnityAction e = null)
    {
        float duringTime = 0.0f;

        Vector3 newDir = new Vector3(dir.x, 0.0f, dir.z);
        newDir.Normalize();

        myAnim.SetBool("IsSlide", true);
        myAnim.SetTrigger("Sliding");

        while (duringTime < Dist/Speed)
        {
            duringTime += Time.deltaTime;
            //CharacterMove(false, dir.x, dir.z);
            transform.position += newDir * Speed * Time.deltaTime;
            yield return null;
        }
        
        myAnim.SetBool("IsSlide", false);

        e?.Invoke();
    }


    /// <summary>
    /// Pos 기준으로 거리 Dist 만큼까지 밀려나는 함수
    /// Dist 와 Player의 위치의 연관없이 거리만큼만 밀려남 
    /// </summary>
    /// <param name="Pos"></param>
    /// <param name="Dist"></param>
    /// <param name="Speed"></param>
    /// <param name="e"></param>
    public void PushedOut(Vector3 Pos, float Dist, float Speed, UnityAction e = null)
    {

        if(!isPushing)
        {
            if(ForceMovement != null)
            {
                StopCoroutine(ForceMovement);
            }
            ForceMovement = PushOut(Pos, Dist, Speed, () => {
                if (e != null) e?.Invoke(); 
                isForceMoving = false;
                Debug.Log("PushEnd");
            });
            Debug.Log("PushStart");
            StartCoroutine(ForceMovement);
        }
        else
        {
            StopCoroutine(ForceMovement);
            ForceMovement = PushOut(Pos, Dist, Speed, () => {
                if (e != null) e?.Invoke();
                isForceMoving = false;
                Debug.Log("PushEnd");
            });
            Debug.Log("PushStart"); 
            StartCoroutine(ForceMovement);
        }
    }

    /// <summary>
    /// PushedOut 함수의 연속적 동작을 담당하는 함수
    /// </summary>
    /// <param name="Pos"></param>
    /// <param name="Dist"></param>
    /// <param name="Speed"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    IEnumerator PushOut(Vector3 Pos, float Dist, float Speed, UnityAction e = null)
    {
        isPushing = true;
        isForceMoving = true    ;

        Vector3 NewPos = new Vector3(Pos.x, 0.0f, Pos.z);
        Vector3 NewTpos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        // 지점부터 플레이어 방향으로의 벡터를 생성
        Vector3 Dir = NewTpos - NewPos;

        // 이동해야할 거리를 측정
        float newDist = Dist - Dir.magnitude;

        float duringTime = 0;
        // 벡터의 크기를 1로 고정
        Dir.Normalize();
        Debug.Log(Dir.magnitude);

        // 이동시간만큼 이동
        while (duringTime < newDist/Speed)
        {
            duringTime += Time.deltaTime;

            transform.position += Dir * Speed * Time.deltaTime;
            //transform.Translate(Dir * Speed * time.deltaTime);

            yield return null;
        }
        e?.Invoke();
        isPushing = false;  
    }



    //public void SetPosition(Vector3 pos)
    //{
    //    //transform.position = pos;
    //}
    public Vector3 GetPosition()
    {
        return transform.position;
    }
 

    #endregion

    #region 특수이동

    public void Jump()
    {
        if(PlayerSM.CurrentState != p_States[pState.Jump])
            PlayerSM.ChangeState(p_States[pState.Jump]);
    }

    public void CharacterJump()
    {
        if (!myData.isJump)
        {

            GetComponent<Rigidbody>().AddForce(Vector3.up*JumpHeight, ForceMode.Impulse);
            myData.isJump = true;
        }
    }

    public void SetisJump(bool IsJump)
    {
        myData.isJump = IsJump;
    }
    public bool GetisJump()
    {
        return myData.isJump;
    }

    public void JumpEnd()
    {
        myData.isJump = false;
    }

    // 입력받은 타겟을 대상으로 입력받은 거리만큼 밀려나기
    public void KnockBack(Transform target, float dist, float Speed, UnityAction e = null)
    {

        StartCoroutine(KnockBackStart(target, dist, Speed, e));
    }

    public IEnumerator KnockBackStart(Transform target, float dist, float Speed, UnityAction e = null)
    {

        myData.isDenial = true;

        Vector3 dir = Vector3.Normalize(transform.position - target.position); 
        
        Vector3 StartPos = transform.position;

        float duringDist = 0;

        while (duringDist < dist)
        {
            duringDist += Speed * Time.deltaTime;

            transform.position += StartPos + dir * Speed * Time.deltaTime;


            // 물체 또는 벽에 충돌시 중단
            if (myData.isCrashed) break;


            yield return null;
        }

        myData.isDenial = false;   
    }

    #endregion

    #region 블럭잡기

    public void Grab()
    {
        if(grabPoint.GrabableBlock != null)
        {
            BlockGrab(grabPoint.GrabableBlock.transform);
        }
    }
    public void DeGrab()
    {
        if (grabPoint.curGrabBlock != null)
        {
            isGrab = false;
            grabPoint.curGrabBlock.DeGrab(this);
        }
    }

    public void BlockGrab(Transform targat)
    {
        targat.GetComponent<BlockObject>().Grab(this);
    }

    public void BlockFree(Transform targat)
    {
        targat.GetComponent<BlockObject>().DeGrab(this);
    }

    #endregion

    #endregion

    #region 아이템

    // 아이템 꺼내기 or 집어넣기
    public void ItemRelease()
    {

        if(PlayerSM.CurrentState == p_States[pState.ItemUse])
        {
            PlayerSM.ChangeState(p_States[pState.Idle]);
            MoveStateCheck();
        }
        else if (PlayerSM.CurrentState == p_States[pState.ItemReady])
        {
            MoveStateCheck();
        }

    }

    // 동작 불가능상태 = 아이템 사용중 , 상태이상, 기상, 점프
    public void ItemReady()
    {
        if (curItems == null) return;
        if (PlayerSM.CurrentState == p_States[pState.ItemUse]) return;
        if (PlayerSM.CurrentState == p_States[pState.Down]) return;
        if (PlayerSM.CurrentState == p_States[pState.Recovery]) return;
        if (PlayerSM.CurrentState == p_States[pState.Jump]) return;

        if (PlayerSM.CurrentState == p_States[pState.ItemReady])
        {
            ItemRelease();
        }
        else
        {
            PlayerSM.ChangeState(p_States[pState.ItemReady]);
        }
    }

    /// <summary>
    /// 아이템 사용동작의 시작을 알리는 함수
    /// </summary>
    public void ItemUse()
    {
        if(curItems != null)
        {
            if(PlayerSM.CurrentState == p_States[pState.ItemReady])
            {
                PlayerSM.ChangeState(p_States[pState.ItemUse]);

                //curItems.ItemUse();
            }
        }
    }

    /// <summary>
    /// 아이템 사용동작이 끝남을 알리는 함수
    /// </summary>
    public void ItemUseEnd()
    {
        if(PlayerSM.CurrentState == p_States[pState.ItemUse])
        {
            PlayerSM.ChangeState(p_States[pState.Idle]);
            MoveStateCheck();
            Debug.Log(GetState()+"Test");
        }
    }

    /// <summary>
    /// 아이템을 지급받는 함수
    /// </summary>
    public void GetItem()
    {

        Items items = ItemSystem.Inst.GiveItem(this, ItemSystem.Inst.RandomItemSelect());
        if( items == null) return;  
        if(curItems == null)
        {
            curItems = items;
        }
        else
        {
            items.ReturnItem();
        }
    }

    #endregion

    #region 판정관련
    [SerializeField] float RecoveryTime = 2.0f;



    /// <summary>
    /// Damage 수치만큼 데미지를 받는 함수
    /// </summary>
    /// <param name="Damage"></param>
    public void GetDamaged(float Damage)
    {
        Debug.Log("Damaged");
        if(isShield)
        {
            CurShield -= Damage;
        }
        else
        {
            CurHP -= Damage;
        }

    }
    
    /// <summary>
    /// time 동안 총합 Damage 수치만큼 데미지를 받는 함수
    /// </summary>
    /// <param name="Damage"></param>
    /// <param name="time"></param>
    public void GetDotDamaged(float Damage,float time)
    {
        StartCoroutine(DotDamaged(Damage,time));

    }

    /// <summary>
    /// Value 수치만큼 실드량을 회복하는 함수
    /// </summary>
    /// <param name="Value"></param>
    public void GetShield(float Value)
    {
        if (Value < 0) return;
        isShield = true;
        CurShield = Value > MaxShield ? MaxShield : Value;  
    }

    public IEnumerator StaminaWork()
    {

        // 항상 동작하게 하기
        while(true)
        {
            // 스테 감소
            if (myData.isRunning)
            {
                CurSP -= Time.deltaTime * SPRecovery;
            }
            // 스테 증가
            else
            {
                CurSP += Time.deltaTime * SPRecovery;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 상태이상을 받는 함수
    /// </summary>
    /// <param name="code">상태이상 종류</param>
    /// <param name="time">상태이상 지속시간</param>
    /// <param name="Power">상태이상 강도</param>
    public void GetCrowdControl(StatusCode code, float time, float Power=0)
    {

        switch (code)
        {
            case StatusCode.Normal:
                break;
            // 강도 = 느려지는 정도
            case StatusCode.Slow:
                GetSlow(time,Power);
                break;
            case StatusCode.Blind:
                break;
            case StatusCode.Bind:
                break;
            case StatusCode.Stun:
                break;
            case StatusCode.AirBone:
                break;
            default:
                break;
        }
    }

    public void DownAction()
    {
        PlayerSM.ChangeState(p_States[pState.Down]);
    }

    public void GetSlow(float time,float Power)
    {
        
    }

    public IEnumerator Slow(float time, float Power)
    {
        float duringTime = 0;

        float tempRate = BaseSpeedRate;

        BaseSpeedRate = Power;
        while (duringTime > time)
        {
            duringTime += Time.deltaTime;
            yield return null;
        }

        BaseSpeedRate = tempRate;
    }

    public void CharacterRecovery()
    {
        StartCoroutine(HpRecovery());
    }

    IEnumerator DotDamaged(float Damage,float time,UnityAction e = null)
    {
        float duringTime = 0;
        while (duringTime < time)
        {
            duringTime += Time.deltaTime;
            GetDamaged(Damage / (time * Time.deltaTime));
            yield return null;
        }
        e?.Invoke();
    }

    IEnumerator HpRecovery()
    {
        float Times = 0.0f;
        myAnim.SetTrigger("Recovery");
        while (Times < RecoveryTime)
        {
            Times += Time.deltaTime;
            SetHp(MaxHP * Times / RecoveryTime, true);
            yield return null;
        }
        PlayerSM.ChangeState(p_States[pState.Idle]);
    }

    /// <summary>
    /// 플레이어의 공격 명령
    /// </summary>
    public void PlayerAttack()
    {
        HitScan.Inst.HitScans(AttackPoint.gameObject, 0.2f, ScanTarget.Player, ScanType.Cube);

    }

    /// <summary>
    /// 패킷에서 전달받은 공격명령
    /// </summary>
    /// <param name="Pos"></param>
    public void Attack(Vector3 Pos)
    {

    }


    // 플레이어의 크기를 입력받은 사이즈로 변경, 변경완료후 입력 받은 명령이 있다면 처리
    public void PlayerSizeChange(float ChangeRate, UnityAction e = null)
    {
        // 
        gameObject.transform.localScale = Vector3.one * ChangeRate * 0.8f;

        e?.Invoke();
    }


    // 해머 공격시 발동
    public void HammerSmash(float Time)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.3f, groundMask);

        foreach (Collider collider in colliders)
        {
            PlayerController player = collider.GetComponent<PlayerController>();
            if (player != null)
            {
                player.GetCrowdControl(StatusCode.Stun, Time);
            }
        }
        myAnim.SetTrigger("HammerSmash");


    }

    // 실드가 파괴 될 경우 발동하는 함수 = 기본값 true
    public void ShieldCrashed(bool isCrash = true)
    {
        // 실드 종료 & 스턴
        isShield = false;

        // 지속시간 종료로 인한 소멸시
        if (!isCrash)
        {
            CurShield = 0.0f;
        }

        DownAction();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log($"{collision.gameObject.layer} , {groundMask.value}");
        
        if ( (1 << collision.gameObject.layer) == groundMask)
        {
            Debug.Log("Ground");
            if(PlayerSM.CurrentState == p_States[pState.Jump])
            {
                MoveStateCheck();
                myData.isJump = false;
            }
        }
    }



    private void OnCollisionExit(Collision collision)
    {
        //
        //if (PlayerSM.CurrentState == p_States[pState.Jump])
        //{
        //    if (collision.gameObject.layer == groundMask)
        //    {
        //        //PlayerSM.ChangeState(p_States[pState.Jump]);
        //        isJump = true;
        //    }
        //}

    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == groundMask)
        {
            myData.isJump = false;
        }
        if (PlayerSM.CurrentState == p_States[pState.Jump])
        {
        }
    }

    #endregion

    #region 데이터인출&수정

    public Team GetEnemyTeam()
    {
        return (Team)((int)myTeam * -1);
    }
    // isSet True = 해당값으로 설정 , False = 해당값만큼 증가
    public void SetHp(float Value,bool isSet = false)
    {
        if (Value > MaxHP)
        {
            CurHP = MaxHP;
            return;
        }

        CurHP = isSet ? Value : CurHP + Value ;
        
    }

    public void SetState(pState state)
    {
        State = state;
    }

    public pState GetState()
    {
        return State;
    }

    #endregion

    #region 승패동작

    public void WinAction()
    {
        myAnim.SetTrigger("Win");
        myAnim.SetBool("IsGameEnd",true);
    }

    public void LoseAction()
    {
        myAnim.SetTrigger("Lose");
        myAnim.SetBool("IsGameEnd", true);
    }

    public void ActionAllStop()
    {
        grabPoint.curGrabBlock.DeGrab(this);
        myAnim.enabled = false;
        this.enabled = false;
    }

    #endregion



}
