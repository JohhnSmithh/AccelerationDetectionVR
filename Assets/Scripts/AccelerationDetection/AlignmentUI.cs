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
        transform.position = Camera.main.transform.position;
    }
}
