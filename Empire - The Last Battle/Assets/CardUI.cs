using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(LerpRotation))]
public class CardUI : MonoBehaviour 
{
    public Image _Image;
    public LerpPosition _PosLerp;
    LerpRotation _lerpRotation;

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
            _PosLerp.LerpTo(new Vector3(_pivotVector.x, _pivotVector.y, 0));
        }
    }

    public Quaternion _TargetRotation
    {
        get
        {
            return _lerpRotation.GetEndRotation();
        }

        set
        {
            _lerpRotation.LerpTo(value);
        }
    }

	// Use this for initialization
	public void Init()
    {
        _lerpRotation = GetComponent<LerpRotation>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
