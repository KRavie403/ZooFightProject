using BackEnd;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryDecision : MonoBehaviour
{
    [SerializeField] private Team DecisionTeam = Team.NotSetting;

    private Coroutine movingNextSceneCoroutine;

    // 게임 종료 알림 이벤트 (단순 Action)
    public static event System.Action<Team> OnGameVictory;

    private void OnTriggerEnter(Collider other)
    {
        if (Gamemanager.Inst.IsGameEnd) return;
        if (DecisionTeam == Team.NotSetting) return;

        BlockObject block = other.GetComponent<BlockObject>();
        if (block == null) return;
        if (block.myTeam == Team.NotSetting) return;

        if (block.myTeam == DecisionTeam)
        {
            HandleVictory(block);
        }
    }

    private void HandleVictory(BlockObject block)
    {
        // 게임 종료 정보 저장
        Gamemanager.Inst.IsGameEnd = true;
        //Gamemanager.Inst.VictoryTeam = block.myTeam;
        var sessionId = Backend.Match.GetMySessionId();
        var team = BackEndMatchManager.GetInstance().GetTeamInfo(sessionId);
        var teamType = BackEndMatchManager.GetInstance().ConvertTeamNumberToEnum(team);
        Gamemanager.Inst.VictoryTeam = teamType;

        // 연출 실행
        WinnerTeamAction(block.myTeam);
        LoseTeamAction(block.myTeam);

        // WorldManager에 알림
        OnGameVictory?.Invoke(block.myTeam);
    }

    private void WinnerTeamAction(Team winnerTeam)
    {
        //Gamemanager.Inst.GetTeam(WinnerTeam);
        if (winnerTeam == Team.NotSetting) return;
        //Gamemanager.Inst.currentPlayer.WinAction();
        // 서버 업로드시 사용할 부분
        //for (int i = 0; i < Gamemanager.Inst.GetTeamId(WinnerTeam).Count; i++)
        //{
        //    // 승리 애니메이션 동작
        //    //if (Gamemanager.Inst.)
        //    Gamemanager.Inst.GetTeam(WinnerTeam)
        //        [Gamemanager.Inst.GetWinnerTeamId()[i]].WinAction();
        //}
    }

    private void LoseTeamAction(Team winnerTeam)
    {
        //Gamemanager.Inst.GetEnemyTeam(WinnerTeam);
        if (winnerTeam == Team.NotSetting) return;
        //Gamemanager.Inst.currentPlayer.LoseAction();

        // 서버 업로드시 사용할부분
        //for (int i = 0; i < Gamemanager.Inst.GetTeamId((myHitScanner.Team)((int)WinnerTeam*-1)).Count; i++)
        //{
        //    // 승리 애니메이션 동작
        //    Gamemanager.Inst.GetEnemyTeam(WinnerTeam)
        //        [Gamemanager.Inst.GetLoserTeamId()[i]].LoseAction();
        //}
    }

}




//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class VictoryDecision : MonoBehaviour
//{
//    [SerializeField]
//    Team DecisionTeam = Team.NotSetting;

//    BlockObject enterBlock;

//    private Coroutine movingNextSceneCoroutine;

//    private void Update()
//    {
//        if (Gamemanager.Inst.IsGameEnd) return;
//        if (DecisionTeam == Team.NotSetting) return;
//        if (enterBlock != null)
//        {
//            if (enterBlock.myTeam == Team.NotSetting) return;

//            if (enterBlock.myTeam == DecisionTeam) 
//            {
//                // 승리팀 정보 & 게임 종료 정보 저장
//                Gamemanager.Inst.IsGameEnd = true;
//                Gamemanager.Inst.VictoryTeam = enterBlock.myTeam;

//                // 승리 & 패배 애니메이션 등등 출력
//                WinnerTeamAction(Gamemanager.Inst.VictoryTeam);
//                LoseTeamAction(Gamemanager.Inst.VictoryTeam);

//                // 씬 전환 실행
//                //if (movingNextSceneCoroutine != null)
//                //{
//                //    StopCoroutine(movingNextSceneCoroutine);
//                //}
//                movingNextSceneCoroutine = StartCoroutine(MovingNextScene());
//            }
//        }
//    }


//    private void OnTriggerEnter(Collider other)
//    {
//        if(other.GetComponent<BlockObject>() != null)
//        {
//            enterBlock = other.GetComponent<BlockObject>();
//        }

//    }
//    private void OnTriggerExit(Collider other)
//    {
//        if(other.GetComponent <BlockObject>() != null)
//        {
//            if(other.GetComponent<BlockObject>() == enterBlock)
//            {
//                enterBlock = null;
//            }
//        }
//    }
//    private void OnTriggerStay(Collider other)
//    {
//        if(other.GetComponent<BlockObject>() != null)
//        {
//            if(enterBlock == null)
//            {
//                enterBlock = other.GetComponent<BlockObject>();
//            }
//        }
//    }

//    public void WinnerTeamAction(Team WinnerTeam)
//    {
//        //Gamemanager.Inst.GetTeam(WinnerTeam);
//        if (WinnerTeam == Team.NotSetting) return;
//        Gamemanager.Inst.currentPlayer.WinAction();
//        // 서버 업로드시 사용할 부분
//        //for (int i = 0; i < Gamemanager.Inst.GetTeamId(WinnerTeam).Count; i++)
//        //{
//        //    // 승리 애니메이션 동작
//        //    //if (Gamemanager.Inst.)
//        //    Gamemanager.Inst.GetTeam(WinnerTeam)
//        //        [Gamemanager.Inst.GetWinnerTeamId()[i]].WinAction();
//        //}

//    }
//    public void LoseTeamAction(Team WinnerTeam)
//    {
//        //Gamemanager.Inst.GetEnemyTeam(WinnerTeam);
//        if (WinnerTeam == Team.NotSetting) return;
//        Gamemanager.Inst.currentPlayer.LoseAction();

//        // 서버 업로드시 사용할부분
//        //for (int i = 0; i < Gamemanager.Inst.GetTeamId((myHitScanner.Team)((int)WinnerTeam*-1)).Count; i++)
//        //{
//        //    // 승리 애니메이션 동작
//        //    Gamemanager.Inst.GetEnemyTeam(WinnerTeam)
//        //        [Gamemanager.Inst.GetLoserTeamId()[i]].LoseAction();
//        //}
//    }

//    IEnumerator MovingNextScene()
//    {
//        Debug.Log("MovingNextScene");

//        yield return new WaitForSeconds(1.0f);
//        SceneManager.LoadScene(3);

//        //  씬 로드 후 코루틴 참조를 초기화
//        movingNextSceneCoroutine = null;
//    }

//}
