using UnityEngine;
using UnityEngine.UI;

public class MinimapViewport : MonoBehaviour
{
    public Camera playerCamera;             // 뷰포트용 카메라 (확대된 카메라)
    public Camera minimapCamera;        // 전체맵용 카메라 (축소된 카메라)
    public RectTransform viewportRect;  // 뷰포트 이미지 RectTransform
    public RectTransform minimapRect;  // 뷰포트 이미지 RectTransform
    
    private float minimapRatio = 0.2f;      // 화면 비율

    private void Update()
    {
        UpdateViewport();
    }

    private void UpdateViewport()
    {
        float minimapCameraSize = minimapCamera.orthographicSize;
        float playerCameraSize = playerCamera.orthographicSize;

        float ratio = playerCameraSize / minimapCameraSize;

        // height는 minimap UI 높이에 비례
        float height = minimapRect.rect.height * ratio;
        // width는 playerCamera의 aspect 기준
        float width = height * playerCamera.aspect;

        viewportRect.sizeDelta = new Vector2(width, height);

    }

    //private void UpdateViewport()
    //{
    //    RectTransform rt = viewportRect.GetComponent<RectTransform>();

    //    float playerSize = playerCamera.orthographicSize;
    //    float minimapSize = minimapCamera.orthographicSize;

    //    float playerAspect = playerCamera.aspect;
    //    float minimapAspect = minimapCamera.aspect;

    //    float widthScale = (playerSize * playerAspect) / (minimapSize * minimapAspect);
    //    float heightScale = playerSize / minimapSize;

    //    float screenWidth = Screen.width;
    //    float screenHeight = Screen.height;


    //    // 화면 크기의 비율에 따라 UI 크기를 정함 (예: 화면의 20%)
    //    rt.sizeDelta = new Vector2(screenWidth * minimapRatio * widthScale,
    //                           screenHeight * minimapRatio * heightScale);
    //}
}
