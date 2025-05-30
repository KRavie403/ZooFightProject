using UnityEngine;

public class AudioBootstrapper : Singleton<AudioBootstrapper>
{
    [SerializeField] private GameObject audioManagerPrefab;

    private void Awake()
    {
        if (FindObjectOfType<AudioManager>() == null)
        {
            GameObject obj = Instantiate(audioManagerPrefab);
            obj.name = "SoundManager";
        }
    }
}
