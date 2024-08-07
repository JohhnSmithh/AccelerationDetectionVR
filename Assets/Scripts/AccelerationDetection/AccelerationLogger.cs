using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

/// <summary>
/// Creates trial log files with reported detection data.
/// Creates motion log files with gain, time, and position data (real and virtual).
/// Also handles ending an individual trial when maximum physical travel distance is detected.
/// </summary>
public class AccelerationLogger : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField, Tooltip("Input threshold on trigger that counts as button press")] private float _triggerThreshold = 0.35f;

    [Header("Distance")]
    [SerializeField, Tooltip("Actual distance (in meters) that that participant will travel per trial")] 
    private float _physicalDistancePerTrial = 3f;
    [SerializeField, Tooltip("Distance the user must travel before acceleration begins (in meters); MUST MATCH VALUE IN VelocityGain.cs")]
    private float _distanceDelay = 0.5f;

    [Header("Audio")]
    [SerializeField, Tooltip("object to play the sfx")]
    private AudioSource _audioSource;
    [SerializeField, Tooltip("sound effect to play when reporting acceleration")]
    private AudioClip _reportSound;
    [SerializeField, Tooltip("loudness of sfx")]
    private float _volume = 0.6f;

    private const string TRIAL_HEADER = "PID,TrainingTrial,TrialNumber,Acceleration,GainValueReported,TimeWhenReported,Detection,TotalTime,Forward,MaxGainValue,TimeWhenStartedWalking";
    private StreamWriter _trialFile;

    private const string MOTION_HEADER = "PID,TrainingTrial,TrialNumber,CurrentGain,TimeSinceStart,RealX,VirtualX,Y,RealZ,VirtualZ,HeadForwardX,HeadForwardY,HeadForwardZ";
    private StreamWriter _motionFile;

    // locally stored values to be calculated/logged
    private float _reportedVelocityGain = -1; // default value -1 indicates not reported
    private float _reportedTime = -1; // default value -1 indicates not reported
    private float _startedWalkingTime = -1f; // default to know if start walk time has been recorded yet or not
    private float _timeSinceStart = 0f;
    private bool _trialDone = false; // used to log end of trial so scene fade does not interfere with trial setup

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
        _trialFile = new StreamWriter(Application.persistentDataPath + "/" + TrialManager.Instance.Data.pid 
            + "_TrialLog_" + System.DateTime.Now.ToFileTimeUtc() + ".csv", true);
        _trialFile.WriteLine(TRIAL_HEADER);
        _trialFile.Flush();

        // motion file
        _motionFile = new StreamWriter(Application.persistentDataPath + "/" + TrialManager.Instance.Data.pid 
            + "_MotionLog_" + System.DateTime.Now.ToFileTimeUtc() + ".csv", true);
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

    // resets relevant trial stats at start of new trial (handled through coroutine)
    void RestartTrial(Scene scene, LoadSceneMode mode)
    {
        _timeSinceStart = 0f;
        _reportedTime = -1f;
        _startedWalkingTime = -1f;
        _reportedVelocityGain = -1f;
        _trialDone = false;
    }

    // Update is called once per frame
    void Update()
    {
        // only log data within the trial scene AND not fading out
        if (SceneManager.GetActiveScene().name != "2_Trial" || _trialDone)
            return;

        #region MOTION LOGGING

        // don't motion log experimenter alignment trial
        if(TrialManager.Instance.Data.testAlignDone)
        {
            string motionLogString = TrialManager.Instance.Data.pid
            + "," + (TrialManager.Instance.Data.training2Done ? 0 : 1)
            + "," + TrialManager.Instance.Data.trialNum
            + "," + TrialManager.Instance.Data.currVelocityGain
            + "," + _timeSinceStart
            + "," + TrialManager.Instance.Data.currRealPos.x
            + "," + TrialManager.Instance.Data.currVirtualPos.x
            + "," + TrialManager.Instance.Data.currRealPos.y // y is same for real/virtual (no gain applied)
            + "," + TrialManager.Instance.Data.currRealPos.z
            + "," + TrialManager.Instance.Data.currVirtualPos.z
            + "," + Camera.main.transform.forward.x
            + "," + Camera.main.transform.forward.y
            + "," + Camera.main.transform.forward.z;
            _motionFile.WriteLine(motionLogString);
            _motionFile.Flush();
        }


        #endregion

        #region TRIAL LOGGING

        if (TrialManager.Instance.Data.currRealPos.z > _distanceDelay && _startedWalkingTime==-1f)
        {
            _startedWalkingTime = _timeSinceStart;
        }

        // save current gain value when reported
        if ((OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > _triggerThreshold || OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) > _triggerThreshold) && _reportedVelocityGain == -1) // only store first report per trial
        {
            _reportedVelocityGain = TrialManager.Instance.Data.currVelocityGain;
            _reportedTime = _timeSinceStart;

            // play audio
            _audioSource.PlayOneShot(_reportSound, _volume);
            // vibration
            VibrateEffect();
        }

        // detect when at max distance to end trial
        if(TrialManager.Instance.Data.currRealPos.z > _physicalDistancePerTrial)
        {
            // no logging for experimenter alignment trial
            if (!TrialManager.Instance.Data.testAlignDone)
            {
                TrialManager.Instance.testAlignDone();
            }
            else // standard/training trial (enable logging)
            {
                // log data for the current trial
                string trialLogString = TrialManager.Instance.Data.pid
                    + "," + (TrialManager.Instance.Data.training2Done ? 0 : 1)
                    + "," + TrialManager.Instance.Data.trialNum
                    + "," + TrialManager.Instance.Data.trialAccel
                    + "," + _reportedVelocityGain
                    + "," + _reportedTime
                    + "," + (_reportedTime == -1 ? 0 : 1)   // detection value exactly correlates with whether reported time is still -1
                    + "," + _timeSinceStart
                    + "," + (TrialManager.Instance.Data.isForward ? 1 : 0) // 1 = forward; 0 = backward
                    + "," + TrialManager.Instance.Data.currVelocityGain
                    + "," + _startedWalkingTime; // should never be -1 for a completed trial
                _trialFile.WriteLine(trialLogString);
                _trialFile.Flush();

                // determine if training trial variables should be updated
                if (!TrialManager.Instance.Data.training1Done)
                    TrialManager.Instance.Training1Done();
                else if (!TrialManager.Instance.Data.training2Done && _reportedTime != -1) // only complete training 2 if detection reported
                    TrialManager.Instance.Training2Done();
            }

            // always update this so that Update() execution stops at the end of any trial (including last trial)
            _trialDone = true;

            // trial complete, either do another trial or transition to exit scene if fully done
            if (TrialManager.Instance.DoTrialsRemain())
            {
                TrialManager.Instance.SetTrialDone(true);
            }
            else // all trials compelte
            {
                // close StreamWriter
                _trialFile.Close();
                _motionFile.Close();

                // Load exit scene to prompt users to remove headset
                FadeHandler fadeHandler = GameObject.Find("FadeToBlack Handler").GetComponent<FadeHandler>(); // bad practice, but works
                fadeHandler.FadeToLevel("3_ThankYou");
            }
        }

        #endregion

        // Handle timer - used in motion and trial logs
        _timeSinceStart += Time.deltaTime;
    }

    /// <summary>
    /// makes both controllers vibrate for a short duration
    /// </summary>
    public void VibrateEffect()
    {
        Invoke("startVib", .1f);
        Invoke("stopVib", .4f);
    }
    public void startVib()
    {
        OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(1, 1, OVRInput.Controller.LTouch);
    }
    public void stopVib()
    {
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
    }
}
