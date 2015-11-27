using System.Collections.Generic;
using UnityEngine;

public class PointsSystem : MonoBehaviour
{

    public float modifierMin = 0;
    public float modifierMax = float.MaxValue;
    public float modifierBase = 1;
    public int pointsMin = 0;
    public int pointsMax = int.MaxValue;
    public int pointsBase = 0;
    public bool UseBreakdown = false;

    public delegate void IntAction(int val);
    public event IntAction OnChange = delegate { };


    int _currentValue;
    float _currentModifier;
    // <ID of what gave the points, <Multiplier at the time, number of times points were given>>
    Dictionary<string, Dictionary<float, int>> _breakdown;
    // <ID of multiplier, multiplier value>
    public Dictionary<string, float> _modifiers;

    void Start()
    {
        _currentValue = pointsBase;
        _currentModifier = modifierBase;
        _breakdown = new Dictionary<string, Dictionary<float, int>>();
        _modifiers = new Dictionary<string, float>();
    }

    void _incrementBreakdown(string id)
    {
        if (_breakdown.ContainsKey(id))
        {
            if (_breakdown[id].ContainsKey(getCurrentModifier()))
            {
                ++_breakdown[id][getCurrentModifier()];
                return;
            }
            _breakdown[id].Add(getCurrentModifier(), 1);
            return;
        }
        Dictionary<float, int> modDic = new Dictionary<float, int>();
        modDic.Add(getCurrentModifier(), 1);
        _breakdown.Add(id, modDic);
    }

    void _calcCurrentModifier()
    {
        _currentModifier = modifierBase;
        foreach (float modifier in _modifiers.Values)
        {
            _currentModifier *= modifier;
        }
        _currentModifier = Mathf.Clamp(_currentModifier, modifierMin, modifierMax);
    }

    public float getCurrentModifier()
    {
        return _currentModifier;
    }

    public void addModifier(float modifier, string id)
    {
        _modifiers.Add(id, modifier);
        _calcCurrentModifier();
    }

    public void removeModifier(string id)
    {
        _modifiers.Remove(id);
        _calcCurrentModifier();
    }

    // Sets the total points. Ignores any modifiers
    public void setPoints(int value, string id)
    {
        _currentValue = Mathf.Clamp(value, pointsMin, pointsMax);
        if (UseBreakdown) {
            Dictionary<float, int> modDic = new Dictionary<float, int>();
            modDic.Add(modifierBase, 1);
            _breakdown.Add(id, modDic);
        }
        OnChange(_currentValue);
    }

    public int getPoints()
    {
        return _currentValue;
    }

    public void addPoints(int value, string id)
    {
        _currentValue += (int)((float)value * _currentModifier);
        if (UseBreakdown) {
            _incrementBreakdown(id);
        }
        _currentValue = (int)Mathf.Clamp((float)_currentValue, pointsMin, pointsMax);
        OnChange(_currentValue);
    }

    public Dictionary<string, Dictionary<float, int>> getBreakdown()
    {
        // Make a deep copy of the breakdown dictionary.
        Dictionary<string, Dictionary<float, int>> newDic = new Dictionary<string, Dictionary<float, int>>();
        foreach (KeyValuePair<string, Dictionary<float, int>> x in _breakdown)
        {
            Dictionary<float, int> innerDic = new Dictionary<float, int>(x.Value);
            newDic.Add(x.Key, innerDic);
        }
        return newDic;
    }

    // This basic one doesn't give you any multipliers
    public Dictionary<string, int> getBasicBreakdown()
    {
        Dictionary<string, int> newDic = new Dictionary<string, int>();
        foreach (KeyValuePair<string, Dictionary<float, int>> x in _breakdown)
        {
            int total = 0;
            foreach (KeyValuePair<float, int> y in x.Value)
            {
                total += y.Value;
            }
            newDic.Add(x.Key, total);
        }
        return newDic;
    }

    public void clearModifiers()
    {
        _currentModifier = modifierBase;
        _modifiers.Clear();
    }

    public void clearPoints()
    {
        _currentValue = pointsBase;
        _breakdown.Clear();
        OnChange(_currentValue);
    }

    public void reset()
    {
        clearPoints();
        clearModifiers();
    }
}
