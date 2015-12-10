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
    Vector3 _targetPrevPosition;
    float _radius = 100;
    bool _canMoveCamera;

	// Use this for initialization
	void Start () 
    {
        //start half way zoomed
        _radius = Mathf.Lerp(_MinZoom, _MaxZoom, 0.5f);
        UpdateTransform(Vector3.zero);

        _canMoveCamera = true;
        this.GetComponent<LerpPosition>().OnLerpFinished+=CameraMovement_OnLerpFinished;
	}

    void CameraMovement_OnLerpFinished()
    {
        _canMoveCamera = true;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (_canMoveCamera)
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
            if (Input.GetMouseButton(1))
            {
                //get the mouse delta in both axis to move camera
                toMoveHorizontal = (Vector3.Normalize(Vector3.ProjectOnPlane(-this.transform.right, Vector3.up)) * (Input.GetAxis("Mouse X") * _HorizontalModifier));//(this.transform.up * (Input.GetAxis("Mouse Y")*_VerticalModifier));
                changesMade = true;
            }

            //update rotation and distance from target
            if (changesMade)
                UpdateTransform(toMoveHorizontal);
        }
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

    public void DisableCameraMovement()
    {
        _targetPrevPosition = _TargetObject.position;
        _canMoveCamera = false;
    }

    public void EnableCameraMovement(Vector3 newFocusPoint)
    {
        //lerp to new camera position else handle it 
        if (_targetPrevPosition != newFocusPoint)
			this.GetComponent<LerpPosition> ().LerpTo ((newFocusPoint - _targetPrevPosition) + this.transform.position);

    }

	public void EnableCameraMovement()
	{
		_canMoveCamera = true;
	}

	public bool IsLerping()
	{
		return this.GetComponent<LerpPosition> ().IsLerping();
	}
}
