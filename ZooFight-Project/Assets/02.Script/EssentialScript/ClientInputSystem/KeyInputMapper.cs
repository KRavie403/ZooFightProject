using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.IO;
using UnityEngine.UIElements;
using TMPro;
using UnityEngine.Events;
using System.Linq;

public enum KeyAction
{
    Forward=0,Backward,Left,Right,
    Jump,Attack,Selectskill,Usingskill,Grab,
    Menu,Run,
    ItemCreate,
    // 맵열기 = M
    KeyCount
}

public static class KeySetting
{
    public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>();
}


public class KeyInputMapper : MonoBehaviour
{
    KeyCode[] defaultKeys = new KeyCode[]
    {
        KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D,
        KeyCode.Space, KeyCode.Mouse0, KeyCode.E, KeyCode.F,KeyCode.Mouse1,
        KeyCode.Escape ,KeyCode.LeftShift,
        KeyCode.Q

    };

    private void Awake()
    {
        for (int i = 0; i < (int)KeyAction.KeyCount; i++)
        {
            KeySetting.keys.Add((KeyAction)i, defaultKeys[i]);
            Debug.Log("keys" + KeySetting.keys);
        }


    }




    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {





        if (Input.anyKeyDown)
        {



        }


    }

    public void KeycodeToInt(string keycode)
    {



    }

    public void TargetKeySetting(KeyAction KeyType,KeyCode keyCode) 
    {
        KeySetting.keys[KeyType] = keyCode;
    }

}
