using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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


    private void Start()
    {
        // 초기화 작업, 기본 BGM을 설정하는 등
        PlayBackgroundMusic(SceneManager.GetActiveScene().name);

        // 저장된 볼륨 설정 로드
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        soundEffectVolumeSlider.value = PlayerPrefs.GetFloat("SoundEffectVolume", 0.7f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.7f);

        // 슬라이더 값 변경 시 호출될 메서드 추가
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        soundEffectVolumeSlider.onValueChanged.AddListener(SetSoundEffectVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
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
        // BGM을 설정하는 로직
        AudioClip clip = GetClipForScene(sceneName, bgm);
        if (clip != null)
        {
            BGMSource.clip = clip;
            BGMSource.Play();
        }
#if DEBUG
        else
        {
            Debug.LogWarning("해당 BGM 클립을 찾을 수 없음: " + sceneName);
        }
#endif
    }

    public void PlayUIEffect(int index)
    {
        // UI 사운드를 설정
        if (index >= 0 && index < sfxUI.Length-1)
        {
            SFXSource.PlayOneShot(sfxUI[index]);
        }
        else
        {
            Debug.LogWarning("UI 사운드 이펙트가 범위를 벗어남: " + index);
        }
    }

    //public void PlayMatchingUIEffect(int index)
    //{
    //    // UI 사운드를 설정
    //    if (index >= 0 && index < sfxUI.Length)
    //    {
    //        SFXSource.PlayOneShot(sfxUI[index]);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("UI 사운드 이펙트가 범위를 벗어남: " + index);
    //    }
    //}

    private AudioClip GetClipForScene(string sceneName, AudioClip[] clips)
    {
        // 씬 이름에 따라 적절한 오디오 클립을 반환
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
        // 마스터 볼륨 설정
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume); // 설정 저장
    } 


    private void SetSoundEffectVolume(float volume)
    {
        // SFX 볼륨 설정
        if (SFXSource != null)
        {
            SFXSource.volume = volume;
            PlayerPrefs.SetFloat("SoundEffectVolume", volume); // 설정 저장
        }
    }
    private void SetMusicVolume(float volume)
    {
        // Music 볼륨 설정
        if (BGMSource != null)
        {
            BGMSource.volume = volume;
            PlayerPrefs.SetFloat("MusicVolume", volume); // 설정 저장
        }
    }
}
