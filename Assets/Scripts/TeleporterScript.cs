using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : MonoBehaviour {

    //formulas taken from: https://www.youtube.com/watch?v=iLlWirdxass

    public float velocity;
    public float angle;
    public int resolution = 10;

    public Transform leftHandAnchor;

    private bool _pressedTrigger;

    private LineRenderer _lineRenderer;

    private Vector3[] _linePoints;
    private float g; //force of gravity on the y axis
    private float radianAngle;

    public void Awake() {
        _pressedTrigger = false;

        _lineRenderer = leftHandAnchor.GetComponent<LineRenderer>();
    }

    public void Update() {
        //if left index trigger is pressed down
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) != 0) {
            _linePoints = new Vector3[resolution + 1];

            radianAngle = Mathf.Deg2Rad * angle;
            float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;

            //fill array
            for(int i = 0; i <= resolution; i++) {
                float t = (float)i / (float)resolution;
                _linePoints[i] = CalculateArcPoint(t, maxDistance);
            }

            //draw line
            //_lineRenderer.SetVertexCount(resolution + 1); //might not be needed
            _lineRenderer.SetPositions(_linePoints);

            _pressedTrigger = true;
        }

        //if the left index trigger was previously pressed and now is released
        if(OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) == 0 && _pressedTrigger) {
            transform.Translate(_linePoints[_linePoints.Length - 1]); //teleport to last vector in the array

            _linePoints = new Vector3[0];
            //_lineRenderer.SetVertexCount(0); //might not be needed
            _lineRenderer.SetPositions(_linePoints); //apply an empty array to not draw anything

            _pressedTrigger = false;

            Debug.Log("Left index trigger released");
        }
    }

    private Vector3 CalculateArcPoint(float t, float maxDistance) {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((g * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
        return new Vector3(x, y);
    }
}
