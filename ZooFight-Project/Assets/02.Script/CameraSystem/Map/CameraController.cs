using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Cameras")]
    public Camera MainCamera;         // 메인 카메라
    public Camera PlayerCamera;       // 2,3 유저 카메라
    private int _newPriority = -2;            // 변경하고자 하는 새로운 우선순위 값

    public int curUser = 0;

    private void Awake()
    {
        MainCamera.GetComponent<Camera>().depth = -1;
        PlayerCamera.GetComponent<Camera>().depth = -1;
        GetUser(curUser);
    }

    // 유저 번호 불러오기
    private void GetUser(int curUser)
    {
        selectCameraPriority(curUser);
    }

    private void selectCameraPriority(int curUser)
    {
        switch (curUser)
        {
            case 0:
                if (MainCamera != null)
                {
                    // 카메라의 우선순위를 변경
                    MainCamera.GetComponent<Camera>().depth = _newPriority;
                }
                else
                {
                    Logger.LogError("카메라가 할당되지 않았습니다.");
                }
                break;
            case 1:
            case 2:
                if (PlayerCamera != null)
                {
                    // 카메라의 우선순위를 변경
                    PlayerCamera.GetComponent<Camera>().depth = _newPriority;
                }
                else
                {
                    Logger.LogError("카메라가 할당되지 않았습니다.");
                }
                break;
            default:
                Logger.LogError("유저 번호가 할당되지 않았습니다.");
                break;
        }
    }
}
