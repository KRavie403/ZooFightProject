using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Clientmanager : MonoBehaviour
{

    private static Clientmanager inst;
    public static Clientmanager Inst => inst;

    public ConfigReader ConfigReader;





    private void Awake()
    {
        if(inst == null)
        {
            inst = FindObjectOfType<Clientmanager>();
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (inst != null) Destroy(this.gameObject);
        }

        // 60프레임 고정
        Application.targetFrameRate = 60;
        // 게임중 슬립모드 해제
        Screen.sleepTimeout = SleepTimeout.NeverSleep;



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
