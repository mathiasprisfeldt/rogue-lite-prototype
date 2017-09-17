using AcrylecSkeleton.Utilities.Collections;
using UnityEditor;
using UnityEngine;

namespace AcrylecSkeleton.Editor.Utilities.Collections
{
    /// <summary>
    /// Purpose: Inspector UI for TagList.
    /// Creator: MP
    /// </summary>
    [CustomPropertyDrawer(typeof(Tags))]
    public class TagsEditor : PropertyDrawer
    {
        private static readonly string[] PossibleTags = UnityEditorInternal.InternalEditorUtility.tags;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty tags = property.FindPropertyRelative("TagList");
            
            if (tags != null)
                tags.intValue = EditorGUI.MaskField(position, property.displayName, tags.intValue, PossibleTags);
        }
    }
}