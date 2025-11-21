using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsController : Singleton<SettingsController>
{
    [Header("Root Settings UI")]
    public GameObject Settings;

    [Header("Canvas Groups")]
    public CanvasGroup SettingGroup;
    public CanvasGroup DisplayGroup;
    public CanvasGroup AudioGroup;
    public CanvasGroup ControlGroup;

    private bool _isESC = true;
    private MenuUI menuUI;

    private Dictionary<string, CanvasGroup> _tabs;      // 카테고리 관리용

    private void Start()
    {
        if (Settings != null)
            Settings.SetActive(false);

        CanvasGroupOff(SettingGroup);

        _tabs = new Dictionary<string, CanvasGroup>
        {
            { "Display", DisplayGroup },
            { "Audio", AudioGroup },
            { "Control", ControlGroup }
        };

        menuUI = FindObjectOfType<MenuUI>();
    }

    private void Update()
    {
        ESC();
    }

    // ---------------------------
    // ▼ SETTINGS OPEN / CLOSE
    // ---------------------------
    public void OpenSettings()
    {
        if (Settings != null)
        {
            Settings.SetActive(true);
            _isESC = false;
            CanvasGroupOn(SettingGroup);
            CanvasGroupOn(DisplayGroup);
            CanvasGroupOff(AudioGroup);
            CanvasGroupOff(ControlGroup);
        }
    }
    public void ClickContinue()
    {
        //Pause(false);
        CanvasGroupOff(SettingGroup);
    }
    public void ClickStart()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    public void ClickDisplay()
    {
        CanvasGroupOn(DisplayGroup);
        CanvasGroupOff(AudioGroup);
        CanvasGroupOff(ControlGroup);
    }
    public void ClickAudio()
    {
        CanvasGroupOff(DisplayGroup);
        CanvasGroupOn(AudioGroup);
        CanvasGroupOff(ControlGroup);
    }
    public void ClickControl()
    {
        CanvasGroupOff(DisplayGroup);
        CanvasGroupOff(AudioGroup);
        CanvasGroupOn(ControlGroup);
    }


    public void ClickESC()
    {
        _isESC = true;
        CanvasGroupOff(SettingGroup);
    }

    private void ESC()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if ((SceneManager.GetActiveScene().name == "GameScene" || SceneManager.GetActiveScene().name == "LobbyScene") && menuUI != null)
            {
                // 게임 씬에서 ESC 키를 누를 때 MenuUI를 관리
                menuUI.ESC();
            }
            else
            {
                // 일반적인 ESC 키 처리
                if (_isESC)
                {
                    _isESC = !_isESC;
                    OpenSettings();
                }
                else
                {
                    _isESC = !_isESC;
                    Settings.SetActive(false);
                    CanvasGroupOff(SettingGroup);
                }
            }
        }
    }

    private void CanvasGroupOn(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
    private void CanvasGroupOff(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
}



//public void ESC()
//{
//    if(Input.GetKeyDown(KeyCode.Escape))
//    {
//        if(isESC && isSetESC) 
//        {
//            isSetESC = false;
//            CanvasGroupOn(SetGroup); 
//        }
//        else if(isESC && !isSetESC) 
//        {
//            isSetESC = true;
//            CanvasGroupOff(SetGroup);
//        }
//        else if (!isESC)
//        {
//            ClickESC();
//        }
//    }
//}

//void Pause(bool isPause)
//{
//    if (true == isPause)     // 일시정지 상태
//    {
//        Time.timeScale = 0;
//    }
//    else                            // 일시정지 해제
//    {
//        Time.timeScale = 1;
//    }
//}
