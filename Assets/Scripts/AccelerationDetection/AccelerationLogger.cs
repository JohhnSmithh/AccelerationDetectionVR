using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AccelerationLogger : MonoBehaviour
{
    // TODO: figure out if/when to track real-world and virtual-world XYZ coordinates - probably with a separate logger

    [SerializeField, Tooltip("Actual distance (in meters) that that participant will travel per trial")] 
    private float _physicalDistancePerTrial = 3f;

    private const string TRIAL_HEADER = "PID,TrialNumber,Acceleration,GainValueReported,TimeWhenReported,Detection,TotalTime";
    private StreamWriter _trialFile;

    private const string MOTION_HEADER = "PID,TrialNumber,CurrentGain,TimeSinceStart,RealX,RealY,RealZ,VirtualX,VirtualY,VirtualZ";
    private StreamWriter _motionFile;

    // locally stored values to be calculated/logged
    private float _reportedVelocityGain = -1; // default value -1 indicates not reported

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
        // trial file
        _trialFile = new StreamWriter(Application.persistentDataPath + "/" + "PID" + "_TrialLog_" + System.DateTime.Now.ToFileTimeUtc() + ".csv", true);
        _trialFile.WriteLine(TRIAL_HEADER);
        _trialFile.Flush();

        // motion file
        _motionFile = new StreamWriter(Application.persistentDataPath + "/" + "PID" + "_MotionLog_" + System.DateTime.Now.ToFileTimeUtc() + ".csv", true);
        _motionFile.WriteLine(MOTION_HEADER);
        _motionFile.Flush();
    }

    // Update is called once per frame
    void Update()
    {
        #region MOTION LOGGING

        // TODO: add actual PID here
        string motionLogString = "PID" + "," + TrialManager.Instance.Data.trialNum + ","
            + TrialManager.Instance.Data.currVelocityGain + "," + "TIME SINCE START" 
            + "," + TrialManager.Instance.Data.currRealPos.x 
            + "," + TrialManager.Instance.Data.currRealPos.y 
            + "," + TrialManager.Instance.Data.currRealPos.z
            + "," + TrialManager.Instance.Data.currVirtualPos.x 
            + "," + TrialManager.Instance.Data.currVirtualPos.y 
            + "," + TrialManager.Instance.Data.currVirtualPos.z;
        _motionFile.WriteLine(motionLogString);
        _motionFile.Flush();

        #endregion

        #region TRIAL LOGGING

        // save current gain value when reported
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            _reportedVelocityGain = TrialManager.Instance.Data.currVelocityGain;
        }

        // detect when at max distance to end trial
        //if(TrialManager.Instance.Data.currRealPos.z > _physicalDistancePerTrial)

        // TODO: stop at END POSITION, not on button press; this is a much larger problem because I cant seem to reset the rig transform (pos/rot)
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) 
        {
            // log data for the current trial
            // TODO: add actual PID here
            // TODO: add times and detection value here
            string trialLogString = "PID" + "," + TrialManager.Instance.Data.trialNum + 
                "," + TrialManager.Instance.Data.trialAccel + "," + _reportedVelocityGain
                + "," + "TimeWhenReported" + "," + "Detection" + "," + "TotalTime";
            _trialFile.WriteLine(trialLogString);
            _trialFile.Flush();

            // trial complete, now what?
            if (TrialManager.Instance.DoTrialsRemain())
            {
                // TODO: add 'Preparation" scene for the user to physically situate for next trial

                // temporary behavior for testing
                TrialManager.Instance.SetTrialDone(true);
            }
            else // all trials compelte
            {
                // close StreamWriter
                _trialFile.Close();
                _motionFile.Close();

                Debug.Log("ALL TRIALS COMPLETE"); // TODO: replace this with whatever exit screen study participants get
            }
        }

        #endregion
    }
}
