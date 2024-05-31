using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to align UI in 3_ThankYou scene with camera.
/// </summary>
public class ThankYouUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {
        // always align UI relative to camera position
        transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
    }
}
