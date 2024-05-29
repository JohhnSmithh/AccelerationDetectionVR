using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class EyeTrackingRay : MonoBehaviour
{
    public float distance = 1.0f;
    public float width = 0.01f;

    public LayerMask layersToInclude;
    public Color defaultColor = Color.yellow;
    public Color altColor = Color.red;


    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
    }

    void SetupRay()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = width ;
        lineRenderer.endWidth = width;
        lineRenderer.startColor = defaultColor;
        lineRenderer.endColor = defaultColor;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y,
            transform.position.z + distance));

    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 raycastdir = transform.TransformDirection(Vector3.forward) * distance;
        if (Physics.Raycast(transform.position, raycastdir, out hit, Mathf.Infinity))
        {
            lineRenderer.startColor = altColor;
            lineRenderer.endColor = defaultColor;

        }else{
            lineRenderer.startColor = defaultColor;
            lineRenderer.endColor = defaultColor;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
