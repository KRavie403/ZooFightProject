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
    private bool _settingOpened = false;
    private bool _menuOpened = false;

    // Start is called before the first frame update
    private void Start()
    {
        CanvasGroupOff(MenuGroup);
        WarningPopup.SetActive(false);

        settingsController = SettingsController.Inst;
    }

    public void ClickESC()
    {
        _settingOpened = true;
    }

    public void ESC()
    {
        if (!_menuOpened)
        {
            _menuOpened = true;
            CanvasGroupOn(MenuGroup);
            return;
        }

        if (!SettingsController.Inst.IsESC)
        {
            SettingsController.Inst.ClickESC();
            return;
        }

        _menuOpened = false;
        CanvasGroupOff(MenuGroup);
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

    public void OnSettingsButtonClick()
    {
        if (SettingsController.Inst != null)
        {
            _settingOpened = true;
            SettingsController.Inst.OpenSettings();
        }
        else
        {
            Logger.LogWarning("SettingsController 인스턴스를 찾을 수 없습니다.");
        }
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
