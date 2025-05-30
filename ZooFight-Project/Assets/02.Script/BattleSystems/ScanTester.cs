using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScanTester : MonoBehaviour,IHitScanner
{
    Component IHitScanner.myComp => this as Component;

    Component[] IHitScanner.myTargets => targets;

    Component[] targets = null;

    // Start is called before the first frame update
    void Start()
    {
        duringTime = 0.0f;
    }

    public float ScanRange = 2.0f;

    public float duringTime = 0.0f;

    // Update is called once per frame
    void Update()
    {

        duringTime += Time.deltaTime;
        if(duringTime >= 5.0f)
        {
            targets = HitScan.Inst.HitScans(this.gameObject, ScanRange, ScanTarget.Player, ScanType.Sphere);

            if (targets != null)
            {
                Debug.Log("ScannerScanned");
                foreach (var comp in targets)
                {
                    Debug.Log(comp.GetComponent<IHitScanTarget>().testcode);

                    comp.GetComponent<IHitScanTarget>().Hit(this as Component);
                }
                
            }
            duringTime = 0.0f;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }

    void IHitScanner.AddTarget(Component[] target)
    {
        targets = target;
    }

    void IHitScanner.Hit()
    {
        Debug.Log("Hit");
        transform.Translate(Vector3.forward);
    }

}
