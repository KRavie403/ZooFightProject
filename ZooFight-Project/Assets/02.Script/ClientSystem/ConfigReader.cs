using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Config
{
    public GraphicSetting GraphicSetting;
    public SoundSetting SoundSetting;
    public KeyboardSetting KeyboardSetting;

}

[System.Serializable]
public class GraphicSetting
{
    public Vector2 resolution;
}

[System.Serializable]
public class SoundSetting
{

    public float Totalvolume;
    public float Bgm;
    public float EffectVolume;
    public float BgmVolume;
}

[System.Serializable]
public class KeyboardSetting
{

}


public class ConfigReader : MonoBehaviour
{





    private void Awake()
    {
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
