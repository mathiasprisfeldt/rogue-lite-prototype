using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing a tile
/// </summary>
public struct Tile
{
    //Prefab
    public GameObject Prefab { get; set; }
    public GameObject GoInstance { get; set; }
    public int Type { get; set; }

    public Tile(GameObject go, int t)
    {
        Prefab = go;
        GoInstance = null;
        Type = t;
    }

    public Tile(int t, GameObject go)
    {
        GoInstance = go;
        Prefab = null;
        Type = t;
    }
}
