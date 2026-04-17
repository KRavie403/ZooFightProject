using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class KeySettingDecoder : MonoBehaviour
{
    public int[] SavedKeyCodes = new int[(int)KeyAction.KeyCount];


    KeyCode[] LoadKeys = new KeyCode[(int)KeyAction.KeyCount];

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SavedCodeDecode(int[] ints)
    {
        if(ints.Length == (int)KeyAction.KeyCount)
        {
            for (int i = 0; i < ints.Length; i++)
            {
                LoadKeys[i] = (KeyCode)SavedKeyCodes[i];
            }
        }

    }

    public void LoadSaveKeys()
    {
        if(LoadKeys.Length == (int)KeyAction.KeyCount)
        {
            for (int i = 0; i < LoadKeys.Length; i++)
            {
                KeySetting.keys[(KeyAction)i] = LoadKeys[i];
            }
        }
        // SavedKeyCodes = Clientmanager.Inst.ConfigReader.(키코드배열)
    }



}
