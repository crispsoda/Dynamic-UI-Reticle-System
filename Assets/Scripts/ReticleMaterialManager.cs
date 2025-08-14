using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Manages the reticle materials and their properties.
//Handles the application of reticle profiles and dynamic runtime property updates.
public class ReticleMaterialManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image reticuleImageInner;
    [SerializeField] private Image reticuleImageOuter;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 0.25f;

    //List containing dynamic properties that should be controlled and animated directly.
    //Depending on their DynamicPropertyType (i.e. HitReact) they will not be applied to profiles in
    //ApplyProfile if the profile supports the corresponding feature (canHitReact, canCharge).
    private List<MaterialPropertyData> dynamicPropertiesList = new List<MaterialPropertyData>();

    //A list of all tweens specifically used to transition when applying reticle profiles.
    //Allows targeted interrupting of transition tweens.
    private List<Tween> activeTransitionTweens = new List<Tween>();

    //Instanced runtime materials to avoid changing the base material.
    private Material runtimeInnerReticuleMat;
    private Material runtimeOuterReticuleMat;

    //Initializes the material manager by creating runtime instances of inner and outer reticle materials from their UI images.
    public void Initialize()
    {
        if (reticuleImageInner == null || reticuleImageOuter == null)
        {
            Debug.LogError("Reticle Images not assigned in ReticleMaterialManager");
            return;
        }

        runtimeInnerReticuleMat = Instantiate(reticuleImageInner.material);
        reticuleImageInner.material = runtimeInnerReticuleMat;

        runtimeOuterReticuleMat = Instantiate(reticuleImageOuter.material);
        reticuleImageOuter.material = runtimeOuterReticuleMat;
    }

    //Applies a reticle profile to both inner and outer reticle materials.
    //Changes all property values together, either instantly or through tween transitions.
    public void ApplyProfile(ReticleProfileData targetProfile, bool tween = true)
    {
        //Interrupt any ongoing transitions to reticle modes.
        CancelTweens();

        //Clear list as it will be remade.
        dynamicPropertiesList.Clear();

        //Joins all lists in a profile into one full list to handle profile application in one loop.
        var allProperties = new List<MaterialPropertyData>();

        allProperties.AddRange(targetProfile.innerReticleFloatProperties);
        allProperties.AddRange(targetProfile.outerReticleFloatProperties);

        allProperties.AddRange(targetProfile.innerReticleColorProperties);
        allProperties.AddRange(targetProfile.outerReticleColorProperties);

        allProperties.AddRange(targetProfile.innerReticleKeywordProperties);
        allProperties.AddRange(targetProfile.outerReticleKeywordProperties);

        //Apply each property in the reticle profile
        foreach (var prop in allProperties)
        {
            //Add properties marked as dynamic to list
            if (prop.dynamicPropertyType == DynamicPropertyType.Runtime || prop.dynamicPropertyType == DynamicPropertyType.ChargeBar || prop.dynamicPropertyType == DynamicPropertyType.HitReact)
            {
                dynamicPropertiesList.Add(prop);

                //Don't apply charge properties if this reticle mode supports it.
                //Otherwise there is a visual lag until transition tweens to new reticle mode is completed.
                if (targetProfile.canCharge && prop.dynamicPropertyType == DynamicPropertyType.ChargeBar)
                {
                    continue;
                }
            }

            //HitReactVisibility logic hardcoded, since it works by being transparent at first across all versions.
            if (prop.propertyName == "_HitReactVisibility")
            {
                SetMaterialProperty("_HitReactVisibility", 0f, prop.reticleLayer);
                continue;
            }

            //Determine which runtime material instance to apply the properties to based on the property's layer.
            Material targetMat;
            switch (prop.reticleLayer)
            {
                case ReticleMaterialLayer.Outer:
                    targetMat = runtimeOuterReticuleMat;
                    break;
                case ReticleMaterialLayer.Inner:
                    targetMat = runtimeInnerReticuleMat;
                    break;
                default:
                    targetMat = null;
                    Debug.LogError($"Profile Property {prop.propertyName} does not have Reticle Material Layer assigned");
                    break;
            }

            //Skip if no layer is set.
            if (targetMat == null)
                continue;

            //Apply property either instantly or by transitioning to it.
            if (tween)
            {
                //Transition to new values using the property's method.
                Tween t = prop.TweenToProfile(targetMat, transitionDuration);
                if (t != null)
                    AddTween(t);
            }
            else 
            {
                //Apply immediately using the property's method.
                prop.ApplyProfile(targetMat);
            }
        }
    }

    #region Runtime Property Setters
    //For runtime changes to material properties.
    //Overloaded methods for either instant application or tween transitions for both float and color values.

    //Sets float property instantly using ReticleMaterialLayer to determine target material.
    public void SetMaterialProperty(string propertyName, float value, ReticleMaterialLayer layer)
    {
        //Set material to apply property to based on reticle layer.
        Material targetMat = layer switch
        {
            ReticleMaterialLayer.Inner => runtimeInnerReticuleMat,
            ReticleMaterialLayer.Outer => runtimeOuterReticuleMat,
            _ => null
        };

        if (targetMat == null)
        {
            Debug.LogWarning($"No material assigned for reticle layer {layer}");
            return;
        }

        if (!targetMat.HasProperty(propertyName))
        {
            Debug.LogWarning($"Material {targetMat} does not have property {propertyName}");
            return;
        }
        else
        {
            targetMat.SetFloat(propertyName, value);
        }
    }

    //Transitions to float property using tween using ReticleMaterialLayer to determine target material.
    public Tween SetMaterialProperty(string propertyName, float value, ReticleMaterialLayer layer, float duration)
    {
        //Set material to apply property to based on reticle layer.
        Material targetMat = layer switch
        {
            ReticleMaterialLayer.Inner => runtimeInnerReticuleMat,
            ReticleMaterialLayer.Outer => runtimeOuterReticuleMat,
            _ => null
        };

        if (targetMat == null)
        {
            Debug.LogWarning($"No material assigned for reticle layer {layer}");
            return null;
        }

        if (!targetMat.HasProperty(propertyName))
        {
            Debug.LogWarning($"Material {targetMat} does not have property {propertyName}");
            return null;
        }
        else
        {
            return targetMat.DOFloat(value, propertyName, duration);
        }
    }

    //Sets color property instantly using ReticleMaterialLayer to determine target material.
    public void SetMaterialProperty(string propertyName, Color value, ReticleMaterialLayer layer)
    {
        //Set material to apply property to based on reticle layer.
        Material targetMat = layer switch
        {
            ReticleMaterialLayer.Inner => runtimeInnerReticuleMat,
            ReticleMaterialLayer.Outer => runtimeOuterReticuleMat,
            _ => null
        };

        if (targetMat == null)
        {
            Debug.LogWarning($"No material assigned for reticle layer {layer}");
            return;
        }

        if (!targetMat.HasProperty(propertyName))
        {
            Debug.LogWarning($"Material {targetMat} does not have property {propertyName}");
            return;
        }
        else
        {
            targetMat.SetColor(propertyName, value);
        }
    }

    //Transitions to color property using tween using ReticleMaterialLayer to determine target material.
    public Tween SetMaterialProperty(string propertyName, Color value, ReticleMaterialLayer layer, float duration)
    {
        //Set material to apply property to based on reticle layer.
        Material targetMat = layer switch
        {
            ReticleMaterialLayer.Inner => runtimeInnerReticuleMat,
            ReticleMaterialLayer.Outer => runtimeOuterReticuleMat,
            _ => null
        };

        if (targetMat == null)
        {
            Debug.LogWarning($"No material assigned for reticle layer {layer}");
            return null;
        }

        if (!targetMat.HasProperty(propertyName))
        {
            Debug.LogWarning($"Material {targetMat} does not have property {propertyName}");
            return null;
        }
        else
        {
            return targetMat.DOColor(value, propertyName, duration);
        }
    }

    //Sets boolean keywords instantly using ReticleMaterialLayer to determine target material.
    public void SetMaterialProperty(string propertyName, bool enabled, ReticleMaterialLayer layer)
    {
        //Set material to apply property to based on reticle layer.
        Material targetMat = layer switch
        {
            ReticleMaterialLayer.Inner => runtimeInnerReticuleMat,
            ReticleMaterialLayer.Outer => runtimeOuterReticuleMat,
            _ => null
        };

        if (targetMat == null)
        {
            Debug.LogWarning($"No material assigned for reticle layer {layer}");
            return;
        }

        if (targetMat.shader.keywordSpace.FindKeyword(propertyName) == null)
        {
            Debug.LogWarning($"Material {targetMat} does not have keyword {propertyName}");
            return;
        }
        else
        {
            if (enabled)
                targetMat.EnableKeyword(propertyName);
            else
                targetMat.DisableKeyword(propertyName);
        }
    }
    #endregion

    #region Dynamic Property Handling
    //Returns list of dynamic properties for ReticleHitReact and ReticleCharger scripts.
    public List<MaterialPropertyData> GetDynamicProperties() => dynamicPropertiesList;

    //Resets the dynamic runtime properties to their inactive/starting states.
    public void ResetDynamicProperties()
    {
        if(dynamicPropertiesList == null || dynamicPropertiesList.Count == 0)
        {
            Debug.LogError("No current dynamic properties defined");
            return;
        }

        Debug.Log($"DynamicProperties being reset count: {dynamicPropertiesList.Count}");

        foreach (var prop in dynamicPropertiesList)
        {
            //Debug.Log($"DynamicProperty being reset: {prop.propertyName}");

            if (prop is MaterialFloatProperties)
            {
                SetMaterialProperty(prop.propertyName, 0f, prop.reticleLayer);
            }
            else if (prop is MaterialColorProperties)
            {
                SetMaterialProperty(prop.propertyName, Color.white, prop.reticleLayer);
            }
            else if (prop is MaterialKeywordProperties)
            {
                SetMaterialProperty(prop.propertyName, false, prop.reticleLayer);
            }
            else
            {
                Debug.LogWarning($"Property type not supported for {prop.propertyName}");
            }
        }
    }
    #endregion

    //Controls the visibility of the reticle UI images.
    //Can also be toggled instantly or faded in and out.
    public void ToggleReticleVisibility(bool visible, bool fade = false)
    {
        if (fade)
        {
            reticuleImageInner.DOFade(visible ? 1f : 0f, 0.25f);
            reticuleImageOuter.DOFade(visible ? 1f : 0f, 0.25f);
        }
        else
        {
            reticuleImageInner.enabled = visible;
            reticuleImageOuter.enabled = visible;
        }
    }

    #region Handle Tweens
    //Adds the tweens used to transition between values when applying full profiles.
    //All profiles use the same Easing for consistency.
    private void AddTween(Tween tween)
    {
        tween.SetEase(Ease.OutQuad);
        activeTransitionTweens.Add(tween);
    }

    //Cancel any transition tweens.
    public void CancelTweens()
    {
        foreach (var tween in activeTransitionTweens)
            tween.Kill();

        //Clear list of all the interrupted tweens.
        activeTransitionTweens.Clear();
    }
    #endregion
}
