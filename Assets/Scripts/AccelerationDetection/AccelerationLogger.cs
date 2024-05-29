using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class AccelerationLogger : MonoBehaviour
{
    // TODO: figure out if/when to track real-world and virtual-world XYZ coordinates - probably with a separate logger

    private const string HEADER = "PID,TrialNumber,Acceleration,GainValueReported";
    private StreamWriter _trialFile;

    // called before start
    private void Awake()
    {
        // only allow one instance of the data logger in the scene at once
        if (GameObject.FindGameObjectsWithTag("Logger").Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // make data logger persistent
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        // create trial file data path and header
        _trialFile = new StreamWriter(Application.persistentDataPath + "/" + "PID" + "_TrialLog_" + System.DateTime.Now.ToFileTimeUtc() + ".csv", true);
        _trialFile.WriteLine(HEADER);
        _trialFile.Flush();
    }

    // Update is called once per frame
    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            // log data for the current trial
            string logString = "PID" + "," + TrialManager.Instance.Data.trialNum + 
                "," + TrialManager.Instance.Data.currAccel + "," + TrialManager.Instance.Data.currVelocityGain;
            _trialFile.WriteLine(logString);
            _trialFile.Flush();

            // TODO: add another scene or otherwise some way for the user to physically situate back to the start location in the physical trial room
            // reload scene
            if (TrialManager.Instance.DoTrialsRemain())
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            else
            {
                // close StreamWriter
                _trialFile.Close();

                Debug.Log("ALL TRIALS COMPLETE"); // TODO: replace this with whatever exit screen study participants get
            }
        }

        // TODO: add alternate condition for reaching the end of an individual trial
    }
}
