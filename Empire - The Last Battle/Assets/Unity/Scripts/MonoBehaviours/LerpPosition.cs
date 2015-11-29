using UnityEngine;
using System.Collections;

public class LerpPosition : MonoBehaviour 
{
    public LerpType _lerpType;
    public float _LerpTime = 1f;
    float _currentLerpTime;
    float _prevPercent;
    Vector3 _startPosition;
    Vector3 _endPosition;
    bool _isLerping;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        //get lerping
        if (_isLerping)
        {
            //update currentlerp time
            _currentLerpTime += Time.deltaTime;
            if (_currentLerpTime > _LerpTime)
            {
                _startPosition = _endPosition;
                _currentLerpTime = 0;
                _isLerping = false;
            }
            else
            {
                //lerp position
                float _prevPercent = GetCurrentPercent();
                if (_lerpType == LerpType.EaseIn)
                    _prevPercent = GetEaseIn(_prevPercent);
                else if (_lerpType == LerpType.EaseOut)
                    _prevPercent = GetEaseOut(_prevPercent);
                else if (_lerpType == LerpType.Smoothstep)
                    _prevPercent = GetSmoothstep(_prevPercent);

                transform.position = Vector3.Lerp(_startPosition, _endPosition, _prevPercent);
            }
        }
	}

    public float GetCurrentPercent()
    {
        return _currentLerpTime / _LerpTime;
    }

    public float GetSpeed()
    {
        return GetCurrentPercent() - _prevPercent;
    }

    public void LerpTo(Vector3 newPosition)
    {
        LerpTo(this.transform.position, newPosition, 0);
    }

    public void LerpTo(Vector3 newStartPos, Vector3 newEndPos, float currentLerpTime)
    {
        _startPosition = newStartPos;
        _endPosition = newEndPos;
        _currentLerpTime = currentLerpTime;
        _isLerping = true;
    }

    public void PauseLerp()
    {
        _isLerping = false;
    }

    public void StartLerp()
    {
        _isLerping = true;
    }

    public void StopLerp()
    {
        _isLerping = false;
        ResetLerp();
    }

    public void ResetLerp()
    {
        _currentLerpTime = 0;
        _endPosition = this.transform.position;
        _startPosition = this.transform.position;
    }

    public float GetEaseOut(float percent)
    {
        return Mathf.Sin(percent * Mathf.PI * 0.5f);
    }

    public float GetEaseIn(float percent)
    {
        return 1f - Mathf.Cos(percent * Mathf.PI * 0.5f);
    }

    public float GetSmoothstep(float percent)
    {
        return percent * percent * (3f - 2f * percent);
    }

    public Vector3 GetEndPosition()
    {
        return _endPosition;
    }
}

public enum LerpType
{
    EaseOut, EaseIn, Smoothstep,
}
