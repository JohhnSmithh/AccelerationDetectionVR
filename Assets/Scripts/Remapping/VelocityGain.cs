using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityGain : MonoBehaviour
{
    [Tooltip("Multiplier for camera rig planar velocity changes; 0.0 = no velocity gain")] public float Acceleration = 0.05f;

    private float _currVelocityGain = 1.0f; // 1.0 = standard behavior
    private Vector3 _prevPosition;

    // Start is called before the first frame update
    void Start()
    {}

    // Update is called once per frame
    void Update()
    {
        // calculate position change without gain
        Vector3 newPos = Camera.main.gameObject.transform.localPosition;
        newPos = new Vector3(newPos.x, 0, newPos.z);
        Vector3 dif = newPos - _prevPosition; //amount of movement

        // apply translation gain from current calculated velocity gain
        // subtract 1.0 so that the standard translation movement (applied by another script) is not re-applied
        this.transform.Translate(dif * (_currVelocityGain - 1.0f));

        // save current position for next frame
        _prevPosition = newPos;

        // TODO: figure out if gain is applied over time or over distance
        // increase velocity gain based on constant acceleration
        _currVelocityGain += Acceleration * Time.deltaTime;
    }
}