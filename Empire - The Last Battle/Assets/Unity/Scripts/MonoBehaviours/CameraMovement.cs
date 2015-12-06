using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
    public Transform _TargetObject;
    public float _ZoomInAngle = 10;
    public float _ZoomOutAngle = 85;
    public float _MinZoom = 5;
    public float _MaxZoom = 20;
    public float _ZoomModifier = 1;
    public float _HorizontalModifier = 1;
    public float _VerticalModifier = 1;
    float _radius = 100;

	// Use this for initialization
	void Start () 
    {
        //start half way zoomed
        _radius = Mathf.Lerp(5,20,0.5f);
        UpdateTransform(Vector3.zero);
	}
	
	// Update is called once per frame
	void Update () 
    {
        bool changesMade = false;
        Vector3 toMoveHorizontal = Vector3.zero;

        //get any change in zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            _radius -= (Input.GetAxis("Mouse ScrollWheel") * _ZoomModifier);
            _radius = Mathf.Clamp(_radius, _MinZoom, _MaxZoom);
            changesMade = true;
        }

        //right mouse button down 
	    if(Input.GetMouseButton(1))
        {
            //get the mouse delta in both axis to move camera
            toMoveHorizontal = (Vector3.Normalize(Vector3.ProjectOnPlane(-this.transform.right, Vector3.up)) * (Input.GetAxis("Mouse X") * _HorizontalModifier));//(this.transform.up * (Input.GetAxis("Mouse Y")*_VerticalModifier));
            changesMade = true;
        }

        //update rotation and distance from target
        if(changesMade)
            UpdateTransform(toMoveHorizontal);
	}

    void UpdateTransform(Vector3 toMove)
    {
        //move to origin and add position to move
        this.transform.position -= _TargetObject.transform.position;
        this.transform.position += toMove;
        this.transform.LookAt(Vector3.zero);

        //wierd zooming
        Vector3 forwardOnUpPlane = Vector3.Normalize(Vector3.ProjectOnPlane(-this.transform.forward, Vector3.up));
        Vector3 minDirection = Quaternion.AngleAxis(_ZoomInAngle, this.transform.right) * forwardOnUpPlane;
        Vector3 maxDirection = Quaternion.AngleAxis(_ZoomOutAngle, this.transform.right) * forwardOnUpPlane;
        Vector3 newZoomedForward = Vector3.Lerp(minDirection, maxDirection, _radius / _MaxZoom);

        //update position and rotation
        this.transform.position = newZoomedForward * _radius;
        this.transform.LookAt(Vector3.zero);
        this.transform.position += _TargetObject.transform.position;
        
    }
}
