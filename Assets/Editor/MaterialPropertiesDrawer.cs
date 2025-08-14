using UnityEditor;
using UnityEngine;

//Color coding of properties if they are set to any kind of dynamic property to
//improve workflow and user-friendliness and find them easier.
//Similarly hides endValue if the property is not dynamic.
[CustomPropertyDrawer(typeof(MaterialFloatProperties))]
public class MaterialFloatPropertiesDrawer : PropertyDrawer
{
    private Color colorRuntime = new Color32(22, 156, 64, 255);
    private Color colorHitReact = new Color32(194, 121, 33, 255);
    private Color colorChargeBar = new Color32(83, 18, 163, 255);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var dynamicTypeProp = property.FindPropertyRelative("dynamicPropertyType");

        Color originalColor = GUI.backgroundColor;
        Color backgroundColor = originalColor;

        //Set background colour based on dynamic property type.
        switch ((DynamicPropertyType)dynamicTypeProp.enumValueIndex)
        {
            case DynamicPropertyType.None:
                break;
            case DynamicPropertyType.Runtime:
                backgroundColor = colorRuntime;
                break;
            case DynamicPropertyType.HitReact:
                backgroundColor = colorHitReact;
                break;
            case DynamicPropertyType.ChargeBar:
                backgroundColor = colorChargeBar;
                break;
        }

        //Darken color for readability.
        backgroundColor = backgroundColor * 0.7f;

        //int initialIndent = EditorGUI.indentLevel;
        //EditorGUI.indentLevel = 0;

        //To account for two indents from being inside a list, of 14 each.
        Rect fullRect = new Rect(position.x - 28, position.y, position.width + 28, position.height);

        if ((DynamicPropertyType)dynamicTypeProp.enumValueIndex != DynamicPropertyType.None)
        {
            //Only draw colored rect if the property type is dynamic.
            EditorGUI.DrawRect(fullRect, backgroundColor);

            //Draw all property fields as usual.
            EditorGUI.PropertyField(position, property, label, true);

            if(property.isExpanded)
            {
                //If property is expanded, manually draw end value property field and background rect (set to HideInInspector attribute by default).
                float mainPropertyHeight = EditorGUI.GetPropertyHeight(property, label, true);

                Rect endValueRect = new Rect(position.x, position.y + mainPropertyHeight + 2, position.width, EditorGUIUtility.singleLineHeight);

                //Rect for the endValue field with slight discolortion to draw attention to it.
                EditorGUI.DrawRect(endValueRect, backgroundColor * 0.7f);

                //Draw endValue field at correct indent
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(endValueRect, property.FindPropertyRelative("endValue"));
                EditorGUI.indentLevel--;
            }
        }
        else
        {
            //EditorGUI.indentLevel = initialIndent;

            //Draw property field as usual if not dynamic.
            EditorGUI.PropertyField(position, property, label, true);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var dynamicPropertyType = property.FindPropertyRelative("dynamicPropertyType");
        bool isDynamic = dynamicPropertyType.enumValueIndex != 0;

        float height = EditorGUI.GetPropertyHeight(property, label, true);

        if (isDynamic && property.isExpanded)
        {
            height += EditorGUIUtility.singleLineHeight + 2;
        }

        return height;
    }
}


[CustomPropertyDrawer(typeof(MaterialColorProperties))]
public class MaterialColorPropertiesDrawer : PropertyDrawer
{
    private Color colorRuntime = new Color32(22, 156, 64, 255);
    private Color colorHitReact = new Color32(194, 121, 33, 255);
    private Color colorChargeBar = new Color32(83, 18, 163, 255);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var dynamicTypeProp = property.FindPropertyRelative("dynamicPropertyType");

        Color originalColor = GUI.backgroundColor;
        Color backgroundColor = originalColor;

        //Set background colour based on dynamic property type
        switch ((DynamicPropertyType)dynamicTypeProp.enumValueIndex)
        {
            case DynamicPropertyType.None:
                break;
            case DynamicPropertyType.Runtime:
                backgroundColor = colorRuntime;
                break;
            case DynamicPropertyType.HitReact:
                backgroundColor = colorHitReact;
                break;
            case DynamicPropertyType.ChargeBar:
                backgroundColor = colorChargeBar;
                break;
        }

        //Darken color for readability
        backgroundColor = backgroundColor * 0.7f;

        //int initialIndent = EditorGUI.indentLevel;
        //EditorGUI.indentLevel = 0;

        //To account for two indents from being inside a list, of 14 each
        Rect fullRect = new Rect(position.x - 28, position.y, position.width + 28, position.height);

        if ((DynamicPropertyType)dynamicTypeProp.enumValueIndex != DynamicPropertyType.None)
        {
            //Only draw colored rect if the property type is dynamic
            EditorGUI.DrawRect(fullRect, backgroundColor);

            //Draw all property fields as usual
            EditorGUI.PropertyField(position, property, label, true);

            if (property.isExpanded)
            {
                //If property is expanded, manually draw end value property field and background rect (set to HideInInspector attribute by default)
                float mainPropertyHeight = EditorGUI.GetPropertyHeight(property, label, true);

                Rect endValueRect = new Rect(position.x, position.y + mainPropertyHeight + 2, position.width, EditorGUIUtility.singleLineHeight);

                //Rect for the endValue field with slight discolortion to draw attention to it
                EditorGUI.DrawRect(endValueRect, backgroundColor * 0.7f);

                //Draw endValue field at correct indent
                EditorGUI.indentLevel++;
                EditorGUI.PropertyField(endValueRect, property.FindPropertyRelative("endValue"));
                EditorGUI.indentLevel--;
            }
        }
        else
        {
            //EditorGUI.indentLevel = initialIndent;

            //Draw property field as usual if not dynamic
            EditorGUI.PropertyField(position, property, label, true);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var dynamicPropertyType = property.FindPropertyRelative("dynamicPropertyType");
        bool isDynamic = dynamicPropertyType.enumValueIndex != 0;

        float height = EditorGUI.GetPropertyHeight(property, label, true);

        if (isDynamic && property.isExpanded)
        {
            height += EditorGUIUtility.singleLineHeight + 2;
        }

        return height;
    }
}


[CustomPropertyDrawer(typeof(MaterialKeywordProperties))]
public class MaterialKeywordPropertiesDrawer : PropertyDrawer
{
    private Color colorRuntime = new Color32(22, 156, 64, 255);
    private Color colorHitReact = new Color32(194, 121, 33, 255);
    private Color colorChargeBar = new Color32(83, 18, 163, 255);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var dynamicTypeProp = property.FindPropertyRelative("dynamicPropertyType");

        Color originalColor = GUI.backgroundColor;
        Color backgroundColor = originalColor;

        //Set background colour based on dynamic property type
        switch ((DynamicPropertyType)dynamicTypeProp.enumValueIndex)
        {
            case DynamicPropertyType.None:
                break;
            case DynamicPropertyType.Runtime:
                backgroundColor = colorRuntime;
                break;
            case DynamicPropertyType.HitReact:
                backgroundColor = colorHitReact;
                break;
            case DynamicPropertyType.ChargeBar:
                backgroundColor = colorChargeBar;
                break;
        }

        //Darken color for readability
        backgroundColor = backgroundColor * 0.7f;

        //int initialIndent = EditorGUI.indentLevel;
        //EditorGUI.indentLevel = 0;

        //To account for two indents from being inside a list, of 14 each
        Rect fullRect = new Rect(position.x - 28, position.y, position.width + 28, position.height);

        if ((DynamicPropertyType)dynamicTypeProp.enumValueIndex != DynamicPropertyType.None)
        {
            //Only draw colored rect if the property type is dynamic
            EditorGUI.DrawRect(fullRect, backgroundColor);

            //Draw all property fields as usual
            EditorGUI.PropertyField(position, property, label, true);

            //if (property.isExpanded)
            //{
            //    //If property is expanded, manually draw end value property field and background rect (set to HideInInspector attribute by default)
            //    float mainPropertyHeight = EditorGUI.GetPropertyHeight(property, label, true);

            //    Rect endValueRect = new Rect(position.x, position.y + mainPropertyHeight + 2, position.width, EditorGUIUtility.singleLineHeight);

            //    //Rect for the endValue field with slight discolortion to draw attention to it
            //    EditorGUI.DrawRect(endValueRect, backgroundColor * 0.7f);

            //    //Draw endValue field at correct indent
            //    EditorGUI.indentLevel++;
            //    EditorGUI.PropertyField(endValueRect, property.FindPropertyRelative("endValue"));
            //    EditorGUI.indentLevel--;
            //}
        }
        else
        {
            //EditorGUI.indentLevel = initialIndent;

            //Draw property field as usual if not dynamic
            EditorGUI.PropertyField(position, property, label, true);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //var dynamicPropertyType = property.FindPropertyRelative("dynamicPropertyType");
        //bool isDynamic = dynamicPropertyType.enumValueIndex != 0;

        return EditorGUI.GetPropertyHeight(property, label, true);

        //if (isDynamic && property.isExpanded)
        //{
        //    height += EditorGUIUtility.singleLineHeight + 2;
        //}

        //return height;
    }
}