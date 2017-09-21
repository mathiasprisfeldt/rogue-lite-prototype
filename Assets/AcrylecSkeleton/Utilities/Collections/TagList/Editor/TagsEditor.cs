using System;
using AcrylecSkeleton.Utilities;
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
            SerializedProperty tagsUnsplitted = property.FindPropertyRelative("TagListUnSplitted");
            SerializedProperty tagMask = property.FindPropertyRelative("TagMask");

            if (tagsUnsplitted != null && tagMask != null)
            {
                tagsUnsplitted.stringValue = String.Empty;
                tagMask.intValue = EditorGUI.MaskField(position, property.displayName, tagMask.intValue, PossibleTags);

                for (var i = 0; i < PossibleTags.Length; i++)
                {
                    if (tagMask.intValue.Contains(i))
                        tagsUnsplitted.stringValue += PossibleTags[i] + "@";
                }
            }
        }
    }
}