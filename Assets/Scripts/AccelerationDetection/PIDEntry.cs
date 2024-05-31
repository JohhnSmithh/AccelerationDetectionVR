using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PIDEntry : MonoBehaviour
{

    public string PID = "P";
    public TextMeshProUGUI display;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddChar(string s)
    {
        
        PID += s;
        //Guidance.PID = PID;
        display.text = "PID: " + PID+"_";
    }

}
