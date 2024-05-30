using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

            // TODO: make resetting camera transform actually work with OVRCameraRig
            // reset position back to origin
            //_rigObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            // load alignment scene before starting next trial
            SceneManager.LoadScene("AlignmentScene");
        }
    }
}
