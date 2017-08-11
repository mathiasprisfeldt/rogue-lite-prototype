using System;
using UnityEngine;
using UnityEditor;

namespace AcrylecSkeleton.UI.Editor
{
    [CustomPropertyDrawer (typeof (MinMaxSliderAttribute))]
    class MinMaxSliderDrawer : PropertyDrawer {
	
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2) {
                Vector2 range = property.vector2Value;
                float min = Mathf.Round(range.x);
                float max = Mathf.Round(range.y);

                MinMaxSliderAttribute att = attribute as MinMaxSliderAttribute;


                EditorGUI.BeginChangeCheck ();

                EditorGUI.MinMaxSlider (position, label, ref min, ref max, att.min, att.max);

                if (EditorGUI.EndChangeCheck ()) {
                    range.x = min;
                    range.y = max;
                    property.vector2Value = range;
                }


                EditorGUI.LabelField(new Rect(position.x, position.y + 15, position.width, position.height), "Min: " + min);
                EditorGUI.LabelField(new Rect(position.x, position.y + 30, position.width, position.height), "Max: " + max);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 50.0f;
        }
    }
}