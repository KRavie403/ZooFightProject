using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 클라이언트 실행시 불러와야할 요소들을 생성 및 호출구조를 연결시켜주는동작을 함
/// </summary>
public class InitateLoader : MonoBehaviour
{
    bool isInitateLoading = false;



    private void Awake()
    {

        // 클라이언트 매니저 생성
        //GameObject clientmanagerobj = Instantiate(Resources.Load("ManagerObjs/ClientManager") as GameObject);



        //// 각 매니저 호출
        //GameObject clientinputmanagerobj = Instantiate(Resources.Load("ManagerObjs/ClientInputManager") as GameObject);
        //Clientmanager.Inst.InputManager = clientinputmanagerobj.GetComponent<ClientInputManager>();

        //GameObject soundmanagerobj = Instantiate(Resources.Load("ManagerObjs/SoundManager") as GameObject);
        //Clientmanager.Inst.soundManager = clientinputmanagerobj.GetComponent<SoundManager>();

        //GameObject effectmanagerobj = Instantiate(Resources.Load("ManagerObjs/EffectManager") as GameObject);
        //Clientmanager.Inst.effectManager = clientinputmanagerobj.GetComponent<EffectManager>();

        //GameObject audiomanagerobj = Instantiate(Resources.Load("ManagerObjs/AudioManager") as GameObject);
        //Clientmanager.Inst.audioManager = clientinputmanagerobj.GetComponent<AudioManager>();

        //GameObject uimanagerobj = Instantiate(Resources.Load("ManagerObjs/UiManager") as GameObject);
        //Clientmanager.Inst.uiManager = clientinputmanagerobj.GetComponent<UIManager>();


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // 최초 로딩 완료시 로그인 씬으로 전환시킴
        if (isInitateLoading == false)
        {

        }
        else
        {

        }

    }
}
