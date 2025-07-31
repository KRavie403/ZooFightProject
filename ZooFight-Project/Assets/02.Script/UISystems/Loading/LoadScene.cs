using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System;

public class LoadScene : Singleton<LoadScene>
{

    public void OnStartGameButtonClicked()
    {
        // 로딩 씬을 비동기적으로 로드하고, 이후 게임 씬을 로드
        LoadGameSceneAsync().Forget();
    }

    private async UniTask LoadGameSceneAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("GameScene");
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            await UniTask.Yield();
            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
                return;
            }
        }
    }
}
