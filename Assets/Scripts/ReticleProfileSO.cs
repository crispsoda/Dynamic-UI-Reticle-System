using System.Collections.Generic;
using UnityEngine;

//ScriptableObject to manage multiple reticle profiles.
//Allows animation of shader properties, as well as more specific control over values to change and smooth transitions compared to creating material instances for each state.
[CreateAssetMenu(fileName = "ReticleProfile", menuName = "ScriptableObjects/Reticle Profile")]
public class ReticleProfileSO : ScriptableObject
{
    public List<ReticleProfileData> profiles = new List<ReticleProfileData>();

    //Fetch a specific profile based on its mode enum as identifier.
    public ReticleProfileData GetProfile(ReticleMode mode)
    {
        if(profiles == null || profiles.Count == 0)
        {
            Debug.LogError("ReticleProfile List is null or empty");
            return null;
        }

        var result = profiles.Find(x => x.reticleMode == mode);

        if(result == null)
        {
            Debug.LogError($"No reticle profile found for mode: {mode}");
        }

        return result;
    }
}