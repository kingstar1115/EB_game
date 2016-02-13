using UnityEngine;
using System.Collections;

public class LerpPosition : MonoBehaviour 
{
    public event System.Action OnLerpFinished = delegate { };

    public LerpType _lerpType;
    public bool _LocalPosition;
    public float _LerpTime = 1f;
    float _currentLerpTime;
    float _prevPercent = 0;
    Vector3 _startPosition;
    Vector3 _endPosition;
    bool _isLerping;
	bool _isPaused;
	
	// Update is called once per frame
	void Update () 
    {
        //get lerping
		if (_isLerping && !_isPaused)
        {
            //update currentlerp time
            _currentLerpTime += Time.deltaTime;
            if (_currentLerpTime > _LerpTime)
            {
                //this.transform.position = _endPosition;
                setPosition(_endPosition);
                StopLerp();
                OnLerpFinished();
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

                //transform.position = Vector3.Lerp(_startPosition, _endPosition, _prevPercent);
                setPosition(Vector3.Lerp(_startPosition, _endPosition, _prevPercent));
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
        //LerpTo(this.transform.position, newPosition, 0);
        LerpTo(getPosition(), newPosition, 0);
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
        _isPaused = true;
    }

	public void ResumeLerp() {
		_isPaused = false;
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
        _endPosition = getPosition();
        _startPosition = getPosition();
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

	public Vector3 GetStartPosition() {
		return _startPosition;
	}

    public Vector3 GetEndPosition()
    {
        return _endPosition;
    }

    public bool IsLerping()
    {
        return _isLerping;
    }

    void setPosition(Vector3 newPosition)
    {
        if (_LocalPosition)
            this.transform.localPosition = newPosition;
        else
            this.transform.position = newPosition;
    }

    Vector3 getPosition()
    {
        return (_LocalPosition) ? this.transform.localPosition : this.transform.position;
    }
}

public enum LerpType
{
    EaseOut, EaseIn, Smoothstep,
}
