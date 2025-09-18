using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    public Button[] btns;

    private void Update()
    {
        ESC();
    }

    // 메인 화면으로 나가기
    public void ESC()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //SceneManager.LoadScene("MainMenuScene");
        }
    }
    
    public void clickESC()
    {
        Gamemanager.GetInstance().ChangeState(Gamemanager.GameState.Result);
    }
}
