using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    public Collider us;
    public bool locked = false;

    public GameObject parent;
    public GameObject held;


    public Vector3 distance;
    // Start is called before the first frame update
    void Start()
    {
        us = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && !locked)
        {
            Debug.Log("grabbing");
            Grabbable[] grabs = FindObjectsOfType<Grabbable>();
            Debug.Log(grabs.Length);
            foreach (Grabbable g in grabs)
            {
                if (g.gameObject.GetComponent<Collider>())
                {

                    if (g.gameObject.GetComponent<Collider>().bounds.Intersects(us.bounds))
                    {
                        Debug.Log("intersection");
                        held = g.gameObject;
                        parent = (held.transform.parent == null)?null:held.transform.parent.gameObject;
                        held.transform.SetParent(gameObject.transform);

                        locked = true;
                        return;
                    }
                    else
                    {
                        Debug.Log("no intersection");
                    }

                }


            }
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) & locked)
        {           
            held.transform.SetParent((parent==null)?null:parent.transform);
            locked = false;
            parent = null;
            held = null;
        }
    }
}
