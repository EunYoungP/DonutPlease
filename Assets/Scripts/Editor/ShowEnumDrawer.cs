using UnityEditor;
using UnityEngine;
using DonutEditor;

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ShowEnumAttribute))]
public class ShowEnumDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ShowEnumAttribute showIf = attribute as ShowEnumAttribute;
        string enumPath = property.propertyPath.Replace(property.name, showIf.enumFieldName);
        SerializedProperty enumProp = property.serializedObject.FindProperty(enumPath);

        if (enumProp != null && enumProp.propertyType == SerializedPropertyType.Enum)
        {
            if (enumProp.enumNames[enumProp.enumValueIndex] == showIf.targetEnumValue.ToString())
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ShowEnumAttribute showIf = attribute as ShowEnumAttribute;

        string enumPath = property.propertyPath.Replace(property.name, showIf.enumFieldName);
        SerializedProperty enumProp = property.serializedObject.FindProperty(enumPath);

        if (enumProp != null && enumProp.propertyType == SerializedPropertyType.Enum)
        {
            if (enumProp.enumNames[enumProp.enumValueIndex] == showIf.targetEnumValue.ToString())
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }
        }

        return 0f; // 안 보이게 함
    }
}
#endif