using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour 
{
    public Transform _TargetObject;
    public float _MinZoom = 5;
    public float _MaxZoom = 20;
    public float _ZoomModifier = 1;
    public float _HorizontalModifier = 1;
    public float _VerticalModifier = 1;
    public float _radius = 100;

	// Use this for initialization
	void Start () 
    {
        _radius = Vector3.Distance(this.transform.position, _TargetObject.transform.position);
        UpdateTransform();
	}
	
	// Update is called once per frame
	void Update () 
    {
        bool changesMade = false;

        //get any change in zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            _radius -= (Input.GetAxis("Mouse ScrollWheel") * _ZoomModifier);
            _radius = Mathf.Clamp(_radius, _MinZoom, _MaxZoom);
            changesMade = true;
        }

        //middle mouse button down 
	    if(Input.GetMouseButton(2))
        {
            //get the mouse delta in both axis to move camera
            Vector3 toMove = (this.transform.right * (Input.GetAxis("Mouse X")*_HorizontalModifier)) + (this.transform.up * (Input.GetAxis("Mouse Y")*_VerticalModifier));
            this.transform.position += toMove;
            changesMade = true;
        }

        //update rotation and distance from target
        if(changesMade)
            UpdateTransform();
        
	}

    void UpdateTransform()
    {
        this.transform.LookAt(_TargetObject.transform.position);
        this.transform.position = Vector3.Normalize(this.transform.position - _TargetObject.transform.position) * _radius;
    }
}
