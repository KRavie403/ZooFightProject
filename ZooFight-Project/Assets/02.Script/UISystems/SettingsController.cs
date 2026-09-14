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

    public bool IsESC
    {
        get => _isESC;
        set => _isESC = value;
    }

    [SerializeField] private MenuUI menuUI;

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
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (menuUI != null)
            {
                menuUI.ESC();
                return;
            }
        }

        // 기존 Settings 처리
        else if (_isESC)
        {
            _isESC = false;
            Logger.Log($"check 뭐지 _isESC 열 : {_isESC}");
            OpenSettings();
        }
        else
        {
            _isESC = true;
            Logger.Log($"check 뭐지 _isESC 닫 : {_isESC}");
            Settings.SetActive(false);
            CanvasGroupOff(SettingGroup);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        menuUI = FindObjectOfType<MenuUI>();
    }

    private void CanvasGroupOn(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
    public void CanvasGroupOff(CanvasGroup cg)
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
//        if(_isESC && isSetESC) 
//        {
//            isSetESC = false;
//            CanvasGroupOn(SetGroup); 
//        }
//        else if(_isESC && !isSetESC) 
//        {
//            isSetESC = true;
//            CanvasGroupOff(SetGroup);
//        }
//        else if (!_isESC)
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
