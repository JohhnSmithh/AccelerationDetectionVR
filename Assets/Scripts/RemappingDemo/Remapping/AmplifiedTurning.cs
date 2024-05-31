using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmplifiedTurning : MonoBehaviour
{
    public float GainValue = 2.0f;
    Vector3 oldRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //apply gain
        Vector3 f = Camera.main.gameObject.transform.forward;
        f = new Vector3(f.x, 0, f.z);
        float angle = (GainValue-1.0f)*Vector3.SignedAngle(oldRotation, f, Vector3.up);
        this.transform.RotateAround(Camera.main.gameObject.transform.position, Vector3.up, angle);

        //save new rotation as old value
        f = Camera.main.gameObject.transform.forward;
        f = new Vector3(f.x, 0, f.z);
        oldRotation = f;
    }
}
