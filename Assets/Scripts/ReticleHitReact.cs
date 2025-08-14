using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Handles the animated visual feedback when successfully hitting a target.
//Animates properties like visibility or element sizes using tweens.
//Materials are created from shader that was built with these animations in mind.
public class ReticleHitReact : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private ReticleController reticleController;
    [SerializeField] private ReticleMaterialManager reticleMaterialManager;

    //Determines whether the current reticle mode supports hit react animations.
    private bool canHitReact;

    //Whether to reverse and play tween backwards at the end or not.
    private bool flipTween;

    //Length of hit reaction animations.
    private float currentHitReactDuration = 0f;

    //Cached list of dynamic properties that are tagged for hit reaction use.
    private List<MaterialPropertyData> hitReactDynamicProperties = new List<MaterialPropertyData>();

    //List of tweens that are animating hit reactions to cancel them in case of overlap.
    private List<Tween> activeHitReactTweens = new List<Tween>();

    //Subscribe to ReticleModeChanged event to update canHitReact bool.
    private void OnEnable()
    {
        if(reticleController != null)
            reticleController.onReticleModeChanged += HandleOnReticleModeChanged;
    }

    //Unsubscribe from events to prevent memory leaks.
    private void OnDisable()
    {
        reticleController.onReticleModeChanged -= HandleOnReticleModeChanged;
    }

    private void Awake()
    {
        if (reticleMaterialManager == null)
            reticleMaterialManager = GetComponent<ReticleMaterialManager>();

        if (reticleController == null)
            reticleController = GetComponent<ReticleController>();
    }

    //Sets canHitReact bool to the new reticle profile when reticle mode is changed.
    private void HandleOnReticleModeChanged(ReticleProfileData newProfile)
    {
        canHitReact = newProfile.canHitReact;

        //Early exit if this reticle profile does not support hit react animations.
        if (!canHitReact)
        {
            hitReactDynamicProperties.Clear();
            return;
        }

        ResetDynamicPropertyList();

        //Cache hit reaction settings for this profile.
        currentHitReactDuration = newProfile.hitReactDuration;
        flipTween = newProfile.resetOnComplete;
    }

    private void ResetDynamicPropertyList()
    {
        //Repopulate list of dynamic properties if they are tagged for use with hit reactions.
        hitReactDynamicProperties.Clear();
        hitReactDynamicProperties = reticleMaterialManager.GetDynamicProperties().Where(p => p.dynamicPropertyType == DynamicPropertyType.HitReact).ToList();
    }

    //Triggers the hit reaction animation.
    public void TriggerHitReact()
    {
        //Early exit if current reticle mode does not support hit react animations.
        if (!canHitReact)
            return;

        if (reticleMaterialManager == null)
        {
            Debug.LogError("ReticleMaterialManager reference is null");
            return;
        }

        if (hitReactDynamicProperties == null || hitReactDynamicProperties.Count == 0)
        {
            Debug.LogError("No current dynamic properties defined");
            return;
        }

        CancelTweens();

        foreach(var prop in hitReactDynamicProperties)
        {
            object startVal = prop.GetValue(true);
            object endVal = prop.GetValue(false);

            if (startVal is float startFloat && endVal is float endFloat)
            {
                reticleMaterialManager.SetMaterialProperty(prop.propertyName, startFloat, prop.reticleLayer);
                AddTween(reticleMaterialManager.SetMaterialProperty(prop.propertyName, endFloat, prop.reticleLayer, currentHitReactDuration), flipTween);
            }
            else if (startVal is Color startColor && endVal is Color endColor)
            {
                reticleMaterialManager.SetMaterialProperty(prop.propertyName, startColor, prop.reticleLayer);
                AddTween(reticleMaterialManager.SetMaterialProperty(prop.propertyName, endColor, prop.reticleLayer, currentHitReactDuration), flipTween);  
            }
            else if (startVal is bool startBool && endVal is bool endBool)
            {
                //No tween for bool keywords, so just set instantly to bool
                reticleMaterialManager.SetMaterialProperty(prop.propertyName, startBool, prop.reticleLayer);
                reticleMaterialManager.SetMaterialProperty(prop.propertyName, startBool, prop.reticleLayer);
            }
            else
            {
                Debug.LogWarning($"Property type not supported for {prop.propertyName}");
            }
        }

        //Could extend with further animated properties and color transitions
    }

    #region Handle Tweens
    //Adds the tweens used to animate hit reactions.
    //All profiles use the same Easing for consistency.
    private void AddTween(Tween tween, bool reset = false)
    {
        //Check if this tween should revert back to starting values once complete.
        if (reset)
        {
            tween.SetEase(Ease.OutQuad).SetAutoKill(false).onComplete = () => { tween.SmoothRewind(); };
        }
        else
        {
            tween.SetEase(Ease.OutQuad);
        }

        activeHitReactTweens.Add(tween);
    }

    //Cancel any hit react tweens.
    public void CancelTweens()
    {
        foreach (var tween in activeHitReactTweens)
            tween.Kill();

        //Clear list of all the interrupted tweens.
        activeHitReactTweens.Clear();
    }
    #endregion
}
