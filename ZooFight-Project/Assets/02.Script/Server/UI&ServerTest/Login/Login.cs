using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;
using BackEnd;
using Unity.VisualScripting;
using static BackEnd.SendQueue;

public class Login : LoginBase
{
    [SerializeField]
    private Image imageID;
    [SerializeField]
    private TMP_InputField inputFieldID;
    [SerializeField]
    private Image imagePW;
    [SerializeField]
    private TMP_InputField inputFieldPW;

    [SerializeField]
    private Button btnLogin;

    [SerializeField]
    private GameObject LoginSuccess;

    public void OnClickLogin()
    {
        ResetUI(imageID, imagePW);

        if (isFieldDataEmpty(imageID, inputFieldID.text, "아이디")) return;
        if (isFieldDataEmpty(imagePW, inputFieldPW.text, "비밀번호")) return;

        btnLogin.interactable = false;

        StartCoroutine(nameof(LoginProcess));

        ResponseToLogin(inputFieldID.text, inputFieldPW.text);
    }

    private void ResponseToLogin(string ID,string PW)
    {
        Backend.BMember.CustomLogin (ID, PW, callback =>
        {
            StopCoroutine(nameof(LoginProcess));

            if (callback.IsSuccess())
            {
                Backend.BMember.GetUserInfo(nickNameCallback =>
                {
                    string nickname = nickNameCallback.GetReturnValuetoJSON()["row"]["nickname"].ToString();
                    SetMessage($"{nickname}님 환영합니다.");
                    Action<bool, string> func = (success, message) =>
                    {
                        success = true;
                        message = $"{nickname} 로그인 성공.";
                    };
                    BackEndMatchManager.GetInstance().GetMatchList(func);
                });
                OnLoadGameScene().Forget();
            }
            else
            {
                btnLogin.interactable = true;
                string message = string.Empty;
                
                switch (int.Parse(callback.GetStatusCode()))
                {
                    case 401:
                        message = callback.GetMessage().Contains("customId") ? "존재하지 않는 아이디입니다." : "잘못된 비밀번호 입니다.";
                        break;
                    case 403:
                        message = callback.GetMessage().Contains("user") ? "차단당한 유저입니다." : "차단당한 계정입니다.";
                        break;
                    case 410:
                        message = "탈퇴가 진행중인 유저입니다.";
                        break;
                    default:
                        message = callback.GetMessage();
                        break;
                }


                if (message.Contains("비밀번호"))
                {
                    GuideForIncorrectlyEnteredData(imagePW, message);
                }
                else
                {
                    GuideForIncorrectlyEnteredData(imageID, message);
                }
            }
        });
    }

    private IEnumerator LoginProcess()
    {
        float time = 0;

        while (true)
        {
            time += Time.deltaTime;

            SetMessage($"로그인 중입니다 . . . {time:F1}");

            yield return null;
        }
    }

    private async UniTaskVoid OnLoadGameScene()
    {
        await UniTask.Yield();

        //AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("TestServer");
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("MainMenuScene");
        loadSceneAsync.allowSceneActivation = false;

        while (!loadSceneAsync.isDone)
        {
            await UniTask.Yield();

            if (loadSceneAsync.progress >= 0.9f)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

                loadSceneAsync.allowSceneActivation = true;
            }
        }
    }
}
