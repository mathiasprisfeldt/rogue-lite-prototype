using System;
using AcrylecSkeleton.Utilities;
using UnityEditor;

[CustomEditor(typeof(Timer))]
public class TimerEditor : Editor {
    public override void OnInspectorGUI()
    {
        Timer timer = (Timer) target;

        EditorGUILayout.LabelField(String.Format("Is Running: {0}", timer.IsRunning));
        EditorGUILayout.LabelField(String.Format("Time: {0}", (timer.Duration - timer.Clock).ToFormattedString()));
        EditorGUILayout.Space();
        
        EditorUtility.SetDirty(target);
        
        base.OnInspectorGUI();
    }
}
