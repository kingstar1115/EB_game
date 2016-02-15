using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(LerpRotation))]
public class CardUI : MonoBehaviour 
{
    //events 
    public delegate void CardUIAction(CardUI cardUI);
    public event CardUIAction OnPointerEnter = delegate { };
    public event CardUIAction OnPointerExit = delegate { };
    public event CardUIAction OnPointerUp = delegate { };

    public Image _Image;
    public Text _valDisplay1;
    public LerpPosition _PosLerp;
    public UIMouseEvents _MouseEvents;
    public float _ImageYAlt;
    public CardData _Card;
    float _imageYDefualt;
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

    int _index;
    public int _Index
    {
        get { return _index; }
        set { _index = value; }
    }

    public int _Value
    {
        set
        {
            _valDisplay1.text = (value > 0) ? value.ToString() : "";
        }
    }

	// Use this for initialization
	public void Init()
    {
        _lerpRotation = GetComponent<LerpRotation>();

        //event handlers for the card
        _MouseEvents.OnPointerEnterUI += _MouseEvents_OnPointerEnterUI;
        _MouseEvents.OnPointerExitUI += _MouseEvents_OnPointerExitUI;
        _MouseEvents.OnPointerUpUI += _MouseEvents_OnPointerUpUI;

        //set y defualt 
        _imageYDefualt = _Image.transform.localPosition.y;
	}

    void _MouseEvents_OnPointerUpUI(UnityEngine.EventSystems.PointerEventData eventData)
    {
        OnPointerUp(this);
    }

    void _MouseEvents_OnPointerExitUI(UnityEngine.EventSystems.PointerEventData eventData)
    {
        OnPointerExit(this);
    }

    void _MouseEvents_OnPointerEnterUI(UnityEngine.EventSystems.PointerEventData eventData)
    {
        OnPointerEnter(this);
    }

    public void Popup()
    {
        //lerp image to y popup
        _Image.GetComponent<LerpPosition>().LerpTo(new Vector3(_Image.transform.localPosition.x, _ImageYAlt, _Image.transform.localPosition.z));
    }

    public void PopDown()
    {
        _Image.GetComponent<LerpPosition>().LerpTo(new Vector3(_Image.transform.localPosition.x, _imageYDefualt, _Image.transform.localPosition.z));
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
