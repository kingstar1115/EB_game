using UnityEngine;
using System.Collections;

public class LerpRotation : MonoBehaviour 
{
    public event System.Action OnLerpFinished = delegate { };

    public LerpType _lerpType;
    public bool _LocalRotation;
    public float _LerpTime = 1f;
    float _currentLerpTime;
    float _prevPercent = 0;
    Quaternion _startRotation;
    Quaternion _endRotation;
    bool _isLerping;

    // Update is called once per frame
    void Update()
    {
        //get lerping
        if (_isLerping)
        {
            //update currentlerp time
            _currentLerpTime += Time.deltaTime;
            if (_currentLerpTime > _LerpTime)
            {
                //this.transform.position = _endPosition;
                setRotation(_endRotation);
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
                setRotation(Quaternion.Lerp(_startRotation, _endRotation, _prevPercent));
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

    public void LerpTo(Quaternion newRot)
    {
        //LerpTo(this.transform.position, newPosition, 0);
        LerpTo(getRotation(), newRot, 0);
    }

    public void LerpTo(Quaternion newStartRot, Quaternion newEndRot, float currentLerpTime)
    {
        _startRotation = newStartRot;
        _endRotation = newEndRot;
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
        _endRotation = getRotation();
        _startRotation = getRotation();
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

    public Quaternion GetEndRotation()
    {
        return _endRotation;
    }

    public bool IsLerping()
    {
        return _isLerping;
    }

    void setRotation(Quaternion newRotation)
    {
        if (_LocalRotation)
            this.transform.localRotation = newRotation;
        else
            this.transform.rotation = newRotation;
    }

    Quaternion getRotation()
    {
        return (_LocalRotation) ? this.transform.localRotation : this.transform.rotation;
    }
}
