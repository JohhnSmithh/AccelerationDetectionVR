using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Responsible for showing the appropriate alignment panels and orienting panels around the ideal physical position.
/// Also includes UI button functions for the 1_Alignment scene
/// </summary>
public class AlignmentUI : MonoBehaviour
{
    [SerializeField, Tooltip("Game object containing all panels for forward orientation")]
    private GameObject _forwardPanels;

    [SerializeField, Tooltip("Game object ocntaining all panels for backwards orientation")]
    private GameObject _backwardPanels;

    [SerializeField, Tooltip("used to call the fade to black animation before transitioning scenes")] 
    private FadeHandler _fadeHandler;

    [Header("Alignment Indicator")]
    [SerializeField, Tooltip("Z-position of UI in forward configuration")] private float _forwardPos = 0f;
    [SerializeField, Tooltip("Z-position of UI in backwards configuration")] private float _backwardPos = 2f;

    private void Start()
    {
        // enable appropriate set of panels for intended orientation
        if (TrialManager.Instance.Data.isForward)
        {
            _forwardPanels.SetActive(true);
            _backwardPanels.SetActive(false);
            transform.position = new Vector3(transform.position.x, transform.position.y, _forwardPos);
        }
        else
        {
            _forwardPanels.SetActive(false);
            _backwardPanels.SetActive(true);
            transform.position = new Vector3(transform.position.x, transform.position.y, _backwardPos);
        }
    }

    private void Update()
    {}

    /// <summary>
    /// for loading the appropriate version of the trial scene.
    /// Called by UI in AlignmentScene.
    /// </summary>
    public void StartTrial()
    {
        _fadeHandler.FadeToLevel("2_Trial");
    }
}
