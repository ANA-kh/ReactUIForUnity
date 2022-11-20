using System;
using UnityEditor;
using UnityEngine;

namespace ReactUI.TestReactUI
{
    [CustomPropertyDrawer(typeof(RangeAttribute))]
    public class TestPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {

            EditorGUI.BeginProperty(position, label, property);
            // First get the attribute since it contains the range for the slider
            RangeAttribute range = attribute as RangeAttribute;

            // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
            if (property.propertyType == SerializedPropertyType.Float)
                EditorGUI.Slider(position, property, range.Min, range.Max, GUIContent.none);
            else if (property.propertyType == SerializedPropertyType.Integer)
                EditorGUI.IntSlider(position, property, Convert.ToInt32(range.Min), Convert.ToInt32(range.Max), label);
            else
                EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
            EditorGUI.EndProperty();
        }
    }

    public class RangeAttribute  : PropertyAttribute
    {
        public float Min;
        public float Max;

        public RangeAttribute(float min, float max)
        {
            this.Min = min;
            this.Max = max;
        }
    }
}