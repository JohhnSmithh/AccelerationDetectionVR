using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AlignmentUI : MonoBehaviour
{
    [SerializeField, Tooltip("Game object containing all panels for forward orientation")]
    private GameObject _forwardPanels;

    [SerializeField, Tooltip("Game object ocntaining all panels for backwards orientation")]
    private GameObject _backwardPanels;

    private void Start()
    {
        // enable appropriate set of panels for intended orientation
        if (TrialManager.Instance.Data.isForward)
        {
            _forwardPanels.SetActive(true);
            _backwardPanels.SetActive(false);
        }
        else
        {
            _forwardPanels.SetActive(false);
            _backwardPanels.SetActive(true);
        }
    }

    private void Update()
    {
        // TODO: polish by instead making them SPAWN relative to the camera and show an indicator on the ground where the participant should be standing
        // if possible, use ideal center position, not just current camera position

        // keep buttons locked near camera
        transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
    }

    /// <summary>
    /// for loading the appropriate version of the trial scene.
    /// Called by UI in AlignmentScene.
    /// </summary>
    public void StartTrial()
    {
        SceneManager.LoadScene("2_Trial"); // load trial scene
    }
}
