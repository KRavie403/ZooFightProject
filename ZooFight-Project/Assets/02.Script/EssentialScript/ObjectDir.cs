using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectDir : MonoBehaviour
{
    public string myObjName = null;


    public Dictionary<int,GameObject> myObjsId = new Dictionary<int,GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        myObjName = gameObject.name;
    }

    public void AddMyId(int Id,GameObject Obj)
    {
        myObjsId.Add(Id, Obj);
    }
    
    public void RemoveMyId(int Id)
    {
        myObjsId.Remove(Id);
    }

    public void ReNewMyObj() 
    {
        if (GetComponentInChildren<IObjectId>() == null) 
        {
            return;
        }
        myObjsId.Clear();

        foreach (GameObject Obj in GetComponentsInChildren<GameObject>())
        {
            if(Obj.name == myObjName)
            {
                myObjsId.Add(Obj.GetComponent<IObjectId>().ObjectId,Obj);
            }
        }
    }

}
