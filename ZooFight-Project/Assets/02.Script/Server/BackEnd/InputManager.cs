using UnityEngine;
using Protocol;

public class InputManager : MonoBehaviour
{
    [Header("Attack Stick for Aiming")]
    //public CharacterController attackStick;   // 공격 스틱 (혹은 마우스 위치로 대체)

    private bool isMove = false;  // 이동 여부

    private void Start()
    {
        // 게임 상태 변화에 따라 입력 처리 메서드를 연결
        Gamemanager.InGame += HandleMoveInput;
        //Gamemanager.InGame += HandleAttackInput;
        Gamemanager.AfterInGame += SendNoMoveMessage;
    }

    /// <summary>
    /// 이동 입력 처리 (PC버전에서는 키보드 사용)
    /// </summary>
    private void HandleMoveInput()
    {
        // 이동 입력을 처리할 때는 키보드 입력을 받는다.
        isMove = false;

        // WASD 키 또는 방향키를 사용한 이동 입력 처리
        float horizontal = Input.GetAxisRaw("Horizontal");  // A/D 또는 좌우 화살표
        float vertical = Input.GetAxisRaw("Vertical");      // W/S 또는 상하 화살표

        if (horizontal == 0 && vertical == 0)
        {
            return;  // 이동하지 않으면 처리하지 않음
        }

        isMove = true;  // 이동 중으로 설정

        // 이동 방향 벡터 생성 (평면에서의 이동)
        Vector3 moveVector = new Vector3(horizontal, 0, vertical).normalized;
        Debug.Log($"moveVector: {moveVector}");

        int keyCode = KeyEventCode.MOVE;  // 이동 키 코드

        // 이동 처리 메시지 생성
        KeyMessage moveMsg = new KeyMessage(keyCode, moveVector);
        Debug.Log($"moveMsg: {moveMsg}");

        // 메시지를 네트워크로 전송
        SendKeyMessage(moveMsg);
    }

    /// <summary>
    /// 공격 입력 처리 (PC버전에서는 마우스 위치를 목표로 공격)
    /// </summary>
    //private void HandleAttackInput()
    //{
    //    if (attackStick == null)
    //    {
    //        return;  // 공격 스틱이 설정되지 않았으면 입력 처리하지 않음
    //    }

    //    if (!Input.GetMouseButton(0)) // 마우스 왼쪽 버튼이 눌렸을 때
    //    {
    //        return;
    //    }

    //    Vector3 aimPos = GetMouseWorldPosition();  // 마우스 위치를 월드 좌표로 변환

    //    if (aimPos == Vector3.zero)
    //    {
    //        return;  // 목표 위치가 없으면 처리하지 않음
    //    }

    //    int keyCode = KeyEventCode.ATTACK;  // 공격 키 코드

    //    // 공격 처리 메시지 생성
    //    KeyMessage attackMsg = new KeyMessage(keyCode, aimPos);

    //    // 메시지를 네트워크로 전송
    //    SendKeyMessage(attackMsg);
    //}

    /// <summary>
    /// 외부에서 공격 위치를 직접 전달받아 공격 처리
    /// </summary>
    //public void AttackInput(Vector3 pos)
    //{
    //    if (GameManager.GetInstance().GetGameState() != GameManager.GameState.InGame)
    //    {
    //        return;  // 게임이 진행 중이지 않으면 처리하지 않음
    //    }

    //    int keyCode = KeyEventCode.ATTACK;
    //    KeyMessage attackMsg = new KeyMessage(keyCode, pos);

    //    // 메시지를 네트워크로 전송
    //    SendKeyMessage(attackMsg);
    //}

    /// <summary>
    /// 이동하지 않는 상태에서 이동 메시지 전송
    /// </summary>
    private void SendNoMoveMessage()
    {
        int keyCode = 0;

        // 이동하지 않고, 월드에서 플레이어가 움직였으면 NoMove 메시지 전송
        if (!isMove && WorldManager.Inst.IsMyPlayerMove())
        {
            keyCode |= KeyEventCode.NO_MOVE;
        }

        if (keyCode == 0) return;  // 이동하지 않았다면 메시지 전송하지 않음

        // NoMove 메시지 생성
        KeyMessage noMoveMsg = new KeyMessage(keyCode, Vector3.zero);

        // 메시지를 네트워크로 전송
        SendKeyMessage(noMoveMsg);
    }

    /// <summary>
    /// 마우스 위치를 월드 좌표로 변환
    /// </summary>
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;  // 마우스가 가리키는 월드 위치 반환
        }

        return Vector3.zero;  // 월드 위치를 찾지 못하면 (예: UI 위에 있을 경우) 0 반환
    }

    /// <summary>
    /// 키 메시지를 네트워크로 전송하는 함수
    /// </summary>
    /// <param name="msg">전송할 키 메시지</param>
    private void SendKeyMessage(KeyMessage msg)
    {
        if (BackEndMatchManager.GetInstance().IsHost())
        {
            // 호스트인 경우 로컬 큐에 메시지 추가
            BackEndMatchManager.GetInstance().AddMsgToLocalQueue(msg);
        }
        else
        {
            // 클라이언트인 경우 메시지를 서버로 전송
            BackEndMatchManager.GetInstance().SendDataToInGame<KeyMessage>(msg);
        }
    }
}