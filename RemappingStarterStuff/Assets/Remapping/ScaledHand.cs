using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaledHand : MonoBehaviour
{
    public GameObject c, h;
    public float s;
    public Vector3 offset;

    public Collider us;
    public bool locked = false;

    public GameObject parent;
    public GameObject held;

    public bool leftHand = false;



    // Start is called before the first frame update
    void Start()
    {
        us = GetComponent<Collider>();
        //real_col = gameObject.AddComponent<SphereCollider>();
        // real_col.center = new Vector3(0, -0.03f, 0.01f);
        //real_col.radius = 0.05f;


        /*
        GameObject selector = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        selector.transform.parent = gameObject.transform;
        selector.transform.localPosition = new Vector3(0, -0.05f, .1f);
        selector.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        selector.GetComponent<MeshRenderer>().material = (Material)Resources.Load("Cyan");
        real_col = selector.GetComponent<SphereCollider>();
        */
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 scalePos = c.transform.position - new Vector3(0, 0.3f, -0.15f);
        Vector3 dif = -(scalePos - h.transform.position);
        gameObject.transform.position = offset + (scalePos + dif * s);

        bool Grabbed = leftHand ? OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch) ==1 : OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) ==1;

        if (Grabbed && !locked)
        {
            //Debug.Log("grabbing");
            Grabbable[] grabs = FindObjectsOfType<Grabbable>();
            foreach (Grabbable g in grabs)
            {
                if (g.gameObject.GetComponent<Collider>())
                {

                    if (g.gameObject.GetComponent<Collider>().bounds.Intersects(us.bounds))
                    {
                        //Debug.Log("intersection");
                        held = g.gameObject;
                        parent = (held.transform.parent == null) ? null : held.transform.parent.gameObject;
                        held.transform.SetParent(gameObject.transform);

                        locked = true;
                        return;
                    }
                    else
                    {
                        //Debug.Log("no intersection");
                    }

                }


            }
        }

        bool Released = leftHand ? OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.Touch)!=1 : OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger, OVRInput.Controller.Touch) !=1;

        if (Released & locked)
        {
            held.transform.SetParent((parent == null) ? null : parent.transform);
            locked = false;
            parent = null;
            held = null;
        }


    }

}
