using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityGain : MonoBehaviour
{
    [Header("Debug Controls")]
    [Tooltip("Multiplier for camera rig planar velocity changes; 0.0 = no velocity gain. This value is only used if debug trial is enabled")] 
    public float Acceleration = 0.05f;
    [Tooltip("enable if you want to manually test acceleration values")] 
    private bool isSingleDebugTrial = false;

    private float _currVelocityGain = 1.0f; // 1.0 = standard behavior
    private Vector3 _prevPosition;
    private Vector3 _startOffset = Vector3.up; // used for logging motion, so that it always starting at 0

    // Start is called before the first frame update
    void Start()
    {
        // reset stored positions for current trial
        TrialManager.Instance.SetCurrentRealPos(Vector3.zero);
        TrialManager.Instance.SetCurrentVirtualPos(Vector3.zero);

        if (!isSingleDebugTrial)
            Acceleration = TrialManager.Instance.GetNewTrialAccel();

        // TODO: REMOVE THIS
        Debug.Log("CURRENT ACCEL: " + Acceleration);
    }

    // Update is called once per frame
    void Update()
    {
        // determine startOffset (must be in Update so position is not read as zero)
        if (_startOffset == Vector3.up)
        {
            _startOffset = Camera.main.transform.position;
            _startOffset.y = 0; // ignore vertical component
        }

        // calculate position change without gain
        Vector3 newPos = Camera.main.gameObject.transform.localPosition;
        newPos = new Vector3(newPos.x, 0, newPos.z);
        Vector3 dif = newPos - _prevPosition; //amount of movement

        // apply translation gain from current calculated velocity gain
        // subtract 1.0 so that the standard translation movement (applied by another script) is not re-applied
        this.transform.Translate(dif * (_currVelocityGain - 1.0f));

        // save current position for next frame
        _prevPosition = newPos;

        // increase velocity gain based on constant acceleration
        _currVelocityGain += Acceleration * Time.deltaTime;

        // update values in TrialManager (for logging)
        TrialManager.Instance.SetCurrentVelocityGain(_currVelocityGain);
        TrialManager.Instance.SetCurrentRealPos((TrialManager.Instance.Data.isForward ? 1 : -1) * // flip logged vector if moving is reversed
            new Vector3(Camera.main.gameObject.transform.localPosition.x, 0, 
            Camera.main.gameObject.transform.localPosition.z) - _startOffset); // camera pos relative to transformed parent
        TrialManager.Instance.SetCurrentVirtualPos((TrialManager.Instance.Data.isForward ? 1 :-1) * // flip logged vector if moving is reversed
            new Vector3(Camera.main.gameObject.transform.position.x, 0, 
            Camera.main.gameObject.transform.position.z) - _startOffset); // absolute camera pos in virtual world
    }
}
