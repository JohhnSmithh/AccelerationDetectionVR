using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlignmentUI : MonoBehaviour
{
    /// <summary>
    /// for loading the appropriate version of the trial scene.
    /// Called by UI in AlignmentScene.
    /// </summary>
    public void StartTrial(bool isForward)
    {
        TrialManager.Instance.SetForward(isForward);
        SceneManager.LoadScene("AccelerationDetection"); // load trial scene
    }

    private void Update()
    {
        // TODO: polish by instead making them SPAWN relative to the camera and show an indicator on the ground where the participant should be standing
        // if possible, use ideal center position, not just current camera position

        // keep buttons locked near camera
        transform.position = Camera.main.transform.position;
    }
}
