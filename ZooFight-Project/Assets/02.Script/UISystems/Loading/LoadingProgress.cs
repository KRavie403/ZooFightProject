using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyUI.Progress;

public class LoadingProgress : Singleton<LoadingProgress>
{
    public Animator anim;
    [SerializeField]
    private RawImage _uiRawImage;

    public static LoadingProgress GetInstance()
    {
        if (Inst == null)
        {
            Logger.LogError("LoadingProgress 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return Inst;
    }

    public void LoadLoadingImg()
    {
        Logger.Log("LoadLoadingImg");
        if (anim == null)
        {
            Logger.LogError("Animator가 할당되지 않았습니다.");
            return;
        }

        anim.SetBool("IsRotating", true);
    }

    //private IEnumerator LoadImg()
    //{
    //    yield return null;

    //    Progress.Hide();
    //    _uiRawImage = Resources.Load<RawImage>("Loading");

    //}
}
