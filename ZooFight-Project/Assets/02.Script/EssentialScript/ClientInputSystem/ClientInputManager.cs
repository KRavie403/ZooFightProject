using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Searcher;
using UnityEngine;

public class ClientInputManager : Singleton<ClientInputManager>
{

    public KeyInputMapper InputMapper;

    public KeySettingDecoder InputSettingDecoder;

    public Vector3 WorldMousePos;
    public Vector2 ScreenMousePos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool isKeyDown;

    // Update is called once per frame
    void Update()
    {
        // 인터넷 팅김시 입력제한 걸기
        // if(???)
        InputKeyDown();
        InputKeyStay();
        InputKeyUp();
        MouseAxis();

        if(Input.GetMouseButtonDown(0))
        {
            //RangeUse(RangeTypeSetting.keys[0].GetComponent<IRangeEvent>().comp);
            //ItemSystem.Inst.UseItem(Gamemanager.Inst.currentPlayer.curItems);
        }
        
    }

    public void MouseAxis()
    {
        Gamemanager.Inst.currentPlayer.TargetCamera.AxisX = Input.GetAxis("Mouse X");
        Gamemanager.Inst.currentPlayer.TargetCamera.AxisY = Input.GetAxis("Mouse Y");

        //Gamemanager.Inst.currentPlayer.GetComponentInChildren<CharacterCamera>().AxisX
        //    = Input.GetAxis("Mouse X");
        //Gamemanager.Inst.currentPlayer.GetComponentInChildren<CharacterCamera>().AxisY
        //    = Input.GetAxis("Mouse Y");
    }

    public void RangeUse(Component rangeChecker)
    {
        if(rangeChecker.GetComponent<IRangeEvent>() != null)
        {
            Ray ray = Gamemanager.Inst.currentPlayer.TargetCamera.myCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 
                rangeChecker.GetComponent<IRangeEvent>().RouteStart(hit.point);
            }
            //rangeChecker.GetComponent<IRangeEvent>().RouteStart(Gamemanager.Inst.currentPlayer.TargetCamera.myCam.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    public void InputKeyDown()
    {

        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Forward]))
        {
            Debug.Log("Down");
        }
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Backward]))
        {

        }
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Left]))
        {

        }
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Right]))
        {

        }
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Jump]))
        {
            Gamemanager.Inst.currentPlayer.Jump();
            //Gamemanager.Inst.currentPlayer.CharacterJump();
        }
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Attack]))
        {

        }
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Selectskill]))
        {

            if (Gamemanager.Inst.currentPlayer == null) return;
            Gamemanager.Inst.currentPlayer.ItemReady();

            //ItemSystem.Inst.RangeViewer.gameObject.SetActive(!(ItemSystem.Inst.RangeViewer.gameObject.activeSelf));
        }
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Usingskill]))
        {
            if (Gamemanager.Inst.currentPlayer == null) return;
            Gamemanager.Inst.currentPlayer.ItemUse();
        }
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Grab]))
        {
            Gamemanager.Inst.currentPlayer.Grab();
        }
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Menu]))
        {

        }
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.Run]))
        {
            Gamemanager.Inst.currentPlayer.SetRunning(true);
        }
        if (Input.GetKeyDown(KeySetting.keys[KeyAction.ItemCreate]))
        {
            Gamemanager.Inst.currentPlayer.GetItem();
            //ItemSystem.Inst.GiveItem(Gamemanager.Inst.currentPlayer,ItemSystem.Inst.RandomItemSelect());
        }

    }

    public void InputKeyStay()
    {

        if (Input.GetKey(KeySetting.keys[KeyAction.Forward]))
        {
            Debug.Log("Stay");
            Gamemanager.Inst.currentPlayer.AxisY = Gamemanager.Inst.currentPlayer.isKeyReverse ?
                Gamemanager.Inst.currentPlayer.AxisY - Time.deltaTime :
                Gamemanager.Inst.currentPlayer.AxisY + Time.deltaTime;

            //Gamemanager.Inst.currentPlayer.AxisY += Time.deltaTime;
        }
        if (Input.GetKey(KeySetting.keys[KeyAction.Backward]))
        {
            Gamemanager.Inst.currentPlayer.AxisY = Gamemanager.Inst.currentPlayer.isKeyReverse ?
                Gamemanager.Inst.currentPlayer.AxisY + Time.deltaTime :
                Gamemanager.Inst.currentPlayer.AxisY - Time.deltaTime;

            //Gamemanager.Inst.currentPlayer.AxisY -= Time.deltaTime;
        }
        if (Input.GetKey(KeySetting.keys[KeyAction.Left]))
        {
            Gamemanager.Inst.currentPlayer.AxisX = Gamemanager.Inst.currentPlayer.isKeyReverse ?
                Gamemanager.Inst.currentPlayer.AxisX + Time.deltaTime :
                Gamemanager.Inst.currentPlayer.AxisX - Time.deltaTime;

            //Gamemanager.Inst.currentPlayer.AxisX -= Time.deltaTime;
        }
        if (Input.GetKey(KeySetting.keys[KeyAction.Right]))
        {
            Gamemanager.Inst.currentPlayer.AxisX = Gamemanager.Inst.currentPlayer.isKeyReverse ?
                Gamemanager.Inst.currentPlayer.AxisX - Time.deltaTime :
                Gamemanager.Inst.currentPlayer.AxisX + Time.deltaTime;

            //Gamemanager.Inst.currentPlayer.AxisX += Time.deltaTime;
        }
        if (Input.GetKey(KeySetting.keys[KeyAction.Jump]))
        {

        }
        if (Input.GetKey(KeySetting.keys[KeyAction.Attack]))
        {

        }
        if (Input.GetKey(KeySetting.keys[KeyAction.Selectskill]))
        {

        }
        if (Input.GetKey(KeySetting.keys[KeyAction.Usingskill]))
        {

        }
        if (Input.GetKey(KeySetting.keys[KeyAction.Grab]))
        {

        }
        if (Input.GetKey(KeySetting.keys[KeyAction.Menu]))
        {

        }
        if (Input.GetKey(KeySetting.keys[KeyAction.Run]))
        {

        }

    }

    public void InputKeyUp()
    {

        if (Input.GetKeyUp(KeySetting.keys[KeyAction.Forward]))
        {
            Debug.Log("Up");
            Gamemanager.Inst.currentPlayer.AxisY = 0;
        }
        if (Input.GetKeyUp(KeySetting.keys[KeyAction.Backward]))
        {

            Gamemanager.Inst.currentPlayer.AxisY = 0;
        }
        if (Input.GetKeyUp(KeySetting.keys[KeyAction.Left]))
        {
            Gamemanager.Inst.currentPlayer.AxisX = 0;
        }
        if (Input.GetKeyUp(KeySetting.keys[KeyAction.Right]))
        {
            Gamemanager.Inst.currentPlayer.AxisX = 0;
        }
        if (Input.GetKeyUp(KeySetting.keys[KeyAction.Jump]))
        {

        }
        if (Input.GetKeyUp(KeySetting.keys[KeyAction.Attack]))
        {

        }
        if (Input.GetKeyUp(KeySetting.keys[KeyAction.Selectskill]))
        {

        }
        if (Input.GetKeyUp(KeySetting.keys[KeyAction.Usingskill]))
        {

        }
        if (Input.GetKeyUp(KeySetting.keys[KeyAction.Grab]))
        {
            //Gamemanager.Inst.currentPlayer.isGrab = false;
            Gamemanager.Inst.currentPlayer.DeGrab();

        }
        if (Input.GetKeyUp(KeySetting.keys[KeyAction.Menu]))
        {

        }
        if (Input.GetKeyUp(KeySetting.keys[KeyAction.Run]))
        {
            Gamemanager.Inst.currentPlayer.SetRunning(false);

        }

    }

    public void InputMouseDrag()
    {

    }


}
