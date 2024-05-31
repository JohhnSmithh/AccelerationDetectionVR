using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles UI alignment with camerea and button functions for UI elements in 0_EnterPID scene.
/// </summary>
public class PIDEntryUI : MonoBehaviour
{
    [SerializeField, Tooltip("used to call the fade to black animation before transitioning scenes")] 
    private FadeHandler _fadeHandler;

    public string PID = "P";
    public TextMeshProUGUI display;

    // Start is called before the first frame update
    void Start()
    { }

    // Update is called once per frame
    void Update()
    {
        // always align UI relative to camera position
        transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
    }

    /// <summary>
    /// adds entered digit to PID, verifying length to ensure no more than 3 numeric digits
    /// </summary>
    public void AddChar(string s)
    {
        // only permit three digit PID
        if (PID.Length < 4)
        {
            PID += s;
            display.text = "PID: " + PID + (PID.Length == 4? "" : "_");
        }
    }

    /// <summary>
    /// Enters the first scene that participants will see.
    /// Called by UI elements in 0_EnterPID scene
    /// </summary>
    public void LoadStudy()
    {
        // only allow loading scene if a 3 digit PID was entered
        if(PID.Length == 4)
        {
            TrialManager.Instance.SetPID(PID);
            _fadeHandler.FadeToLevel("1_Alignment");
        }
    }
}
