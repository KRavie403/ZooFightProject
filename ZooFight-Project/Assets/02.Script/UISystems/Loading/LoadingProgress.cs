using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyUI.Progress;

public class LoadingProgress : MonoBehaviour
{
    public Animator anim;
    [SerializeField]
    private RawImage _uiRawImage;

    void Start()
    {
        LoadLoadingImg();
    }

    public void LoadLoadingImg()
    {
        anim.SetBool("IsRotating", true);
        //Progress.Show("Loading image. . .", ProgressColor.Orange);
        //StartCoroutine("LoadImg");
    }

    //private IEnumerator LoadImg()
    //{
    //    yield return null;

    //    Progress.Hide();
    //    _uiRawImage = Resources.Load<RawImage>("Loading");

    //}
}
