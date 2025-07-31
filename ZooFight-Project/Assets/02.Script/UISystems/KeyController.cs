using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyController : Singleton<KeyController>
{
    public Button[] keyButtons;
    public KeySettingDecoder InputSettingDecoder;
    //public KeySettingDecoder InputSettingDecoder;
    public GameObject[] keyBindingPopup; // 0: 기본 팝업, 1: 키 변경 완료 팝업, 2: 중복 경고 팝업

    private KeyAction currentKeyAction;
    private bool isListeningForKey = false; // 키 입력 대기 중인지 여부

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
            keyButtons[i].GetComponentInChildren<TMP_Text>().text =KeySetting.keys[action].ToString();
            keyButtons[i].onClick.AddListener(() => OnKeyButtonClick(action));
        }

        // 저장된 키 설정을 불러오기
        LoadSavedKeys();
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
                    // 중복된 키가 아닌지 확인
                    if (IsKeyCodeAlreadyAssigned(keyCode))
                    {
                        Debug.LogWarning($"{keyCode} 키는 이미 할당된 키입니다.");
                        keyBindingPopup[0].SetActive(false); // 기존 팝업 닫기
                        keyBindingPopup[2].SetActive(true); // 중복 경고 팝업 활성화
                        return;
                    }

                    // 키 설정을 변경하고 UI 텍스트 업데이트
                    KeySetting.keys[currentKeyAction] = keyCode;
                    keyButtons[(int)currentKeyAction].GetComponentInChildren<TMP_Text>().text = keyCode.ToString();

                    // 변경된 키 저장
                    SaveKeySetting(currentKeyAction, keyCode);

                    // 팝업 처리: 키 변경 완료 팝업 활성화
                    keyBindingPopup[0].SetActive(false); // 기존 팝업 닫기
                    keyBindingPopup[1].SetActive(true); // 키 변경 완료 팝업 활성화

                    isListeningForKey = false;
                    break;
                }
            }
        }
    }

    private void OnKeyButtonClick(KeyAction action)
    {
        currentKeyAction = action;
        isListeningForKey = true;
        keyBindingPopup[0].SetActive(true); // 키 변경 대기 팝업 활성화
#if DEBUG || UNITY_EDITOR
        Debug.Log($"{action}을 설정하기 위해 아무 키나 눌러주세요.");
#endif
    }

    private void SaveKeySetting(KeyAction action, KeyCode keyCode)
    {
        if (InputSettingDecoder != null)
        {
            InputSettingDecoder.SavedKeyCodes[(int)action] = (int)keyCode;
        }
    }

    private bool IsKeyCodeAlreadyAssigned(KeyCode keyCode)
    {
        // 현재 설정된 키들 중에서 이미 사용 중인 키코드가 있는지 확인
        foreach (var entry in KeySetting.keys)
        {
            if (entry.Value == keyCode)
            {
                // 중복된 키가 있을 경우 기존 키를 비움
                KeySetting.keys[entry.Key] = KeyCode.None;
                keyButtons[(int)entry.Key].GetComponentInChildren<TMP_Text>().text = " ";
                return true;
            }
        }
        return false; // 중복된 키코드가 없음
    }

    public void LoadSavedKeys()
    {
        if (InputSettingDecoder != null)
        {
            InputSettingDecoder.SavedCodeDecode(InputSettingDecoder.SavedKeyCodes);

            // 저장된 키 값을 UI에 반영
            for (int i = 0; i < keyButtons.Length; i++)
            {
                KeyAction action = (KeyAction)i;
                KeyCode keyCode = (KeyCode)InputSettingDecoder.SavedKeyCodes[i];
                KeySetting.keys[action] = keyCode;
                keyButtons[i].GetComponentInChildren<TMP_Text>().text = keyCode.ToString();
            }
        }
#if DEBUG
        else
        {
            Debug.LogWarning("InputSettingDecoder가 할당되지 않음");
        }
#endif
    }
}
