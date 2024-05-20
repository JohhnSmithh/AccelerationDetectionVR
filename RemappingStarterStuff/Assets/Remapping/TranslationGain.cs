using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationGain : MonoBehaviour
{
    public float GainValue = 2.0f;
    Vector3 oldPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //apply gain
        Vector3 newPos = Camera.main.gameObject.transform.localPosition;
        newPos = new Vector3(newPos.x, 0, newPos.z);

        Vector3 dif = newPos- oldPosition; //amount of movement;

        this.transform.Translate(dif * (GainValue - 1.0f)) ;

        oldPosition = newPos;
    }
}
