using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


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
    public Vector3 DenialPos = Vector3.zero;
    public Vector3 Dir => Vector3.right * AxisX + Vector3.forward * AxisY;
    public Vector3 curNetPos = Vector3.zero;

    public CharacterCamera TargetCamera;
    public LayerMask groundMask;
    Vector2 SetNetPos = Vector2.zero;

    public GrabPoint grabPoint;
    public Transform AttackPoint;
    public Transform ItemPoint;

    public Items curItems;

    CharacterData myData;

    [SerializeField]
    CharacterDatas C_myData;


    public bool isPlayersConrtol = false;
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
    // 캐릭터 위치 검증여부
    bool isDenial = false;
    Vector2 acceleration = Vector2.zero;


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

    Type IHitScanTarget.GetMyType() 
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


    /// <summary>
    /// 플레이어의 이동을 진행 하는 함수
    /// </summary>
    /// <param name="Pos">목표 방향 or 좌표</param>
    /// <param name="Rot">오브젝트의 전방 설정</param>
    /// <param name="isStatic">동적,정적이동을 결정</param>
    /// <param name="e"></param>
    public void PlayerMove(Vector3 Pos, Vector3 Rot, bool isStatic, UnityAction e = null)
    {
        if (myData.isDenial)
        {
            return;
        }
        if (isStatic)
        {
            StaticMove(Pos, Rot,e);
        }
        else
        {
            DynamicMove(Pos, Rot,e);
        }
    }

    /// <summary>
    /// 동적 이동을 진행하는 함수
    /// </summary>
    /// <param name="Pos">목표 방향</param>
    /// <param name="Rot">오브젝트의 전방설정</param>
    /// <param name="e">이동 종료후 작업</param>
    public void DynamicMove(Vector3 Pos, Vector3 Rot, UnityAction e = null)
    {
        if(Pos == Vector3.zero)
        {
            myAnim.SetBool("IsMoving", false);
            myAnim.SetBool("IsRunning", false);
            myAnim.SetFloat("MoveAxisX", 0);
            myAnim.SetFloat("MoveAxisY", 0);
            return;
        }

        // 목표방향으로 회전 -> 지정거리 이동
        transform.forward = Rot;
        transform.Translate(Pos);

    }

    /// <summary>
    /// 정적 이동을 진행하는 함수
    /// </summary>
    /// <param name="Pos">목표 좌표</param>
    /// <param name="Rot">오브젝트의 전방 설정</param>
    /// <param name="e">이동 종료후 작업</param>
    public void StaticMove(Vector3 Pos, Vector3 Rot, UnityAction e = null)
    {
        if (transform.position == Pos)
        {
            return;
        }

        transform.position = Pos;

        transform.forward = Rot;

        myAnim.SetBool("IsMoving", false);
        myAnim.SetBool("IsRunning", false);
        myAnim.SetFloat("MoveAxisX", 0);
        myAnim.SetFloat("MoveAxisY", 0);

        DenialPos = Vector3.zero;
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
            myAnim.SetBool("IsMoving", false);
            myAnim.SetBool("IsRunning", false); 
            myAnim.SetFloat("MoveAxisX", 0);
            myAnim.SetFloat("MoveAxisY", 0);
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
            transform.position = DenialPos;
            myAnim.SetBool("IsMoving", false);
            myAnim.SetBool("IsRunning", false);
            DenialPos = Vector3.zero;
        }
    }

    /// <summary>
    /// 패킷의 명령전달로 인한 이동 함수
    /// </summary>
    /// <param name="pos"></param>
    public void NetworkMove(Vector3 pos)
    {
        curNetPos = pos;
        // 현재위치 입력시
        if(pos == transform.position)
        {
            //정지상태 모션으로 전환
            return;
        }
        
        //이동상태 모션으로 전환 & 상태 이동상태로 변환





    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Dir"></param>
    public void SetPosition(Vector3 Dir)
    {

    }
    public void SetPosition(float x, float y, float z)
    {
        SetPosition(new Vector3(x,y,z)); 
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
            transform.position += Dir * Speed * Time.deltaTime;
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
        isForceMoving = true;

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


    /// <summary>
    /// 패킷 데이터용 이동 함수
    /// </summary>
    /// <param name="dir">목표 좌표</param>
    public void MoveToPos(Vector3 dir)
    {
        

        // 현재 위치 그대로 이동하면 미동작
        if (dir == transform.position) return;

        Vector3 Axis =  Quaternion.Euler(-transform.rotation.eulerAngles) * dir;

        float Speed = myData.isRunning ? MoveSpeed * RunSpeedRate : MoveSpeed;

        float curSpeed = Mathf.Sqrt(Axis.x * Axis.x + Axis.z * Axis.z);

        transform.position = dir;
        // 30 틱 이상의격차가 나면 이동모션없이 이동시키기
        if (curSpeed > 30 * Speed / Gamemanager.Inst.PollingRate) return;

        myAnim.SetFloat("MoveAxisX", Mathf.Clamp(Axis.x * MotionSpeed, -1.0f, 1.0f));
        myAnim.SetFloat("MoveAxisY", Mathf.Clamp(Axis.y * MotionSpeed, -1.0f, 1.0f));
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

    public void ItemRemove()
    {

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
