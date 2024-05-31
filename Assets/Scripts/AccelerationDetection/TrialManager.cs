using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Utilizes a singleton pattern and creates itself as a persistent object in the scene upon its first static access.
/// Stores trial tracking data for trial randomization and other data for logging or scene transitions.
/// </summary>
public class TrialManager : MonoBehaviour
{
    // trial count constants
    const int NO_ACCEL_TRIALS_COUNT = 15;
    const int CONDITION_TRIALS_COUNT = 5; // trials per condition
    // experimental acceleration constants - units of velocity gain per second
    const float NO_ACCEL = 0f;
    const float LOW_ACCEL = 0.05f;
    const float MED_ACCEL = 0.1f;
    const float HIGH_ACCEL = 0.15f;

    // Setup for singleton pattern
    private static TrialManager _instance;
    public static TrialManager Instance
    {
        get
        {
            // set up TrialManager if not already present
            if(_instance == null)
            {
                // create new TrialManager object
                GameObject newManager = new();
                newManager.name = "Trial Manager";
                newManager.AddComponent<TrialManager>();
                DontDestroyOnLoad(newManager);
                _instance = newManager.GetComponent<TrialManager>();
            }

            return _instance;
        }
    }

    public class TrialData
    {
        public string pid;

        // data used for trial tracking
        public int noAccelLeft;
        public int lowAccelLeft;
        public int medAccelLeft;
        public int highAccelLeft;

        // data used for logger - same during entire trial
        public int trialNum;
        public float trialAccel;
        public bool isForward;

        // data used for logger - can change every frame
        public float currVelocityGain;
        public Vector3 currRealPos;
        public Vector3 currVirtualPos;

        // used to indicate that the trial is ready to be reset
        public bool trialDone;
    }

    private TrialData _data;
    public TrialData Data
    {
        get
        {
            // initialize if uninitialized on access
            if(_data == null)
            {
                _data = new();
                _data.pid = "-1"; // indicates invalid PID entered or no PID entered (should never occur)
                _data.noAccelLeft = NO_ACCEL_TRIALS_COUNT;
                _data.lowAccelLeft = CONDITION_TRIALS_COUNT;
                _data.medAccelLeft = CONDITION_TRIALS_COUNT;
                _data.highAccelLeft = CONDITION_TRIALS_COUNT;
                _data.trialNum = 0;
                _data.trialAccel = -1; // should never be seen since it is overriden at the start of a given trial
                _data.isForward = true; // start out moving forward
                _data.currVelocityGain = 0;
                _data.currRealPos = Vector3.zero; // initial rig pos
                _data.currVirtualPos = Vector3.zero; // initial rig pos
                _data.trialDone = false;
            }
            return _data;
        }
        private set
        {
            _data = value;
        }
    }

    /// <summary>
    /// for updating the PID value for the current set of logged trials.
    /// Should be called from the 0_EnterPID scene before the trials begin.
    /// </summary>
    public void SetPID(string newPID)
    {
        Data.pid = newPID;
    }

    /// <summary>
    /// returns acceleration value to use in next trial.
    /// increments trial counters accordingly.
    /// </summary>
    public float GetNewTrialAccel()
    {
        // generate random number for trial to select
        int trialsRemaining = Instance.Data.noAccelLeft + Instance.Data.lowAccelLeft + Instance.Data.medAccelLeft + Instance.Data.highAccelLeft;
        int rand = Random.Range(0, trialsRemaining);

        // always increment trial number of this is called
        Data.trialNum++;

        // no accel trial selected
        if (rand < Instance.Data.noAccelLeft) 
        {
            Data.noAccelLeft--;
            Data.trialAccel = NO_ACCEL;
            return NO_ACCEL; 
        }
        // low accel trial selected
        else if (rand < Instance.Data.noAccelLeft + Instance.Data.lowAccelLeft)
        {
            Data.lowAccelLeft--;
            Data.trialAccel = LOW_ACCEL;
            return LOW_ACCEL;
        }
        // med accel trial selected
        else if (rand < Instance.Data.noAccelLeft + Instance.Data.lowAccelLeft + Instance.Data.medAccelLeft)
        {
            Data.medAccelLeft--;
            Data.trialAccel = MED_ACCEL;
            return MED_ACCEL;
        }
        // high accel trial selected
        else
        {
            Data.highAccelLeft--;
            Data.trialAccel = HIGH_ACCEL;
            return HIGH_ACCEL;
        }
    }

    /// <summary>
    /// returns false only if ALL expected trials have been completed.
    /// </summary>
    public bool DoTrialsRemain()
    {
        return Data.noAccelLeft > 0 || Data.lowAccelLeft > 0 || Data.medAccelLeft > 0 || Data.highAccelLeft > 0;
    }

    /// <summary>
    /// for updating whether the participant is walking forward or backward.
    /// Updated in the AlignmentScene.
    /// </summary>
    public void SetForward(bool newForward)
    {
        Data.isForward = newForward;
    }

    /// <summary>
    /// for updating the velocity gain stored to be logged.
    /// should be updated in VelocityGain.cs where the value is calculated.
    /// </summary>
    public void SetCurrentVelocityGain(float newGain)
    {
        Data.currVelocityGain = newGain;
    }
   
    /// <summary>
    /// for updating the current real position to be logged in the motion log.
    /// should be updated in VelocityGain.cs where values are calculated.
    /// </summary>
    public void SetCurrentRealPos(Vector3 newPos)
    {
        Data.currRealPos = newPos;
    }

    /// <summary>
    /// for updating the current virtual position to be logged in the motion log.
    /// should be updated in VelocityGain.cs where values are calculated.
    /// </summary>
    public void SetCurrentVirtualPos(Vector3 newPos)
    {
        Data.currVirtualPos = newPos;
    }

    /// <summary>
    /// for indicating that the trial is ready to be reset
    /// should be updated in AccelerationLogger.cs when trial logging is complete
    /// </summary>
    public void SetTrialDone(bool newState)
    {
        Data.trialDone = newState;
    }
    
    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {}
}
