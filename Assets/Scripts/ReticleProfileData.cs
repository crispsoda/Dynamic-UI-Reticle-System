using System.Collections.Generic;
using UnityEngine;

//Profile for each reticle mode, containing all user-set material properties
//and behaviours for both inner and outer reticle materials.
[System.Serializable]
public class ReticleProfileData
{
    //To identify which profile to apply based on reticle mode.
    public ReticleMode reticleMode;

    [Header("Hit Reaction Settings")]
    //To set whether this profile has hit react animations.
    public bool canHitReact;

    //How long a hit react animation should play for.
    //Should usually belong to player or weapon related data, but for the purposes of this demo is added here.
    public float hitReactDuration = 0.25f;

    //Determines whether the properties should reset to their initial values after the hit reaction is complete.
    //Done through DOTween Flip in ReticleHitReact script.
    public bool resetOnComplete;
    
    [Header("Hit Reaction Settings")]
    //To set whether this profile has a chargable progress bar.
    public bool canCharge;

    //To determine how long the chargable progress bar should take to fully charge.
    //Should usually belong to player or weapon related data, but for the purposes of this demo is added here.
    public float fullChargeTime = 2f;

    ////DELETE
    ////The properties that should be ignored when applying a profile
    ////Needs to be set manually as it varies from case by case, depending on which elements are to be animated for a reticle
    //[Header("Dynamic Properties (Manual Set)")]
    //public List<DynamicMaterialProperties> dynamicFloatProperties = new List<DynamicMaterialProperties>();

    //Properties are split into separate lists for property types and reticle material layers for inspector readability.
    [Header("Inner Reticle Properties")]
    public List<MaterialFloatProperties> innerReticleFloatProperties = new List<MaterialFloatProperties>();
    public List<MaterialColorProperties> innerReticleColorProperties = new List<MaterialColorProperties>();
    public List<MaterialKeywordProperties> innerReticleKeywordProperties = new List<MaterialKeywordProperties>();

    [Header("Outer Reticle Properties")]
    public List<MaterialFloatProperties> outerReticleFloatProperties = new List<MaterialFloatProperties>();
    public List<MaterialColorProperties> outerReticleColorProperties = new List<MaterialColorProperties>();
    public List<MaterialKeywordProperties> outerReticleKeywordProperties = new List<MaterialKeywordProperties>();
}