using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

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
    private float _reportedTime = -1; // default value -1 indicates not reported
    private float _timeSinceStart = 0f;

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += RestartTrial;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= RestartTrial;
    }

    void RestartTrial(Scene scene, LoadSceneMode mode)
    {
        _timeSinceStart = 0f;
        _reportedTime = -1f;
        _reportedVelocityGain = -1f;
    }

    // Update is called once per frame
    void Update()
    {
        // skip logging within alignment scene
        if (SceneManager.GetActiveScene().name == "1_Alignment")
            return;

        #region MOTION LOGGING

        // TODO: add actual PID here
        string motionLogString = "PID" + "," + TrialManager.Instance.Data.trialNum + ","
            + TrialManager.Instance.Data.currVelocityGain + "," + _timeSinceStart 
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
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) && _reportedVelocityGain == -1) // only store first report per trial
        {
            _reportedVelocityGain = TrialManager.Instance.Data.currVelocityGain;
            _reportedTime = _timeSinceStart;
        }

        // detect when at max distance to end trial
        if(TrialManager.Instance.Data.currRealPos.z > _physicalDistancePerTrial)
        {
            // log data for the current trial
            // TODO: add actual PID here
            string trialLogString = "PID" + "," + TrialManager.Instance.Data.trialNum + 
                "," + TrialManager.Instance.Data.trialAccel + "," + _reportedVelocityGain
                + "," + _reportedTime + "," + (_reportedTime == -1 ? 0 : 1) + "," + _timeSinceStart;
            _trialFile.WriteLine(trialLogString);
            _trialFile.Flush();

            // trial complete, now what?
            if (TrialManager.Instance.DoTrialsRemain())
            {
                TrialManager.Instance.SetTrialDone(true);
            }
            else // all trials compelte
            {
                // close StreamWriter
                _trialFile.Close();
                _motionFile.Close();

                // TODO: replace this with whatever exit screen study participants get
                Debug.Log("ALL TRIALS COMPLETE"); 
            }
        }

        #endregion

        // Handle timer - used in motion and trial logs
        _timeSinceStart += Time.deltaTime;
    }
}
