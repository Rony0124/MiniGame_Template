using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MiniGameInfo", menuName = "Scriptable Object/MiniGameInfo", order = int.MaxValue)]
public class MiniGameInfo : ScriptableObject
{
    public enum ResultRecordOrder
    {
        BiggerNumber,
        LesserTimeSpan,
        BiggerTimeSpan,
        BonusTries
    }
    
    [Serializable]
    public struct FloatValue
    {
        public string name;
        public float value;
    }

    [Serializable]
    public struct IntValue
    {
        public string name;
        public int value;
    }

    [Serializable]
    public struct StringValue
    {
        public string name;
        public string value;
    }

    [Serializable]
    public struct StringsValue
    {
        public string name;
        public string[] value;
    }

    [Serializable]
    public class PrefabValue
    {
        public string name;
        public GameObject prefab;
    }
    
    [Serializable]
    public struct DifficultyValue
    {
        public string valueName;
        public AnimationCurve curve;
    }
    
    [Header("Complete within a time limit")]
    public bool completeWithinTimeLimit = false;
    
    [Header("Score")]
    public ResultRecordOrder resultRecordOrder;
    
    [Header("Difficulty")]
    public List<DifficultyValue> difficultyValues;
    
    [Header("CutOffs")] 
    public List<AnimationCurve> gameCutOffs;
    
    [Header("Constant")]
    [SerializeField]
    private List<FloatValue> floatValues;
    [SerializeField]
    private List<IntValue> intValues;
    [SerializeField]
    private List<StringValue> stringValues;
    [SerializeField]
    private List<StringsValue> stringsValues;
    
    public float GetFloatValue(string name)
    {
        for (int i = 0; i < floatValues.Count; ++i)
        {
            var floatValue = floatValues[i];

            if (floatValue.name == name)
            {
                return floatValue.value;
            }
        }

        return 0.0f;
    }

    public int GetIntValue(string name)
    {
        for (int i = 0; i < intValues.Count; ++i)
        {
            var intValue = intValues[i];

            if (intValue.name == name)
            {
                return intValue.value;
            }
        }

        return 0;
    }

    public string GetStringValue(string name)
    {
        for (int i = 0; i < stringValues.Count; ++i)
        {
            StringValue stringValue = stringValues[i];

            if (stringValue.name == name)
            {
                return stringValue.value;
            }
        }

        return null;
    }

    public string[] GetStringsValue(string name, bool allowSearchBySubstring = false)
    {
        for (int i = 0; i < stringsValues.Count; ++i)
        {
            StringsValue stringsValue = stringsValues[i];

            if (stringsValue.name == name ||
                (allowSearchBySubstring && stringsValue.name.Contains(name)))
            {
                return stringsValue.value;
            }
        }

        return null;
    }

    public AnimationCurve GetDifficultyValueCurve(string valueName)
    {
        for (int i = 0; i < difficultyValues.Count; ++i)
        {
            if (difficultyValues[i].valueName == valueName)
                return difficultyValues[i].curve;
        }
        
        Debug.Assert(false, $"Invalid DifficultyValueCurve: {valueName}");
        return null;
    }
    
    public float GetDifficultyValue(string valueName, float time, float defaultValue = 0.0f)
    {
        AnimationCurve curve = GetDifficultyValueCurve(valueName);

        if (curve != null)
        {
            return curve.Evaluate(time);
        }
        
        return defaultValue;
    }
    
    public int GetDifficultyValueInt(string valueName, float time, int defaultValue = 0)
    {
        return Mathf.FloorToInt(GetDifficultyValue(valueName, time, defaultValue));
    }
}
