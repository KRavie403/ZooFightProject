using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PoolObjectData
{
    public GameObject prefab;

    [Tooltip("poolCount는 최소 1개 이상 설정")]
    public int poolCount;
    [Header("해당 오브젝트 설명")]
    public string explan;
}

/// <summary> Object Pooling Manager </summary>
public class ObjectPoolingManager : MonoBehaviour
{
    public static ObjectPoolingManager instance;

    [SerializeField] private bool dontDestroy = true;
    [SerializeField] private List<PoolObjectData> playerPreset = new List<PoolObjectData>();
    [SerializeField] private List<PoolObjectData> ItemPools = new List<PoolObjectData>();
    [SerializeField] private List<PoolObjectData> ObjectPoolList = new List<PoolObjectData>();
    [SerializeField] private List<PoolObjectData> npcList= new List<PoolObjectData>();

    [SerializeField] private List<PoolObjectData> EffectPools = new List<PoolObjectData>();


    private Dictionary<string, Transform> poolParentDic = new Dictionary<string, Transform>();                  // 각각 풀링들의 부모
    private Dictionary<string, PoolObjectData> instantiateObject = new Dictionary<string, PoolObjectData>();    // 풀링으로 생성된 오브젝트

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            if (dontDestroy == true)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else
        {
            Destroy(this.gameObject);   // 오브젝트 풀링 중복 방지
        }

        MakeDir("Using");
        Pool(playerPreset);   // 초기 풀링에 들어가있는 오브젝트 풀링
        Pool(ItemPools);   // 초기 풀링에 들어가있는 오브젝트 풀링
        Pool(ObjectPoolList);   // 초기 풀링에 들어가있는 오브젝트 풀링
        Pool(npcList);
    }

    #region Set
    public void EffectPoolSet(List<PoolObjectData> pool)
    {
        EffectPools = pool;
    }
    #endregion


    #region Instantiate
    public void Pool(List<PoolObjectData> _poolObjectList)
    {
        GameObject prefab;

        string prefabName;
        int count;

        PoolObjectData data;
        for (int i = 0; i < _poolObjectList.Count; i++)
        {
            data = _poolObjectList[i];

            prefab = data.prefab;
            prefabName = prefab.name;
            count = data.poolCount;

            if (prefab == null)
            {
                Debug.LogError($" <color=red> ObjectPooler Instantiate Error</color> : <color=yellow>Index</color> <color=orange>{i}</color> <color=yellow>Is prefab missing</color>");
                continue;
            }

            if (poolParentDic.ContainsKey(prefabName) == false)
            {
                MakeDir(prefabName);
                InstantiatePool(prefab, poolParentDic[prefabName], count);

                instantiateObject[prefabName] = data;
            }
            else
            {
                continue;
            }
        }
    }

    /// <summary> 오브젝트를 반환하거나 생성할 부모를 생성 </summary>
    void MakeDir(string _name)
    {
        GameObject newDir = new GameObject(_name);
        newDir.AddComponent<ObjectDir>();
        newDir.transform.SetParent(this.transform);
        poolParentDic[_name] = newDir.transform;
    }

    /// <summary> 오브젝트를 갯수 만큼 생성하고 마지막 생성된 오브젝트를 반환합니다 </summary>
    GameObject InstantiatePool(GameObject _prefab, Transform _parent, int _count)
    {
        if (_count <= 0)
        {
            Debug.LogError($" <color=red> ObjectPooler Instantiate Error </color> <color=Orange>{_prefab.name}</color> <color=yellow> is pooling count 0</color>");
            return null;
        }

        GameObject inst = null;
        for (int i = 0; i < _count; i++)
        {
            inst = Instantiate(_prefab, Vector3.zero, Quaternion.identity, _parent);
            inst.name = _prefab.name;
            inst.SetActive(false);
        }
        _parent.GetComponent<ObjectDir>().ReNewMyObj();
        return inst;
    }

    #endregion

    #region  Get

    /// <summary> 오브젝트 불러오기 </summary>
    public GameObject GetObject(string _id, Transform _parent = null, bool _enable = true)
    {
        if (poolParentDic.ContainsKey(_id) == false)
        {
            Debug.LogErrorFormat(" <color=red> ObjectPooler Get Error </color> : <color=yellow>not found object name </color> <color=orange> {0} </color>", _id);
            return null;
        }

        if (poolParentDic[_id].childCount > 0)
        {
            Transform pool = poolParentDic[_id].GetChild(0);
            if (pool.gameObject.activeSelf == false)
            {
                if (_parent == null)
                {
                    pool.SetParent(poolParentDic["Using"]);
                }
                else
                {
                    pool.SetParent(_parent);
                }

                pool.gameObject.SetActive(_enable);
                return pool.gameObject;
            }
        }

        // 부족한 경우 추가 생성 후 하나를 반환해 줍니다.
        return InstantiatePool(instantiateObject[_id].prefab, poolParentDic[_id], instantiateObject[_id].poolCount);
    }

    /// <summary> 오브젝트의 포지션과 로테이션을 설정하고 반환합니다. </summary>
    public GameObject GetObject(string _id, Vector3 _position, Quaternion _rotation, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _parent, false);
        rtnObject.transform.SetPositionAndRotation(_position, _rotation);
        rtnObject.SetActive(_enable);
        return rtnObject;
    }

    /// <summary> 오브젝트의 포지션과 로테이션을 설정하고 특정 타입의 컨퍼넌트를 반환합니다 </summary>
    public T GetObject<T>(string _id, Vector3 _position, Quaternion _rotation, Transform _parent = null, bool _enable = true)
    {
        return GetObject(_id, _position, _rotation, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트의 포지션과 로테이션을 설정하고 _rtnTime뒤 자동 리턴됩니다. </summary>
    public GameObject GetObject(string _id, Vector3 _position, Quaternion _rotation, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _position, _rotation, _parent, _enable);
        StartCoroutine(WaitReturnObject(rtnObject, _rtnTime));
        return rtnObject;
    }

    /// <summary> 오브젝트의 포지션과 로테이션을 설정하고 _rtnTime뒤 자동 반환 및 컨퍼넌트 반환 </summary>
    public T GetObject<T>(string _id, Vector3 _position, Quaternion _rotation, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        return GetObject(_id, _position, _rotation, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정하고 반환합니다. </summary>
    public GameObject GetObject(string _id, Vector3 _position, Quaternion _rotation, Vector3 _scale, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _parent, false);
        rtnObject.transform.SetPositionAndRotation(_position, _rotation);
        rtnObject.transform.localScale = _scale;
        rtnObject.SetActive(_enable);
        return rtnObject;
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 컨퍼넌트 반환 </summary>
    public T GetObject<T>(string _id, Vector3 _position, Quaternion _rotation, Vector3 _scale, Transform _parent = null, bool _enable = true)
    {
        return GetObject(_id, _position, _rotation, _scale, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 일정시간 뒤 자동 리턴 </summary>
    public GameObject GetObject(string _id, Vector3 _position, Quaternion _rotation, Vector3 _scale, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _position, _rotation, _scale, _parent, _enable);
        StartCoroutine(WaitReturnObject(rtnObject, _rtnTime));
        return rtnObject;
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 일정시간 뒤 자동 리턴 및 특정 컨퍼너는 반환 </summary>
    public T GetObject<T>(string _id, Vector3 _position, Quaternion _rotation, Vector3 _scale, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        return GetObject(_id, _position, _rotation, _scale, _rtnTime, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 오브젝트가 비활성화 될 경우 자동 리턴 </summary>
    public GameObject GetObjectToAutoReturn(string _id, Vector3 _position, Quaternion _rotation, Vector3 _scale, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _position, _rotation, _scale, _parent, _enable);
        StartCoroutine(AutoReturn(rtnObject));
        return rtnObject;
    }

    /// <summary> 오브젝트의 포지션과 로테이션 및 스케일 설정 및 오브젝트가 비활성화 될 경우 자동 리턴 실행 및 컨퍼넌트 반환 </summary>
    public T GetObjectToAutoReturn<T>(string _id, Vector3 _position, Quaternion _rotation, Vector3 _scale, Transform _parent = null, bool _enable = true)
    {
        return GetObjectToAutoReturn(_id, _position, _rotation, _scale, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 일정 시간뒤 오브젝트 반환 </summary>
    public GameObject GetObject(string _id, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _parent, _enable);
        StartCoroutine(WaitReturnObject(rtnObject, _rtnTime));
        return rtnObject;
    }

    /// <summary> 컴퍼넌트 반환 </summary>
    public T GetObject<T>(string _id, float _rtnTime, Transform _parent = null, bool _enable = true)
    {
        return GetObject(_id, _rtnTime, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 오브젝트 포지션과, 로테이션 및 부모를 설정 및 자동 리턴 실행 </summary>
    public GameObject GetObjectToAutoReturn(string _id, Vector3 _position, Quaternion _rotation, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _position, _rotation, _parent, _enable);
        StartCoroutine(AutoReturn(rtnObject));
        return rtnObject;
    }

    /// <summary> 오브젝트 포지션과, 로테이션 및 부모를 설정 및 자동 리턴 실행 및 특정 컨퍼넌트 반환 </summary>

    public T GetObjectToAutoReturn<T>(string _id, Vector3 _position, Quaternion _rotation, Transform _parent = null, bool _enable = true)
    {
        return GetObjectToAutoReturn(_id, _position, _rotation, _parent, _enable).GetComponent<T>();
    }

    /// <summary> 부모 설정 및 오브젝트 비활성화시 자동 리턴 </summary>
    public GameObject GetObjectToAutoReturn(string _id, Transform _parent = null, bool _enable = true)
    {
        GameObject rtnObject = GetObject(_id, _parent, _enable);
        StartCoroutine(AutoReturn(rtnObject));
        return rtnObject;
    }

    /// <summary> 특정 컨퍼넌트 반환 </summary>
    public T GetObjectToAutoReturn<T>(string _id, Transform _parent = null, bool _enable = true)
    {
        return GetObjectToAutoReturn(_id, _parent, _enable).GetComponent<T>();
    }

    #endregion

    #region Return

    /// <summary> 오브젝트 리턴 </summary>
    public void ReturnObject(GameObject _object)
    {
        if (_object == null)
        {
            Debug.LogErrorFormat(" <color=red> ObjectPooler Return Error </color> : <color=yellow> return object is null</color>");
            return;
        }

        if (poolParentDic.ContainsKey(_object.name) == false)
        {
            Debug.LogWarningFormat(" <color=red> ObjectPooler Return Error </color> : <color=yellow> not found object name is</color> <color=orange> {0} </color>", _object.name);
            return;
        }

        _object.transform.SetParent(poolParentDic[_object.name]);
        _object.SetActive(false);
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
}