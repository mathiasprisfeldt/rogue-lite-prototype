using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Platforms : MonoBehaviour
{

    public int NumberOfTiles { get; private set; }
    public int NumberOfPlatforms { get; private set; }
    public int NumberOfVerticals { get; private set; }
    public int NumberOfHorizontals { get; private set; }

    public void ParentToThis(Transform child, int numberOfTiles, bool isVertical)
    {
        child.SetParent(transform);
        NumberOfTiles += numberOfTiles;
        NumberOfPlatforms++;

        if (isVertical)
            NumberOfVerticals++;
        else
            NumberOfHorizontals++;
    }
}

[CustomEditor(typeof(Platforms))]
public class PlatformsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Platforms platformsScript = (Platforms)target;

        EditorGUILayout.LabelField("Platforms", platformsScript.NumberOfPlatforms.ToString());
        EditorGUILayout.LabelField("Tiles", platformsScript.NumberOfTiles.ToString());
        EditorGUILayout.LabelField("Verticals", platformsScript.NumberOfVerticals.ToString());
        EditorGUILayout.LabelField("Horizontals", platformsScript.NumberOfHorizontals.ToString());

    }
}