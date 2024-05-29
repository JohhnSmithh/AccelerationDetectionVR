using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public int noAccelLeft;
        public int lowAccelLeft;
        public int medAccelLeft;
        public int highAccelLeft;
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
                _data.noAccelLeft = NO_ACCEL_TRIALS_COUNT;
                _data.lowAccelLeft = CONDITION_TRIALS_COUNT;
                _data.medAccelLeft = CONDITION_TRIALS_COUNT;
                _data.highAccelLeft = CONDITION_TRIALS_COUNT;
            }
            return _data;
        }
        private set
        {
            _data = value;
        }
    }

    /// <summary>
    /// returns acceleration value to use in next trial.
    /// increments trial counters accordingly..
    /// </summary>
    public float GetNewAccel()
    {
        // generate random number for trial to select
        int trialsRemaining = Instance.Data.noAccelLeft + Instance.Data.lowAccelLeft + Instance.Data.medAccelLeft + Instance.Data.highAccelLeft;
        int rand = Random.Range(0, trialsRemaining);

        // no accel trial selected
        if (rand < Instance.Data.noAccelLeft) 
        {
            Instance.Data.noAccelLeft--;
            return NO_ACCEL; 
        }
        // low accel trial selected
        else if (rand < Instance.Data.noAccelLeft + Instance.Data.lowAccelLeft)
        {
            Instance.Data.lowAccelLeft--;
            return LOW_ACCEL;
        }
        // med accel trial selected
        else if (rand < Instance.Data.noAccelLeft + Instance.Data.lowAccelLeft + Instance.Data.medAccelLeft)
        {
            Instance.Data.medAccelLeft--;
            return MED_ACCEL;
        }
        // high accel trial selected
        else
        {
            Instance.Data.highAccelLeft--;
            return HIGH_ACCEL;
        }
    }

    /// <summary>
    /// returns false only if ALL expected trials have been completed
    /// </summary>
    public bool DoTrialsRemain()
    {
        return _data.noAccelLeft > 0 || _data.lowAccelLeft > 0 || _data.medAccelLeft > 0 || _data.highAccelLeft > 0;
    }
    
    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {}
}
