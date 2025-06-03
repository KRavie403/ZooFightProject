using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class AudioSettings
{
    public float masterVol = 0.8f;
    public float sfxVol = 0.8f;
    public float musicVol = 0.8f;
}

public class AudioManager : Singleton<AudioManager>
{
    public Slider masterVolumeSlider;
    public Slider soundEffectVolumeSlider;
    public Slider musicVolumeSlider;

    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource BGMSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---------- Audio Clip ----------")]
    public AudioClip[] bgm;
    public AudioClip[] sfxBasic;
    public AudioClip[] sfxItemUse;
    public AudioClip[] sfxUI;

    private AudioSettings audioSettings;
    private string settingsPath;


    private void Start()
    {
        settingsPath = Path.Combine(Application.persistentDataPath, "audioSettings.json");
        LoadSettings();

        PlayBackgroundMusic(SceneManager.GetActiveScene().name);

        // 슬라이더 초기값 설정
        masterVolumeSlider.value = audioSettings.masterVol;
        soundEffectVolumeSlider.value = audioSettings.sfxVol;
        musicVolumeSlider.value = audioSettings.musicVol;

        // 리스너 연결
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        soundEffectVolumeSlider.onValueChanged.AddListener(SetSoundEffectVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

        UpdateAllVolumes();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBackgroundMusic(scene.name);
    }

    public void PlayBackgroundMusic(string sceneName)
    {
        AudioClip clip = GetClipForScene(sceneName, bgm);
        if (clip != null)
        {
            BGMSource.clip = clip;
            BGMSource.Play();
        }
        else
        {
            Debug.LogWarning("해당 BGM 클립을 찾을 수 없음: " + sceneName);
        }
    }

    public void PlayUIEffect(int index)
    {
        if (index >= 0 && index < sfxUI.Length)
        {
            SFXSource.PlayOneShot(sfxUI[index]);
        }
        else
        {
            Debug.LogWarning("UI 사운드 이펙트가 범위를 벗어남: " + index);
        }
    }

    private AudioClip GetClipForScene(string sceneName, AudioClip[] clips)
    {
        foreach (var clip in clips)
        {
            if (clip.name == sceneName)
            {
                return clip;
            }
        }
        return null;
    }

    private void SetMasterVolume(float volume)
    {
        audioSettings.masterVol = volume;
        SaveSettings();
        UpdateAllVolumes();
    }

    private void SetSoundEffectVolume(float volume)
    {
        audioSettings.sfxVol = volume;
        SaveSettings();
        UpdateAllVolumes();
    }

    private void SetMusicVolume(float volume)
    {
        audioSettings.musicVol = volume;
        SaveSettings();
        UpdateAllVolumes();
    }

    private void UpdateAllVolumes()
    {
        float scaledMaster = audioSettings.masterVol * 0.2f;

        AudioListener.volume = scaledMaster;

        if (SFXSource != null)
            SFXSource.volume = scaledMaster * audioSettings.sfxVol;

        if (BGMSource != null)
            BGMSource.volume = scaledMaster * audioSettings.musicVol;
    }

    private void LoadSettings()
    {
        if (File.Exists(settingsPath))
        {
            string json = File.ReadAllText(settingsPath);
            audioSettings = JsonUtility.FromJson<AudioSettings>(json);
        }
        else
        {
            audioSettings = new AudioSettings(); // 기본값으로 초기화
            SaveSettings();
        }
    }

    private void SaveSettings()
    {
        string json = JsonUtility.ToJson(audioSettings, true);
        File.WriteAllText(settingsPath, json);
    }
}