using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSwitch : MonoBehaviour
{
    [Header("UI Buttons")]
    public GameObject switchButton;
    public GameObject selectButton;

    [Header("Cameras")]
    public CinemachineVirtualCamera camA;    // 원래 있던 카메라
    public CinemachineVirtualCamera camB;    // 이동할 카메라
    public float transitionSpeed = 2f;                // 카메라 전환 속도
    [SerializeField] private CinemachineVirtualCamera _activeCam; // 현재 활성화된 카메라

    private void Start()
    {
        _activeCam = camA;    // 시작할 때 camA를 기본 카메라로 설정
        camA.Priority = 10;      // 우선 순위를 높게 설정
        camB.Priority = 0;       // 두 번째 카메라는 우선 순위를 낮게 설정
    }

    public void SwitchOn() => SetActiveCamera(camB);
    public void SwitchOff() => SetActiveCamera(camA);

    /// <summary>
    /// 지정한 카메라를 활성 카메라로 설정
    /// </summary>
    void SetActiveCamera(CinemachineVirtualCamera activeCamera)
    {
        camA.Priority = activeCamera == camA ? 10 : 0;
        camB.Priority = activeCamera == camB ? 10 : 0;
    }
}
