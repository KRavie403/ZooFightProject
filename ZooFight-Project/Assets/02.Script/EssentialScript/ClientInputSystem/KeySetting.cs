using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.IO;


public enum KeyAction
{
    Forward = 0, Backward, Left, Right,
    Jump, Run, Attack, Selectskill, Usingskill, Grab,
    Discard,
    Map,
    Menu,
    ItemCreate,     // ItemCreate(테스트용)
    KeyCount
}

#region 기존 (확인 후 제거)
//public enum KeyAction
//{
//    Forward = 0, Backward, Left, Right,
//    Jump, Attack, Selectskill, Usingskill, Grab,
//    Menu, Run,
//    ItemCreate, PickupOrHold, Equip, Drop,
//    // 맵열기 = M
//    KeyCount
//}

//public static class KeySetting
//{
//    public static Dictionary<KeyAction, KeyCode> keys = new Dictionary<KeyAction, KeyCode>();
//}
#endregion

public static class KeySetting
{
    public static readonly Dictionary<KeyAction, KeyCode> keys;

    static readonly KeyCode[] defaultKeys =
    {
            KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D,
            KeyCode.Space, KeyCode.LeftShift, KeyCode.Mouse0, KeyCode.E, KeyCode.F, KeyCode.Mouse1,
            KeyCode.R,
            KeyCode.M,
            KeyCode.Escape,
            KeyCode.Q,
            KeyCode.None
    };

    static KeySetting()
    {
        keys = new Dictionary<KeyAction, KeyCode>();

        for (int i = 0; i < (int)KeyAction.KeyCount; i++)
        {
            keys[(KeyAction)i] = defaultKeys[i];
        }

        Logger.Log("KeySetting initialized");
    }


    #region 기존 (확인 후 제거)
    //public class KeyInputMapper : MonoBehaviour
    //{
    //    KeyCode[] defaultKeys = new KeyCode[]
    //    {
    //        KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D,
    //        KeyCode.Space, KeyCode.LeftShift, KeyCode.Mouse0, KeyCode.E, KeyCode.F, KeyCode.Mouse1, 
    //        KeyCode.R,
    //        KeyCode.M,
    //        KeyCode.Escape,
    //        KeyCode.Q,
    //        KeyCode.None
    //    };

    //    private void Awake()
    //    {
    //        for (int i = 0; i < (int)KeyAction.KeyCount; i++)
    //        {
    //            KeySetting.keys.Add((KeyAction)i, defaultKeys[i]);
    //            Debug.Log("keys" + KeySetting.keys);
    //        }

    //    }


    //    public void KeycodeToInt(string keycode)
    //    {

    //    }
    //    public void TargetKeySetting(KeyAction KeyType,KeyCode keyCode) 
    //    {
    //        KeySetting.keys[KeyType] = keyCode;
    //    }

    #endregion(수정함)
}
