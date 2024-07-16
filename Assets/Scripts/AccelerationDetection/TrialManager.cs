using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Utilizes a singleton pattern and creates itself as a persistent object in the scene upon its first static access.
/// Stores trial tracking data for trial randomization and other data for logging or scene transitions.
/// </summary>
public class TrialManager : MonoBehaviour
{
    // trial count constants
    const int NO_ACCEL_TRIALS_COUNT = 20;
    const int CONDITION_TRIALS_COUNT = 10; // trials per condition
    // experimental acceleration constants - units of velocity gain per second
    const float NO_ACCEL = 0f;
    readonly float[] ACCELS = { 0.05f, 0.1f, 0.15f, 0.2f, 0.25f, 0.3f };
    const float TRAINING_HIGH_ACCEL = 0.5f; // even higher than fastest trial to reduce bias

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

        // data used for training trials
        public bool testAlignDone;
        public bool training1Done;
        public bool training2Done;

        // data used for trial tracking
        public int noAccelLeft;
        public int[] accelsLeft;

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
                _data.testAlignDone = false;
                _data.training1Done = false;
                _data.training2Done = false;
                _data.noAccelLeft = NO_ACCEL_TRIALS_COUNT;
                _data.accelsLeft = new int[ACCELS.Length];
                for (int i = 0; i < ACCELS.Length; i++)
                {
                    _data.accelsLeft[i] = CONDITION_TRIALS_COUNT;
                }
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
    /// for tracking that the test align trial was completed.
    /// Should be called from AccelerationLogger.cs when end of trial detected.
    /// </summary>
    public void testAlignDone()
    {
        Data.training1Done = true;
    }

    /// <summary>
    /// for tracking that training trial 1 was completed.
    /// Should be called from AccelerationLogger.cs when end of trial detected.
    /// </summary>
    public void Training1Done()
    {
        Data.training1Done = true;
    }

    /// <summary>
    /// for tracking that training trial 2 was completed.
    /// Should be called from AccelerationLogger.cs when end of trial detected.
    /// </summary>
    public void Training2Done()
    {
        Data.training2Done = true;
    }

    /// <summary>
    /// returns acceleration value to use in next trial.
    /// increments trial counters accordingly.
    /// </summary>
    public float GetNewTrialAccel()
    {
        // always increment trial number of this is called
        Data.trialNum++;

        if (!Data.testAlignDone)
        {
            Data.trialAccel = NO_ACCEL;
            return NO_ACCEL;
        }
        else if (!Data.training1Done) // training trial 1 is always no acceleration
        {
            Data.trialAccel = NO_ACCEL;
            return NO_ACCEL;
        }
        else if(!Data.training2Done) // training trial 2 is always very high acceleration
        {
            Data.trialAccel = TRAINING_HIGH_ACCEL;
            return TRAINING_HIGH_ACCEL;
        }

        // generate random number for trial to select
        int trialsRemaining = Instance.Data.noAccelLeft + Instance.Data.accelsLeft.Sum();
        int rand = Random.Range(0, trialsRemaining);

        // no accel trial selected
        if (rand < Instance.Data.noAccelLeft) 
        {
            Data.noAccelLeft--;
            Data.trialAccel = NO_ACCEL;
            return NO_ACCEL; 
        }
        else
        {
            // sum starts as only previous no accel trials
            int currSum = Instance.Data.noAccelLeft + Instance.Data.accelsLeft[0];
            for (int i = 0; i < Instance.Data.accelsLeft.Length; i++)
            {
                currSum += Data.accelsLeft[i];
                if(rand < currSum)
                {
                    Data.accelsLeft[i]--;
                    Data.trialAccel = ACCELS[i];
                    return ACCELS[i];
                }
            }
        }

        // failsafe return - should always be handled in previous for loop
        return ACCELS[ACCELS.Length-1];
    }

    /// <summary>
    /// returns false only if ALL expected trials have been completed.
    /// </summary>
    public bool DoTrialsRemain()
    {
        // any no accel trials left?
        if (Data.noAccelLeft > 0) return true;

        // any condition trials left?
        for (int i = 0; i < Data.accelsLeft.Length; i++)
            if (Data.accelsLeft[i] > 0) return true;

        // no trials remain if all are at 0
        return false;
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
