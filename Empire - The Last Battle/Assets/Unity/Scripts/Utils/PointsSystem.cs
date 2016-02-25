using System.Collections.Generic;
using UnityEngine;

public class PointsSystem
{
    public float modifierBase = 1;
    public int pointsMin = 0;
    public int pointsMax = int.MaxValue;
    public int pointsBase = 0;

    public delegate void IntAction(int val);
    public event IntAction OnChange = delegate { };

    int _currentValue;
    float _currentModifier;

    public PointsSystem() {
        _currentValue = pointsBase;
        _currentModifier = modifierBase;
    }

	public void RemoveListeners() {
		OnChange = delegate {};
	}

    public float getCurrentModifier()
    {
        return _currentModifier;
    }

    public void setModifier(float modifier)
    {
        _currentModifier = modifier;
    }

    // Sets the total points. Ignores any modifiers
    public void setPoints(int value)
    {
        _currentValue = Mathf.Clamp(value, pointsMin, pointsMax);
        OnChange(_currentValue);
    }

    public int getPoints()
    {
        return _currentValue;
    }

    public void addPoints(int value)
    {
        _currentValue += (int)((float)value * _currentModifier);
        _currentValue = (int)Mathf.Clamp((float)_currentValue, pointsMin, pointsMax);
        OnChange(_currentValue);
    }

    public void clearModifiers()
    {
        _currentModifier = modifierBase;
    }

    public void clearPoints()
    {
        _currentValue = pointsBase;
        OnChange(_currentValue);
    }

    public void reset()
    {
        clearPoints();
        clearModifiers();
    }
}
