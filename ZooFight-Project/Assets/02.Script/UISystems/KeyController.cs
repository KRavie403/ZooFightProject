using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class KeyController : Singleton<KeyController>
{
    public Button[] keyButtons;
    public GameObject[] keyBindingPopup; // 0: 기본 팝업, 1: 키 변경 완료 팝업, 2: 중복 경고 팝업

    private KeyAction currentKeyAction;
    private bool isListeningForKey = false; // 키 입력 대기 중인지 여부

    // 중복 처리용 임시 저장
    private KeyAction duplicatedAction;
    private KeyCode pendingKeyCode;

    private void Start()
    {
#if DEBUG || UNITY_EDITOR
        // KeySetting.keys 초기화가 완료되었는지 확인
        if (KeySetting.keys == null || KeySetting.keys.Count == 0)
        {
            Debug.LogError("KeySetting.keys가 초기화되지 않았습니다.");
            return;
        }
#endif

        for (int i = 0; i < keyButtons.Length; i++)
        {
            KeyAction action = (KeyAction)i;

            keyButtons[i].GetComponentInChildren<TMP_Text>().text = KeySetting.keys[action].ToString();
            keyButtons[i].onClick.AddListener(() => OnKeyButtonClick(action));
        }

        // 저장된 키 설정을 불러오기 (PlayerPrefs 기반)
        LoadAllKeyBindings();
    }

    private void Update()
    {
        if (isListeningForKey)
        {
            // ESC 키 처리: 팝업창 닫기 및 대기 해제
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                keyBindingPopup[0].SetActive(false); // 키 변경 팝업 비활성화
                isListeningForKey = false;
                return;
            }

            // 모든 키코드를 검사하여 어떤 키가 눌렸는지 확인
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    // 중복 → 확인 팝업
                    if (IsKeyCodeAlreadyAssigned(keyCode))
                    {
                        Debug.LogWarning($"{keyCode} 키는 이미 할당된 키입니다.");
                        ShowKeyOwnPopupAsync().Forget();
                        return;
                    }

                    // 키 설정을 변경하고 UI 텍스트 업데이트
                    KeySetting.keys[currentKeyAction] = keyCode;
                    keyButtons[(int)currentKeyAction].GetComponentInChildren<TMP_Text>().text = keyCode.ToString();

                    // 변경된 키 저장
                    SaveAllKeyBindings();

                    // 팝업 처리: 키 변경 완료 팝업 활성화
                    ShowKeyChangePopupAsync().Forget();

                    isListeningForKey = false;
                    break;
                }
            }
        }
    }



    // PlayerPrefs 기반으로 모든 키 바인딩 저장
    private void SaveAllKeyBindings()
    {
        foreach (var entry in KeySetting.keys)
        {
            PlayerPrefs.SetInt($"KeyBinding_{entry.Key}", (int)entry.Value);
        }
        PlayerPrefs.Save();
    }

    // PlayerPrefs 기반으로 모든 키 바인딩 불러오기
    private void LoadAllKeyBindings()
    {
        foreach (KeyAction action in System.Enum.GetValues(typeof(KeyAction)))
        {
            if (action == KeyAction.KeyCount)
                continue;

            string key = $"KeyBinding_{action}";

            if ((int)action >= keyButtons.Length)
                continue;

            if (PlayerPrefs.HasKey(key))
            {
                // 저장된 키가 있다면 그 값으로 설정
                KeyCode code = (KeyCode)PlayerPrefs.GetInt(key);
                KeySetting.keys[action] = code;
            }
            else
            {
                PlayerPrefs.SetInt(
                    key,
                    (int)KeySetting.keys[action]
                );
                //// 저장된 키가 없다면 기본값 설정
                //KeyCode defaultKey = GetDefaultKeyForAction(action);
                //KeySetting.keys[action] = defaultKey;
                //PlayerPrefs.SetInt(key, (int)defaultKey); // 기본값도 저장
            }

            keyButtons[(int)action].GetComponentInChildren<TMP_Text>().text = KeySetting.keys[action].ToString();
        }

        PlayerPrefs.Save();
    }

    // 중복된 키 확인
    private bool IsKeyCodeAlreadyAssigned(KeyCode keyCode)
    {
        foreach (var entry in KeySetting.keys)
        {
            if (entry.Value == keyCode)
            {
                duplicatedAction = entry.Key;
                pendingKeyCode = keyCode;

                return true;
            }
        }

        return false;
    }

    public void ConfirmDuplicateKey()
    {
        // 기존 키 제거
        KeySetting.keys[duplicatedAction] = KeyCode.None;

        // 새 키 적용
        KeySetting.keys[currentKeyAction] = pendingKeyCode;

        // UI 갱신
        keyButtons[(int)duplicatedAction]
            .GetComponentInChildren<TMP_Text>().text = "None";

        keyButtons[(int)currentKeyAction]
            .GetComponentInChildren<TMP_Text>().text = pendingKeyCode.ToString();

        SaveAllKeyBindings();
    }

    public void CancelDuplicateKey()
    {
        keyBindingPopup[2].SetActive(false);

        isListeningForKey = false;
    }



    #region 팝업 처리

    // 비동기 팝업 처리 함수 (1: 키 변경 완료)
    private async UniTaskVoid ShowKeyChangePopupAsync()
    {
        // 기존 팝업 모두 비활성화
        keyBindingPopup[0].SetActive(false);
        keyBindingPopup[2].SetActive(false);

        // 단축키 변경 완료 팝업 활성화
        keyBindingPopup[1].SetActive(true);

        await UniTask.Delay(2000); // 2초 대기

        keyBindingPopup[1].SetActive(false); // 팝업 비활성화
    }

    // 비동기 팝업 처리 함수 (2: 중복 키 경고)
    private async UniTaskVoid ShowKeyOwnPopupAsync()
    {
        keyBindingPopup[0].SetActive(false); // 키 입력 대기 팝업 비활성화
        keyBindingPopup[2].SetActive(true);  // 중복 경고 팝업 활성화

        await UniTask.Delay(2000); // 2초 대기

        keyBindingPopup[2].SetActive(false); // 중복 경고 팝업 비활성화
    }

    private void OnKeyButtonClick(KeyAction action)
    {
        currentKeyAction = action;
        isListeningForKey = true;
        keyBindingPopup[0].SetActive(true); // 키 변경 대기 팝업 활성화

        Logger.Log($"{action}을 설정하기 위해 아무 키나 눌러주세요.");
    }

    #endregion
}
