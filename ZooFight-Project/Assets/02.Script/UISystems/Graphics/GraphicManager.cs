using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class GraphicManager : Singleton<GraphicManager>
{
    [Header("Graphic UI")]
    [SerializeField] TMP_Text graphicsCardName;
    [SerializeField] TMP_Dropdown displayModeDropdown;
    [SerializeField] TMP_Dropdown resolutionDropdown;
    [SerializeField] Slider brightnessSlider;
    [SerializeField] Image overlay;

    private List<Resolution> resolutions = new List<Resolution>();
    private int resolutionIndex;
    private FullScreenMode screenMode;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitGPUInfo();
        InitDisplayModeDropdown();
        InitResolutionDropdown();

        LoadSettings();

        displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);

        Logger.Log("그래픽 매니저 초기화 완료");
    }

    // ===================== 초기화 =====================

    void InitGPUInfo()
    {
        if (graphicsCardName != null)
        {
            graphicsCardName.text = SystemInfo.graphicsDeviceName;
            Logger.Log($"그래픽 카드 감지: {SystemInfo.graphicsDeviceName}");
        }
    }

    void InitDisplayModeDropdown()
    {
        displayModeDropdown.options.Clear();
        displayModeDropdown.options.Add(new TMP_Dropdown.OptionData("창 모드"));
        displayModeDropdown.options.Add(new TMP_Dropdown.OptionData("전체 화면"));
        displayModeDropdown.options.Add(new TMP_Dropdown.OptionData("테두리 없는 창 모드"));
        displayModeDropdown.RefreshShownValue();
    }

    void InitResolutionDropdown()
    {
        resolutions.Clear();
        HashSet<string> added = new HashSet<string>();

        foreach (Resolution r in Screen.resolutions)
        {
            string key = $"{r.width}x{r.height}@{Mathf.RoundToInt((float)r.refreshRateRatio.value)}";

            if (!added.Contains(key))
            {
                added.Add(key);
                resolutions.Add(r);
            }
        }

        resolutions.Sort((a, b) =>
        {
            int cmp = (a.width * a.height).CompareTo(b.width * b.height);
            return cmp != 0 ? cmp : a.refreshRateRatio.value.CompareTo(b.refreshRateRatio.value);
        });

        resolutionDropdown.options.Clear();

        for (int i = 0; i < resolutions.Count; i++)
        {
            var r = resolutions[i];
            string text = $"{r.width} x {r.height} ({Mathf.RoundToInt((float)r.refreshRateRatio.value)}Hz)";
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(text));
        }

        resolutionIndex = resolutions.Count - 1;
        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    // ===================== 모드 변경 =====================

    void OnDisplayModeChanged(int index)
    {
        switch (index)
        {
            case 0:
                screenMode = FullScreenMode.Windowed;
                Logger.Log("디스플레이 모드: 창 모드");
                break;

            case 1:
                screenMode = FullScreenMode.FullScreenWindow;
                Logger.Log("디스플레이 모드: 전체 화면");
                break;

            case 2:
                screenMode = FullScreenMode.Windowed;
                Logger.Log("디스플레이 모드: 테두리 없는 창 모드");
                break;
        }

        ApplyResolution().Forget();
        SaveSettings();
    }

    void OnResolutionChanged(int index)
    {
        resolutionIndex = index;

        var r = resolutions[index];

        Logger.Log($"해상도 선택: {r.width}x{r.height} ({r.refreshRateRatio.value}Hz)");

        ApplyResolution().Forget();
        ApplyFrameRate(r); // FPS 동기화

        SaveSettings();
    }

    void OnBrightnessChanged(float value)
    {
        overlay.color = new Color(0, 0, 0, 0.5f - value * 0.5f);
        PlayerPrefs.SetFloat("Brightness", value);
    }

    // ===================== 핵심 로직 =====================

    async UniTaskVoid ApplyResolution()
    {
        Resolution r;

        // 전체화면만 모니터 해상도 강제
        if (displayModeDropdown.value == 1)
        {
            r = Screen.currentResolution;
        }
        else
        {
            r = resolutions[resolutionIndex];
        }

        Screen.SetResolution(r.width, r.height, screenMode);

        Logger.Log($"해상도 적용: {r.width}x{r.height} / Mode:{screenMode}");

        // Unity 내부 처리 끝까지 대기
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

        // Borderless 적용
        if (displayModeDropdown.value == 2)
        {
            SetBorderlessWindowedMode();
        }
    }

    void ApplyFrameRate(Resolution r)
    {
        int hz = Mathf.RoundToInt((float)r.refreshRateRatio.value);

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = hz;

        Logger.Log($"FPS 제한 적용: {hz}");
    }

    // ===================== 저장 =====================

    void SaveSettings()
    {
        PlayerPrefs.SetInt("DisplayMode", displayModeDropdown.value);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        if (!PlayerPrefs.HasKey("DisplayMode"))
        {
            // 기본값 설정 (전체화면)
            displayModeDropdown.SetValueWithoutNotify(1);
            resolutionIndex = resolutions.Count - 1;
            resolutionDropdown.SetValueWithoutNotify(resolutionIndex);
            screenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            // 저장된 값 불러오기
            int savedMode = PlayerPrefs.GetInt("DisplayMode");
            resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");

            displayModeDropdown.SetValueWithoutNotify(savedMode);
            resolutionDropdown.SetValueWithoutNotify(resolutionIndex);

            // 정수 값을 FullScreenMode 열거형으로 변환
            screenMode = (savedMode == 1) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        }

        // 설정값 적용 시점 명확화
        ApplyResolution().Forget();

        // 해상도 리스트가 비어있지 않은지 체크 후 프레임 레이트 적용
        if (resolutions.Count > resolutionIndex)
            ApplyFrameRate(resolutions[resolutionIndex]);
    }

    // ===================== WinAPI =====================

    const int GWL_STYLE = -16;
    const uint WS_POPUP = 0x80000000;
    const uint WS_CAPTION = 0x00C00000;
    const uint WS_THICKFRAME = 0x00040000;
    const uint WS_MINIMIZEBOX = 0x00020000;
    const uint WS_MAXIMIZEBOX = 0x00010000;
    const uint WS_SYSMENU = 0x00080000;

    const uint SWP_NOMOVE = 0x0002;
    const uint SWP_NOSIZE = 0x0001;
    const uint SWP_NOZORDER = 0x0004;
    const uint SWP_FRAMECHANGED = 0x0020;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    static extern int GetWindowLong(System.IntPtr hWnd, int nIndex);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    static extern int SetWindowLong(System.IntPtr hWnd, int nIndex, uint dwNewLong);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    static extern bool SetWindowPos(System.IntPtr hWnd, System.IntPtr hWndInsertAfter,
        int X, int Y, int cx, int cy, uint uFlags);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    static extern System.IntPtr GetActiveWindow();

    void SetBorderlessWindowedMode()
    {
        if (Application.isEditor) return;

        var hwnd = GetActiveWindow();
        uint style = (uint)GetWindowLong(hwnd, GWL_STYLE);

        style &= ~(WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
        style |= WS_POPUP;

        SetWindowLong(hwnd, GWL_STYLE, style);

        SetWindowPos(hwnd, System.IntPtr.Zero, 0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);

        Logger.Log("Borderless 적용 완료");
    }
}