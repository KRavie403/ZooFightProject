using UnityEngine;
using UnityEngine.UI;

public class MinimapMovement : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform minimapRect;         // 미니맵 전체 이미지의 RectTransform
    public RectTransform viewportRect;         // 이동하는 뷰포트(플레이어 시야 영역)의 RectTransform

    [Header("Settings")]
    public float moveSpeed = 50f;                 // 뷰포트 이동 속도
    public float screenEdgeBoundary = 20f;  // 마우스가 화면 끝으로 인식되는 거리(px)

    private Vector2 boundaryMin;                 // 뷰포트가 이동 가능한 최소 위치
    private Vector2 boundaryMax;                // 뷰포트가 이동 가능한 최대 위치

    void Update()
    {
        // 미니맵 및 뷰포트의 크기를 기준으로 경계 계산 (해상도 변화 등 대응)
        UpdateBoundaries();

        // 입력 처리
        Vector2 move = GetKeyboardInput() + GetMouseEdgeInput();

        if (move.sqrMagnitude > 1f)
            move.Normalize(); // 대각선 이동 시 속도 보정

        // 이동 처리
        MoveViewport(move);

        // 경계 밖으로 나가지 않도록 제한
        ClampViewportPosition();
    }

    /// <summary>
    /// 경계값을 실시간으로 계산 (미니맵/뷰포트 크기 기반)
    /// </summary>
    void UpdateBoundaries()
    {
        Vector2 minimapSize = minimapRect.rect.size;
        Vector2 viewportSize = viewportRect.rect.size;

        boundaryMin = -minimapSize * 0.5f + viewportSize * 0.5f;
        boundaryMax = minimapSize * 0.5f - viewportSize * 0.5f;
    }

    /// <summary>
    /// 키보드 입력 처리 (WASD, 화살표)
    /// </summary>
    Vector2 GetKeyboardInput()
    {
        float h = 0f, v = 0f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) h -= 1f;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) h += 1f;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) v += 1f;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) v -= 1f;
        return new Vector2(h, v);
    }

    /// <summary>
    /// 마우스가 화면 가장자리에 있을 때 입력 처리
    /// </summary>
    Vector2 GetMouseEdgeInput()
    {
        Vector2 move = Vector2.zero;
        Vector2 mouse = Input.mousePosition;

        if (mouse.x < screenEdgeBoundary) move.x -= 1f;
        else if (mouse.x > Screen.width - screenEdgeBoundary) move.x += 1f;

        if (mouse.y < screenEdgeBoundary) move.y -= 1f;
        else if (mouse.y > Screen.height - screenEdgeBoundary) move.y += 1f;

        return move;
    }

    /// <summary>
    /// 뷰포트를 입력 방향으로 이동
    /// </summary>
    void MoveViewport(Vector2 dir)
    {
        viewportRect.anchoredPosition += dir * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 뷰포트가 미니맵 밖으로 나가지 않도록 Clamp
    /// </summary>
    void ClampViewportPosition()
    {
        Vector2 pos = viewportRect.anchoredPosition;
        pos.x = Mathf.Clamp(pos.x, boundaryMin.x, boundaryMax.x);
        pos.y = Mathf.Clamp(pos.y, boundaryMin.y, boundaryMax.y);
        viewportRect.anchoredPosition = pos;
    }
}
