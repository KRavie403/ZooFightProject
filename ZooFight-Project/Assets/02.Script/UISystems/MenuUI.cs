using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public GameObject Menu;
    public CanvasGroup MenuGroup;
    public GameObject WarningPopup;

    private SettingsController settingsController;
    private bool _isSettingESC = true;
    private bool _isMenuESC = true;

    // Start is called before the first frame update
    private void Start()
    {
        Menu.SetActive(false);
        CanvasGroupOff(MenuGroup);
        WarningPopup.SetActive(false);

        settingsController = SettingsController.Inst;
    }

    public void ClickESC()
    {
        _isSettingESC = true;
    }

    public void ESC()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isSettingESC && _isMenuESC)
            {
                _isMenuESC = false;
                CanvasGroupOn(MenuGroup);
            }
            else if (_isSettingESC && !_isMenuESC)
            {
                _isMenuESC = true;
                CanvasGroupOff(MenuGroup);
            }
            else if (!_isSettingESC)
            {
                ClickESC();
            }
        }
    }

    public void ClickContinue()
    {
        CanvasGroupOff(MenuGroup);
    }

    // WarningPopup을 활성화하는 함수
    public void ShowWarningPopup()
    {
        if (WarningPopup != null)
        {
            WarningPopup.SetActive(true);
        }
        else
        {
            Debug.LogError("WarningPopup 경고창이 할당되었습니다");
        }
    }

    // WarningPopup을 비활성화하는 함수
    public void HideWarningPopup()
    {
        if (WarningPopup != null)
        {
            WarningPopup.SetActive(false);
        }
        else
        {
            Debug.LogError("WarningPopup 경고창이 할당되지 않았습니다.");
        }
    }

    public void ClickQuit()
    {
        WorldManager.Inst.OnGameResult();
    }

    public void CanvasGroupOn(CanvasGroup cg)
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
