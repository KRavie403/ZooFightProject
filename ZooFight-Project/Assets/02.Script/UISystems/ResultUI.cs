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
        Q();
    }

    // 메인 화면으로 나가기
    public void ESC()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
    
    public void clickESC()
    {
            SceneManager.LoadScene("MainMenuScene");
    }
    

    // 재매칭하기
    public void Q()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
    public void clickQ()
    {
            SceneManager.LoadScene("MainMenuScene");
    }
}
