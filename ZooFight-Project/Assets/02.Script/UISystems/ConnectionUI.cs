using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class ConnectionUI : Singleton<ConnectionUI>
{
    public GameObject connectionStatus;
    public TMPro.TextMeshProUGUI connectionText;

    private CancellationTokenSource uiCTS;

    private void Start()
    {
        BackEndServerManager.GetInstance().OnConnectionStatusChanged += UpdateUIConnectionStatus;
    }

    private void UpdateUIConnectionStatus(bool isConnected)
    {
        uiCTS?.Cancel();
        uiCTS = new CancellationTokenSource();
        ShowConnectionTemporary(isConnected, uiCTS.Token).Forget();
    }

    private async UniTaskVoid ShowConnectionTemporary(bool isConnected, CancellationToken token)
    {
        connectionText.text = isConnected ? "연결됨" : "재접속 중 . . .";
        connectionText.color = isConnected ? Color.green : Color.red;

        connectionStatus.SetActive(true);

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f), cancellationToken: token);
        }
        catch { return; }

        connectionStatus.SetActive(false);
    }
}
