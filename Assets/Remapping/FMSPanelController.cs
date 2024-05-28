using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FMSPanelController : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Slider scale;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = scale.value.ToString();
    }
}
