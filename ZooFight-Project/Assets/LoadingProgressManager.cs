using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System;

public class LoadingProgressManager : Singleton<LoadingProgressManager>
{
    public GameObject LoadingBG;
    public GameObject LoadingImage;
    public GameObject LoadingText;

    public static LoadingProgressManager GetInstance()
    {
        if (Inst == null)
        {
            Logger.LogError("LoadingManager 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return Inst;
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        LoadingBG.SetActive(true);
        LoadingImage.SetActive(true);
        LoadingText.SetActive(true);

        AudioManager.Inst.PlayBackgroundMusic("LoadingScene");

        LoadingProgress.Inst.LoadLoadingImg();
    }

    public void Hide()
    {
        LoadingBG.SetActive(false);
        LoadingImage.SetActive(false);
        LoadingText.SetActive(false);
    }

}
