using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationGain : MonoBehaviour
{
    [Tooltip("Multiplier for camera rig planar translations; 1.0 = unchanged")] public float GainValue = 2.0f;
    
    private Vector3 _prevPosition;

    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {
        // calculate position change without gain
        Vector3 newPos = Camera.main.gameObject.transform.localPosition;
        newPos = new Vector3(newPos.x, 0, newPos.z);
        Vector3 dif = newPos - _prevPosition; //amount of movement

        // apply gain - subtract 1.0 so that the standard movement (applied by another script) is not re-applied
        this.transform.Translate(dif * (GainValue - 1.0f)) ;

        // save current position for next frame
        _prevPosition = newPos;
    }
}
