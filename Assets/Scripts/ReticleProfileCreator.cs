using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

//Used by ReticleProfileCreator to know which material represents which layer.
[System.Serializable]
public class ReticleMaterialInfo
{
    public Material material;
    public ReticleMaterialLayer layer;
}

//Automatically create reticle profiles with the click of a button.
//This initially required manual copy and pasting of property values in my Honours Project.

//HOW TO USE: Set the inner and outer reticle material properties to create a desired reticle design.
//Then, drag and drop those materials in the inspector, assign the corresponding material layer,
//select the reticle mode this profile should be, and press "Create profile".
//In the profile, set whether this reticle mode supports hit react animations and the charge progress bar.

//TODO: Add undo functionality, ability to keep certain values and modes instead of clearing profile each time
public class ReticleProfileCreator : MonoBehaviour
{
    [Header("Profile Reference")]
    public ReticleProfileSO reticleProfile;

    [Header("Profile Configuration")]
    public List<ReticleMaterialInfo> materialsToRecord;
    public ReticleMode targetMode;
    public bool provideLogReportOnCreation;

    //Scans the assigned materials and extracts all relevant shader properties and their values into the ScriptableObject profile.
    public void GetMaterialPropertyData()
    {
        //Ensure we have the required setup.
        if (reticleProfile == null)
        {
            Debug.LogError("ReticleProfileSO not assigned in ReticleProfileCreator");
            return;
        }

        if (materialsToRecord == null || materialsToRecord.Count == 0)
        {
            Debug.LogError("No materials assigned for reticle profile creation");
            return;
        }

        ReticleProfileData profileData = reticleProfile.GetProfile(targetMode);

        //Check if entry with target reticle mode already exists.
        if (profileData == null)
        {
            //Create new profile with empty lists.
            profileData = new ReticleProfileData
            {
                reticleMode = targetMode,
                innerReticleFloatProperties = new List<MaterialFloatProperties>(),
                innerReticleColorProperties = new List<MaterialColorProperties>(),
                innerReticleKeywordProperties = new List<MaterialKeywordProperties>(),
                outerReticleFloatProperties = new List<MaterialFloatProperties>(),
                outerReticleColorProperties = new List<MaterialColorProperties>(),
                outerReticleKeywordProperties = new List<MaterialKeywordProperties>()
            };

            //Insert new profile at beginning for easy access.
            reticleProfile.profiles.Insert(0, profileData);
        }
        else
        {
            //If profile exists, clear that profile's property data to make way for new data.
            //This does not clear profile properties not included in the property lists, such as hit reaction and charge bar settings.
            profileData.innerReticleFloatProperties.Clear();
            profileData.innerReticleColorProperties.Clear();
            profileData.innerReticleKeywordProperties.Clear();
            profileData.outerReticleFloatProperties.Clear();
            profileData.outerReticleColorProperties.Clear();
            profileData.outerReticleKeywordProperties.Clear();
        }

        //Track how many properties were found or skipped in the creation of this profile.
        int propertiesFoundTotal = 0;
        int propertiesSkipped = 0;

        //Process each assigned material.
        foreach (var matInfo in materialsToRecord)
        {
            Material mat = matInfo.material;
            ReticleMaterialLayer layer = matInfo.layer;

            //If unassigned, go to next.
            if (mat == null)
                continue;
            
            Shader shader = mat.shader;

            int propertyCount = shader.GetPropertyCount();

            //Scan all shader properties.
            for (int i = 0; i < propertyCount; i++)
            {
                //Assign name and property type.
                string name = shader.GetPropertyName(i);
                ShaderPropertyType propertyType = shader.GetPropertyType(i);

                //Skip if the property is flagged to be hidden from inspector.
                if (shader.GetPropertyFlags(i) != 0)
                {
                    propertiesSkipped++;
                    continue;
                }

                //Handle different property types.
                switch (propertyType)
                {
                    case ShaderPropertyType.Float:
                    case ShaderPropertyType.Range:
                        //If the property name is all caps, don't add since likely a keyword boolean.
                        //Checks if none are lowercase instead of if all are uppercase since "_" are in property names.
                        if (!name.Where(char.IsLower).Any())
                        {
                            //Not counting this to propertiesSkipped since it will likely be added in keyword boolean section.
                            continue;
                        }

                        var floatData = new MaterialFloatProperties
                        {
                            propertyName = name,
                            reticleLayer = layer,
                            value = mat.GetFloat(name)
                        };

                        //Add to corresponding layer's list.
                        (layer == ReticleMaterialLayer.Inner
                            ? profileData.innerReticleFloatProperties
                            : profileData.outerReticleFloatProperties).Add(floatData);

                        propertiesFoundTotal++;
                        break;

                    case ShaderPropertyType.Color:
                        var colorData = new MaterialColorProperties
                        {
                            propertyName = name,
                            reticleLayer = layer,
                            value = mat.GetColor(name)
                        };

                        //Add to corresponding layer's list.
                        (layer == ReticleMaterialLayer.Inner
                            ? profileData.innerReticleColorProperties
                            : profileData.outerReticleColorProperties).Add(colorData);

                        propertiesFoundTotal++;
                        break;
                }
            }

            //Shader keywords are handled separately.
            foreach (LocalKeyword localKeyword in mat.shader.keywordSpace.keywords)
            {
                //Skip invalid keywords.
                if (!localKeyword.isValid)
                    continue;

                //Filter the built in keywords.
                if (localKeyword.type != ShaderKeywordType.UserDefined || 
                    localKeyword.type == ShaderKeywordType.BuiltinDefault || 
                    !localKeyword.name.StartsWith("_"))
                    continue;

                var keywordData = new MaterialKeywordProperties
                {
                    propertyName = localKeyword.name,
                    reticleLayer = layer,
                    keywordEnabled = mat.IsKeywordEnabled(localKeyword.name)
                };

                //Add to corresponding layer's list.
                (layer == ReticleMaterialLayer.Inner
                    ? profileData.innerReticleKeywordProperties
                    : profileData.outerReticleKeywordProperties).Add(keywordData);

                propertiesFoundTotal++;
            }
        }

        //Provide report on generated profile for debugging.
        if (provideLogReportOnCreation)
        {
            Debug.Log($"Profile creation complete for reticle mode: {targetMode}:");
            Debug.Log($"- Total properties found: {propertiesFoundTotal}");
            Debug.Log($"- Properties skipped: {propertiesSkipped}");
            Debug.Log($"- Inner float properties: {profileData.innerReticleFloatProperties.Count}");
            Debug.Log($"- Outer float properties: {profileData.outerReticleFloatProperties.Count}");
            Debug.Log($"- Inner color properties: {profileData.innerReticleColorProperties.Count}");
            Debug.Log($"- Outer color properties: {profileData.outerReticleColorProperties.Count}");
            Debug.Log($"- Inner keyword properties: {profileData.innerReticleKeywordProperties.Count}");
            Debug.Log($"- Outer keyword properties: {profileData.outerReticleKeywordProperties.Count}");
        }
    }
}

#if UNITY_EDITOR
//Custom inspector button for user-friendly creation of profiles.
[CustomEditor(typeof(ReticleProfileCreator))]
public class ReticleProfileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var instance = (ReticleProfileCreator)target;

        EditorGUILayout.Space();

        //Profile creation button.
        if (GUILayout.Button("Create profile"))
        {
            instance.GetMaterialPropertyData();

            //Set the ScriptableObject to dirty so Unity saves the changes.
            EditorUtility.SetDirty(instance.reticleProfile);
        }
    }
}
#endif