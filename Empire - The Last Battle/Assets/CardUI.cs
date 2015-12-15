using UnityEngine;
using System.Collections;

public class CardUI : MonoBehaviour 
{
    public LerpPosition _CardImage;

    float _zRotation;
    public float _ZRotation
    {
        get
        {
            return _zRotation;
        }

        set
        {
            this.transform.rotation = Quaternion.Euler(0, 0, value);
            _zRotation = value;
        }
    }

    Vector2 _pivotVector;
    public Vector2 _PivotVector
    {
        get
        {
            return _pivotVector;
        }

        set
        {
            _pivotVector = value;
            _CardImage.LerpTo(new Vector3(_pivotVector.x, _pivotVector.y, 0));
        }
    }

	// Use this for initialization
	void Start () 
    {
        _PivotVector = new Vector2(0,12);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
