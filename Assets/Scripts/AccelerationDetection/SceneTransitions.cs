using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles scene transitions from the trial to alignment scene.
/// Also handles resetting of relevant parameters for the next trial
/// </summary>
public class SceneTransitions : MonoBehaviour
{
    [SerializeField, Tooltip("used to reset rig position for next trial")] private GameObject _rigObject;

    // Start is called before the first frame update
    void Start()
    {
        // reset rig object to remove previous change accumulation
        _rigObject.transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(TrialManager.Instance.Data.trialDone)
        {
            // reset trialDone state
            TrialManager.Instance.SetTrialDone(false);

            // flip walking direction
            TrialManager.Instance.SetForward(!TrialManager.Instance.Data.isForward);

            // load alignment scene before starting next trial
            SceneManager.LoadScene("1_Alignment");
        }
    }
}
