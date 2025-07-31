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
        LoadLoadingScene().Forget();
    }


    private async UniTask LoadLoadingScene()
    {
        // 현재 로딩 씬이 이미 로드되어 있는 경우 언로드
        if (SceneManager.GetSceneByName("LoadingScene").isLoaded)
        {
            await SceneManager.UnloadSceneAsync("LoadingScene");
        }

        // 로딩 씬을 비동기적으로 로드
        AsyncOperation loadingSceneOp = SceneManager.LoadSceneAsync("LoadingScene");
        loadingSceneOp.allowSceneActivation = true; // 씬 활성화를 제어

        // 로딩 씬이 로드될 때까지 기다림
        while (!loadingSceneOp.isDone)
        {
            await UniTask.Yield(); // 프레임마다 대기하여 로딩 진행 상황을 체크
        }

        // 게임 씬을 비동기적으로 로드
        await LoadGameSceneAsync();
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
