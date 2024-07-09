using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Responsible for showing the appropriate alignment panels and orienting panels around the ideal physical position.
/// Also includes UI button functions for the 1_Alignment scene
/// </summary>
public class AlignmentUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField, Tooltip("Game object containing all panels for forward orientation")]
    private GameObject _forwardPanels;
    [SerializeField, Tooltip("text component on forward panel")] 
    private TextMeshProUGUI _forwardText;
    [SerializeField, Tooltip("button component on forward panel")]
    private GameObject _forwardButton;
    [SerializeField, Tooltip("Alignment prompt UI components on forward panel")]
    private GameObject _forwardPrompt;

    [SerializeField, Tooltip("Game object ocntaining all panels for backwards orientation")]
    private GameObject _backwardPanels;
    [SerializeField, Tooltip("text component on forward panel")]
    private TextMeshProUGUI _backwardText;
    [SerializeField, Tooltip("button component on backward panel")]
    private GameObject _backwardButton;
    [SerializeField, Tooltip("Alignment prompt UI components on backward panel")]
    private GameObject _backwardPrompt;

    [Header("Scene Transitions")]
    [SerializeField, Tooltip("used to call the fade to black animation before transitioning scenes")] 
    private FadeHandler _fadeHandler;

    [Header("Alignment Verification")]
    [SerializeField, Tooltip("Object for determining user alignmet")] 
    private Transform _alignmentIndicator;
    [SerializeField, Tooltip("Max valid distance from alignment indicator (in meters)")]
    private float _alignmentDistance = 0.5f;
    

    [Header("Repositioning")]
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

            // set text
            if (!TrialManager.Instance.Data.training1Done)
                _forwardText.SetText("Start Normal Trial");
            else if (!TrialManager.Instance.Data.training2Done)
                _forwardText.SetText("Start Acceleration Trial");
            else
                _forwardText.SetText("Start Trial");
        }
        else
        {
            _forwardPanels.SetActive(false);
            _backwardPanels.SetActive(true);
            transform.position = new Vector3(transform.position.x, transform.position.y, _backwardPos);

            // set text
            if (!TrialManager.Instance.Data.training1Done)
                _backwardText.SetText("Start Normal Trial");
            else if (!TrialManager.Instance.Data.training2Done)
                _backwardText.SetText("Start Acceleration Trial");
            else
                _backwardText.SetText("Start Trial");
        }
    }

    private void Update()
    {
        // check if close enough to indicator to continue
        Vector2 rigPos = new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.z);
        Vector2 alignmentPos = new Vector2(_alignmentIndicator.position.x, _alignmentIndicator.position.z);
        if(Vector2.Distance(rigPos, alignmentPos) <= _alignmentDistance)
        {
            // enable buttons
            _forwardButton.SetActive(true);
            _backwardButton.SetActive(true);

            // disable prompts
            _forwardPrompt.SetActive(false);
            _backwardPrompt.SetActive(false);
        }
        else // too far from alignment indicator
        {
            // enable buttons
            _forwardButton.SetActive(false);
            _backwardButton.SetActive(false);

            // disable prompts
            _forwardPrompt.SetActive(true);
            _backwardPrompt.SetActive(true);
        }
    }

    /// <summary>
    /// for loading the appropriate version of the trial scene.
    /// Called by UI in AlignmentScene.
    /// </summary>
    public void StartTrial()
    {
        _fadeHandler.FadeToLevel("2_Trial");
    }
}
