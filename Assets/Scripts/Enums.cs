//Defines the reticle states for different weapons/combat modes.
public enum ReticleMode { A, B, C }

//Identifies whether the data applies to the inner or outer material.
public enum ReticleMaterialLayer { Inner, Outer }

//To set for which circumstance a property should later be ignored when applying profile to material.
public enum DynamicPropertyType { None, Runtime, HitReact, ChargeBar }