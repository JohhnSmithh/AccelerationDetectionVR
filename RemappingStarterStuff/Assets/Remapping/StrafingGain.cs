using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//You et. al's Strafing Gain (alternate expression)
//Rather than being expressed as an angle of redirection
public class StrafingGain : MonoBehaviour
{
    [Tooltip("Two values can be used to control the technique. If true, then GainAngle is used. If false, then GainRatio is used.")]
    public bool AngleGain = true;

    [Tooltip("The percent of translational movement that is turned into perpendicular strafing movement. Positive values = rightward virtual head translation relative to movement = leftward strafing movements when walking.")]
    public float GainRatio = 0.0f;//e.g., if GainRatio = 1, and the user walks forward X meters, the user will have GainRatio*X meters of additional rightward movement added.
    
    /*
     * Diagram
     * 
     |
     |
     |X units of real walking
     |
     |    
     V---------> GainRatio*X horizontal translation applied to the head
    */
    


    [Tooltip("The angle of strafing redirection. Positive values = Positive values = rightward virtual head translation relative to movement = leftward strafing movements when walking.")]
    public float GainAngle = 0.0f;
    /*
      |\
      |A\
      |  \
      |   \
      |    \    
      V---->V User deflected on to a diagonal path of angle A

     */


    Vector3 oldPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Calc the other equivalent gain value
        if (!AngleGain)
            GainAngle = Mathf.Rad2Deg * Mathf.Atan(GainRatio);
        else
            GainRatio = Mathf.Tan(GainAngle * Mathf.Deg2Rad);


        //apply gain
        Vector3 newPos = Camera.main.gameObject.transform.localPosition;
        newPos = new Vector3(newPos.x, 0, newPos.z);

        Vector3 dif = newPos - oldPosition; //amount of movement;

        dif = Quaternion.Euler(0, 90, 0) * dif;


        this.transform.Translate(dif* GainRatio);

        oldPosition = newPos;
    }
}
