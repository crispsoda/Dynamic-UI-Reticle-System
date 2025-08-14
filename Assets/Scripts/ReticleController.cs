using System;
using UnityEngine;

//The main script for external systems to interact with while coordinating any subsystems.
//Sets the reticle mode.
[RequireComponent(typeof(ReticleCharger))]
[RequireComponent(typeof(ReticleHitReact))]
[RequireComponent(typeof(ReticleMaterialManager))]
public class ReticleController : MonoBehaviour
{
    [Header("Profile Reference")]
    [SerializeField] private ReticleProfileSO reticuleProfileSO;

    [Header("Script References")]
    [SerializeField] private ReticleCharger reticleCharger;
    [SerializeField] private ReticleHitReact reticleHitReact;
    [SerializeField] private ReticleMaterialManager reticleMaterialManager;

    //Current reticle modes and associated profile data.
    public ReticleMode currentMode { get; private set; }
    public ReticleProfileData currentProfile { get; private set; }

    //Event to signify changes to reticle mode.
    public event Action<ReticleProfileData> onReticleModeChanged;

    //To differentiate between initial setup and subsequent mode changes.
    //Solves issues caused by safety checks.
    private bool isInitialized = false;

    #region Initialization
    private void Start()
    {
        if(reticleMaterialManager ==  null)
            reticleMaterialManager = GetComponent<ReticleMaterialManager>();

        if(reticleCharger == null)
            reticleCharger = GetComponent<ReticleCharger>();

        if(reticleHitReact == null)
            reticleHitReact = GetComponent<ReticleHitReact>();

        //MaterialManager has to initialize first, as it creates the runtime material instances.
        reticleMaterialManager.Initialize();
        //reticleMaterialManager.ResetDynamicProperties();
        InitializeReticle();
    }

    private void InitializeReticle()
    {
        //Set reticle mode to Mode A instantly (no tweening).
        ChangeReticuleMode(ReticleMode.A, false);
        isInitialized = true;
    }
    #endregion

    //Changes the current reticle mode and applies the corresponding profile's properties to the runtime reticle materials.
    public void ChangeReticuleMode(ReticleMode newMode, bool tween = true)
    {
        //Early exit if current mode is already the requested mode.
        if (newMode == currentMode && isInitialized)
            return;

        var targetProfile = reticuleProfileSO.GetProfile(newMode);

        //Ensure the requested profile exists and is different from the current profile.
        if (targetProfile == null || targetProfile == currentProfile && isInitialized)
            return;

        //Cancel any current profile transitions.
        reticleMaterialManager.CancelTweens();

        //Either instantly apply or transition to the new reticle mode profile's property values.
        reticleMaterialManager.ApplyProfile(targetProfile, tween);

        //Update controller state after successful profile application.
        currentMode = newMode;
        currentProfile = targetProfile;

        //Notify other systems the reticle mode has been changed.
        onReticleModeChanged?.Invoke(currentProfile);
    }

    //Triggers hit react animation.
    public void TriggerHitReact()
    {
        if (reticleHitReact != null)
        {
            reticleHitReact.TriggerHitReact();
        }
        else
        {
            Debug.LogError("Hit react triggered but ReticleHitReact reference is null");
        }
    }

    //Sets reticle's visibility by either instantly appearing/disappearing or fading in/out.
    public void ToggleReticleVisibility(bool visible, bool fade = false)
    {
        reticleMaterialManager.ToggleReticleVisibility(visible, fade);
    }

    //Start charging the reticle charge progress bar (if current reticle mode supports charge bar).
    public void ChargeReticle()
    {
        if(currentProfile.canCharge)
            reticleCharger.ChargeReticle();
    }

    //Stop charging the reticle progress bar (does not reset charge progress).
    //Can start charging again by calling ChargeReticle().
    public void StopChargingReticle()
    {
        if (currentProfile.canCharge)
            reticleCharger.StopChargingReticle();
    }

    //Resets the charge progress to 0 and sets chargeComplete to false.
    public void ResetReticleCharge()
    {
        if (currentProfile.canCharge)
            reticleCharger.ResetChargeProgress();
    }
}