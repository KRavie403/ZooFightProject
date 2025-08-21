using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiinimapUI : MonoBehaviour
{
    public GameObject background;
    public GameObject minimap;
    public GameObject minimapActivateBtn;
    public GameObject minimapOpenClose;

    public RectTransform minimapPanel; // 맵 전체
    public RectTransform minimapBar;
    [SerializeField] private TextMeshProUGUI mapBtnText;

    private bool _isOpen = true;

    private float minimapRatio = 0.2f;      // 화면 비율
    private Vector2 _openMinimapPos;
    private Vector2 _closedMinimapPos;
    private Vector2 _openBarPos;
    private Vector2 _closedBarPos;


    private void Start()
    {
        // 초기 위치 저장 (좌표는 anchoredPosition 기준)
        _openMinimapPos = minimapPanel.anchoredPosition;
        _closedMinimapPos = new Vector2(_openMinimapPos.x, _openMinimapPos.y - minimapPanel.rect.height);   // 아래로 내려감

        _openBarPos = minimapBar.anchoredPosition;
        _closedBarPos = new Vector2(_openBarPos.x, _openMinimapPos.y); // 맨 위로 옴 (맵 안 보이게)

        AdjustMinimapUI();
    }

    public void ToggleMinimap()
    {
        _isOpen = !_isOpen;

        if (_isOpen)
        {
            minimapPanel.anchoredPosition = _openMinimapPos;
            minimapBar.anchoredPosition = _openBarPos;
            mapBtnText.text = "-";
        }
        else
        {
            minimapPanel.anchoredPosition = _closedMinimapPos;
            minimapBar.anchoredPosition = _closedBarPos;
            mapBtnText.text = "+";
        }
    }

    void AdjustMinimapUI()
    {
        RectTransform rt = minimap.GetComponent<RectTransform>();
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 화면 크기의 비율에 따라 UI 크기를 정함 (예: 화면의 20%)
        rt.sizeDelta = new Vector2(screenWidth * minimapRatio, screenHeight * minimapRatio);
    }
}
