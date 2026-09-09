using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// 사용법 개편예정
// EffectPool 은 ObjectPooling 에서 특정 방식으로 오브젝트를
// 인출해 오는 방식을 저장하는 클래스로 변경


public class EffectPool : MonoBehaviour
{
    public List<EffectPlayer> EffectClones;     // 복제된 Effect 저장

    #region 신규코드

    #region Create
    // 사전세팅 작업 추후 코드로 변경예정
    [SerializeField] private List<PoolObjectData> EffectPools = new List<PoolObjectData>();

    private void Start()
    {
        //
        ObjectPoolingManager.instance.EffectPoolSet(EffectPools);
        ObjectPoolingManager.instance.Pool(EffectPools);
    }

    #endregion 


    #region  Get

    /// <summary> 오브젝트 불러오기 </summary>
    public GameObject GetEffectObject(EffectCode code, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObject(code.ToString(), _parent, _enable);
        // 부족한 경우 추가 생성 후 하나를 반환해 줍니다.
    }

    /// <summary>
    /// 오브젝트를 불러오고 컴퍼넌트 반환합니다.
    /// </summary>
    public T GetEffectObject<T>(EffectCode code, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObject(code.ToString(), _parent, _enable).GetComponent<T>();
        // 부족한 경우 추가 생성 후 하나를 반환해 줍니다.
    }

    /// <summary> 오브젝트의 포지션과 로테이션을 설정하고 반환합니다. </summary>
    public GameObject GetEffectObject(EffectCode code, Vector3 _position, Quaternion _rotation, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObject(code.ToString(), _position, _rotation, _parent, _enable);
    }

    /// <summary> 오브젝트의 포지션과 로테이션을 설정하고 특정 타입의 컨퍼넌트를 반환합니다 </summary>
    public T GetEffectObject<T>(EffectCode code, Vector3 _position, Quaternion _rotation, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObject(code.ToString(), _position, _rotation, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트의 포지션과 로테이션을 설정하고 _rtnTime뒤 자동 리턴됩니다. </summary>
    public GameObject GetEffectObject(EffectCode code, Vector3 _position, Quaternion _rotation, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObject(code.ToString(), _position, _rotation, _rtnTime, _parent, _enable);
    }

    /// <summary> 오브젝트의 포지션과 로테이션을 설정하고 _rtnTime뒤 자동 반환 및 컨퍼넌트 반환 </summary>
    public T GetEffectObject<T>(EffectCode code, Vector3 _position, Quaternion _rotation, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObject(code.ToString(), _position, _rotation, _rtnTime, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정하고 반환합니다. </summary>
    public GameObject GetEffectObject(EffectCode code, Vector3 _position, Quaternion _rotation, Vector3 _scale, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObject(code.ToString(), _position, _rotation, _scale, _parent, _enable);
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 컨퍼넌트 반환 </summary>
    public T GetEffectObject<T>(EffectCode code, Vector3 _position, Quaternion _rotation, Vector3 _scale, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObject(code.ToString(), _position, _rotation, _scale, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 일정시간 뒤 자동 리턴 </summary>
    public GameObject GetEffectObject(EffectCode code, Vector3 _position, Quaternion _rotation, Vector3 _scale, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObject(code.ToString(), _position, _rotation, _scale, _rtnTime, _parent, _enable);
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 일정시간 뒤 자동 리턴 및 특정 컨퍼너는 반환 </summary>
    public T GetEffectObject<T>(EffectCode code, Vector3 _position, Quaternion _rotation, Vector3 _scale, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObject(code.ToString(), _position, _rotation, _scale, _rtnTime, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 오브젝트가 비활성화 될 경우 자동 리턴 </summary>
    public GameObject GetEffectObjectToAutoReturn(EffectCode code, Vector3 _position, Quaternion _rotation, Vector3 _scale, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObjectToAutoReturn(code.ToString(), _position, _rotation, _scale, _parent, _enable);
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 오브젝트가 비활성화 될 경우 자동 리턴 실행 및 컨퍼넌트 반환 </summary>
    public T GetEffectObjectToAutoReturn<T>(EffectCode code, Vector3 _position, Quaternion _rotation, Vector3 _scale, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObjectToAutoReturn(code.ToString(), _position, _rotation, _scale, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 일정 시간뒤 오브젝트 반환 </summary>
    public GameObject GetEffectObject(EffectCode code, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObject(code.ToString(), _rtnTime, _parent, _enable);
    }

    /// <summary> 컴퍼넌트 반환 </summary>
    public T GetEffectObject<T>(EffectCode code, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObject(code.ToString(), _rtnTime, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트 포지션과, 로테이션 및 부모를 설정 및 자동 리턴 실행 </summary>
    public GameObject GetEffectObjectToAutoReturn(EffectCode code, Vector3 _position, Quaternion _rotation, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObjectToAutoReturn(code.ToString(), _position, _rotation, _parent, _enable);
    }

    /// <summary> 오브젝트 포지션과, 로테이션 및 부모를 설정 및 자동 리턴 실행 및 특정 컨퍼넌트 반환 </summary>

    public T GetEffectObjectToAutoReturn<T>(EffectCode code, Vector3 _position, Quaternion _rotation, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObjectToAutoReturn(code.ToString(), _position, _rotation, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 부모 설정 및 오브젝트 비활성화시 자동 리턴 </summary>
    public GameObject GetEffectObjectToAutoReturn(EffectCode code, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObjectToAutoReturn(code.ToString(), _parent, _enable);
    }

    /// <summary> 특정 컨퍼넌트 반환 </summary>
    public T GetEffectObjectToAutoReturn<T>(EffectCode code, Transform _parent = null, bool _enable = true)
    {
        return ObjectPoolingManager.instance.GetObjectToAutoReturn(code.ToString(), _parent, _enable).GetComponent<T>();
    }

    #endregion

    #region Return

    /// <summary> 오브젝트 리턴 </summary>
    public void ReturnObject(GameObject _object)
    {
        ObjectPoolingManager.instance.ReturnObject(_object);
    }

    private IEnumerator WaitReturnObject(GameObject _object, float _waitTIme)
    {
        yield return new WaitForSeconds(_waitTIme);
        ReturnObject(_object);
    }

    private IEnumerator AutoReturn(GameObject _object)
    {
        while (_object.activeSelf == true)
        {
            yield return null;
        }
        ReturnObject(_object);
    }
    #endregion

    #endregion


    //신규코드 안정화시 삭제예정
    #region 기존코드 

    // 주어진 'EffectCode'에 해당하는 'EffectPlayer'을 찾아 오브젝트를 반환
    public GameObject GetClones(EffectCode effectCode)
    {
        EffectPlayer players = EffectSetting.keys[effectCode];

        return EffectClones[EffectClones.IndexOf(players)].gameObject;
    }

    // 주어진 'EffectCode'에 해당하는 'EffectPlayer' 객체를 찾아 복제하고 비활성화 상태로 'EffectClones' 리스트에 추가
    public void CreateClones(GameObject gameObject)
    {
        GameObject clone = Instantiate(gameObject);
        EffectClones.Add(clone.GetComponent<EffectPlayer>());
    }
    public void CreateClones(EffectCode effectCode)
    {
        if (EffectSetting.keys.TryGetValue(effectCode, out EffectPlayer player))
        {
            GameObject clone = Instantiate(player.gameObject);
            clone.SetActive(false);  // 복제된 오브젝트를 비활성화
            EffectClones.Add(clone.GetComponent<EffectPlayer>());  // 풀에 추가
        }
        else
        {
            Debug.LogError($"EffectCode: {effectCode}  찾을 수 없음");
        }
    }

    // 주어진 'EffectCode'에 해당하는 효과를 시작 시간과 함께 재생
    public void CloneEffectPlay(EffectCode effectCode,float StartTime)
    {
        EffectPlayer player = EffectSetting.keys[effectCode];
        if (player != null)
        {
            player.EffectPlay(0, StartTime, player.transform, true);
        }
    }

    // 주어진 게임 오브젝트를 비활성화하고 초기 위치로 되돌린 후, 'EffectClones' 리스트에 추가
    public void ReturnToPool(GameObject effectObject)
    {
        effectObject.SetActive(false);
        effectObject.transform.position = Vector3.zero;  // 위치 초기화
        EffectClones.Add(effectObject.GetComponent<EffectPlayer>());
    }

    // 주어진 'EffectCode'에 해당하는 비활성화된 'EffectPlayer' 객체를 풀에서 찾아 활성화하고 반환
    // 없으면 'null'을 반환
    public GameObject GetFromPool(EffectCode effectCode)
    {
        foreach (var player in EffectClones)
        {
            if (((IEffect)player).EffectCode == effectCode && !player.gameObject.activeInHierarchy)
            {
                player.gameObject.SetActive(true);
                return player.gameObject;
            }
        }
        return null;
    }
    #endregion 


}
