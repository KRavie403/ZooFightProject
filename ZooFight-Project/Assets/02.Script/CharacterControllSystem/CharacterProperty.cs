using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterProperty : MonoBehaviour
{
    public ObjectType myObjType = ObjectType.Character;
    public int myObjectNum = -1;

    public int SessionId = new();
    public int CharacterID = -1;
    public Team myTeam = Team.NotSetting;

    [Range(0f, 10f)]
    public float BaseSpeedRate;
    [SerializeField]
    private float _moveSpeed = 1.0f;
    public float MoveSpeed
    {
        get
        {
            return _moveSpeed * BaseSpeedRate;
        }
    }

    [Range(0f,10.0f)]
    public float RunSpeedRate = 1;

    [Range(0f,10.0f)]
    public float JumpHeight = 2.0f;
    public float MaxHP;


    public UnityEvent<float> UpdateHp;
    private float _curHP = -100.0f;
    public float CurHP
    {
        get
        {
            if (_curHP < 0.0f) _curHP = MaxHP;
            return _curHP;
        }
        set
        {
            _curHP = Mathf.Clamp(value, 0.0f, MaxHP);
            UpdateHp?.Invoke(Mathf.Approximately(MaxHP, 0.0f) ? 0.0f : _curHP);
        }
    }

    public UnityEvent<float> UpdateSp;
    [Range(0.0f,100.0f)]
    public float MaxSP;
    private float _curSP = -100.0f;
    public float CurSP
    {
        get
        {
            if (_curSP < 0.0f) _curSP = MaxSP;
            return _curSP;
        }
        set
        {
            _curSP = Mathf.Clamp(value, 0.0f, MaxSP);
            UpdateSp?.Invoke(Mathf.Approximately(MaxSP, 0.0f) ? 0.0f : _curSP);
        }
    }
    public float SPRecovery = 10.0f;

    public bool isShield = false;
    public UnityEvent<float> UpdateShield;
    public float MaxShield;
    private float _curShield = 0.0f;
    public float CurShield
    {
        get
        {
            if(!isShield) return 0;
            if (_curShield < 0.0f) _curShield = 0;
            return _curShield;
        }
        set
        {
            _curShield = Mathf.Clamp(value, 0.0f, MaxShield);
            UpdateShield?.Invoke(Mathf.Approximately(MaxShield, 0.0f) ? 0.0f : _curShield);
        }
    }


    public float MotionSpeed;
    Animator _anim = null;
    public Animator myAnim
    {
        get
        {
            if (_anim == null)
            {
                _anim = GetComponent<Animator>(); //자기자신의 것부터 찾아봄.
                if (_anim == null) //없으면 자식의 컴포넌트를 가져옴.
                {
                    _anim = GetComponentInChildren<Animator>();
                }
            }
            return _anim;
        }
    }





}
