using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class TypeTest : MonoBehaviour ,IHitScanTarget
{
    Component IHitScanTarget.myComp => this as Component;

    int IHitScanTarget.testcode => test1;

    public int test1 = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    
    }

    void IHitScanTarget.Hit(Component comp)
    {
        Debug.Log("Hit");
        transform.Translate(Vector3.forward);
    }

}
