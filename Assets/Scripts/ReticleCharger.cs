using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Handles the charge progress bar for reticle modes that support it.
//Provides visual feedback to inform the player how far along they are in the charge attack.
//NOTE: This script contains the charging logic for the purposes of this demo. Real applications should only use this for visuals and handle the logic with the player/weapon systems
public class ReticleCharger : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] private ReticleController reticleController;
    [SerializeField] private ReticleMaterialManager reticleMaterialManager;

    //Whether charging is currently active (can be paused without losing progress).
    public bool isCharging { get; private set; } = false;
    //Whether charging is successful and complete.
    public bool chargeComplete { get; private set; } = false;

    //Determines whether the current reticle mode supports charge progress bar.
    private bool canCharge = false;
    //How long it takes to fully charge the progress bar.
    private float fullChargeTime = 1f;
    //The current charge progress for lerps.
    private float chargeProgress = 0f;

    //Events at various stages of the charging process.
    public event Action OnChargeStart;
    public event Action OnChargeStop;
    public event Action OnChargeComplete;

    //Subscribe to ReticleModeChanged event to update canCharge bool.
    private void OnEnable()
    {
        if(reticleController != null)
            reticleController.onReticleModeChanged += HandleReticleModeChanged;
    }

    //Unsubscribe from events to prevent memory leaks.
    private void OnDisable()
    {
        reticleController.onReticleModeChanged -= HandleReticleModeChanged;
    }

    private void Awake()
    {
        if (reticleMaterialManager == null)
            reticleMaterialManager = GetComponent<ReticleMaterialManager>();

        if (reticleController == null)
            reticleController = GetComponent<ReticleController>();
    }

    //Handles the charging logic over time.
    //Handled in this script for the purposes of the demo.
    //TODO: Use curves for non-linear charge progression for game feel
    private void Update()
    {
        //Early exit if current reticle mode can't charge.
        if (!canCharge)
            return;

        if (reticleMaterialManager == null)
        {
            Debug.LogError("ReticleMaterialManager reference is null");
            return;
        }

        //Only process charging when actively charging and not yet complete.
        if (isCharging && !chargeComplete)
        {
            //Linear charge rate using scaled time.
            chargeProgress += Time.deltaTime / fullChargeTime;

            //Charge progress is limited from 0-1, as 1 would be complete.
            chargeProgress = Mathf.Clamp01(chargeProgress);

            //If at 99% of charge completion, set the keyword boolean to signify completed charge to true.
            //Fixes shader issue where value of 1 does not complete the full circle. The shader instead instantly sets the value to 2 if the keyword boolean is true.
            //Done at 0.99 in case precision problems prevent chargeProgress from reaching 1.
            if (chargeProgress >= 0.99f && !chargeComplete)
            {
                reticleMaterialManager.SetMaterialProperty("_CHARGECOMPLETE", true, ReticleMaterialLayer.Outer);

                //Notify completion of charging process.
                chargeComplete = true;
                OnChargeComplete?.Invoke();
            }

            //Update charge progress material property every frame while charging and not yet complete.
            reticleMaterialManager.SetMaterialProperty("_ChargeBarProgress", chargeProgress, ReticleMaterialLayer.Outer);
        }
    }

    //Starts the charging process.
    public void ChargeReticle()
    {
        //Early exit if current reticle mode does not support charge progress bar.
        if (!canCharge)
            return;

        //Only invoke start event if not already charging.
        if (!isCharging)
        {
            isCharging = true;
            OnChargeStart?.Invoke();
        }
    }

    //To momentarily stop charging without resetting progress.
    //Progress can be resumed by calling ChargeReticule() again.
    public void StopChargingReticle()
    {
        //Only invoke stop event if actually charging.
        if(isCharging)
        {
            isCharging = false;
            OnChargeStop?.Invoke();
        }
    }

    //Resets any charge progress and states to initial values.
    public void ResetChargeProgress()
    {
        chargeComplete = false;
        chargeProgress = 0f;

        if (reticleMaterialManager == null)
        {
            Debug.LogError("ReticleMaterialManager reference is null");
            return;
        }

        //Update UI visuals to match reset.
        reticleMaterialManager.SetMaterialProperty("_CHARGECOMPLETE", false, ReticleMaterialLayer.Outer);
        reticleMaterialManager.SetMaterialProperty("_ChargeBarProgress", 0f, ReticleMaterialLayer.Outer);
    }

    //Updates canCharge and fullChargeTime to match new reticle mode when changed.
    private void HandleReticleModeChanged(ReticleProfileData profileData)
    {
        if (reticleController == null)
        {
            Debug.LogError("ReticleController is null during profile change");
            return;
        }

        canCharge = profileData.canCharge;

        //Early exit if reticle mode does not support charge bar.
        if(!canCharge) 
            return;

        fullChargeTime = profileData.fullChargeTime;


        //Always reset charge progress when changing reticle modes.
        ResetChargeProgress();

        //Automatically start charging if the current reticle mode supports chargable progress bar.
        //This is done here for the purposes of this demo. Probably not useful for actual applications.
        ChargeReticle();
    }
}