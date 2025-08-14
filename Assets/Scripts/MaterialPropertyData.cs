using DG.Tweening;
using UnityEngine;

//Base class for all material property types used for the reticle profiles.
//Can apply itself to a target material either directly, or through a transition using tweens.
[System.Serializable]
public abstract class MaterialPropertyData
{
    public string propertyName;
    public ReticleMaterialLayer reticleLayer;
    public DynamicPropertyType dynamicPropertyType = DynamicPropertyType.None;

    //Instantly applies this property to the target material.
    //Used for instant profile switches or initialization.
    public abstract void ApplyProfile(Material mat);

    //Creates a tween to smoothly transition from the material's current values to the property values.
    //Return type tween to later add all transition tweens to a list for targeted cancellation.
    public abstract Tween TweenToProfile(Material mat, float duration);

    //Return either base or end value of this property.
    public abstract object GetValue(bool baseValue);
}

//Class for float properties (floats, ranges).
[System.Serializable]
public class MaterialFloatProperties: MaterialPropertyData
{
    public float value;
    [HideInInspector]
    public float endValue;

    public override void ApplyProfile(Material mat)
    {
        mat.SetFloat(propertyName, value);
    }

    public override Tween TweenToProfile(Material mat, float duration)
    {
        return mat.DOFloat(value, propertyName, duration);
    }

    public override object GetValue(bool baseValue)
    {
        if (!baseValue)
        {
            return endValue;
        }
        else
        {
            return value;
        }
    }
}

//Class for color properties.
[System.Serializable]
public class MaterialColorProperties: MaterialPropertyData
{
    public Color value;
    [HideInInspector]
    public Color endValue;

    public override void ApplyProfile(Material mat)
    {
        mat.SetColor(propertyName, value);
    }

    public override Tween TweenToProfile(Material mat, float duration)
    {
        return mat.DOColor(value, propertyName, duration);
    }

    public override object GetValue(bool baseValue)
    {
        if (!baseValue)
        {
            return endValue;
        }
        else
        {
            return value;
        }
    }
}

//Class for boolean keyword properties.
[System.Serializable]
public class MaterialKeywordProperties: MaterialPropertyData
{
    public bool keywordEnabled;

    public override void ApplyProfile(Material mat)
    {
        if (keywordEnabled)
        {
            mat.EnableKeyword(propertyName);
        }
        else
        {
            mat.DisableKeyword(propertyName);
        }
    }

    //Can't tween a boolean, so just apply it instantly.
    public override Tween TweenToProfile(Material mat, float duration)
    {
        ApplyProfile(mat);
        return null;
    }

    public override object GetValue(bool baseValue)
    {
        if (!baseValue)
        {
            return keywordEnabled;
        }
        else
        {
            return keywordEnabled;
        }
    }
}

////DELETE
////Class for the properties that should be ignored when applying profiles
////In this case just floats, but can be extended to support other property types
//[System.Serializable]
//public class DynamicMaterialProperties
//{
//    public string propertyName;
//    public DynamicPropertyType dynamicPropertyType;
//    public ReticleMaterialLayer reticleLayer;
//}