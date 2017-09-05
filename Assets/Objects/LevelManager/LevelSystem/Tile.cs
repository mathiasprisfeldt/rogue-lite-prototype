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
    public bool PhysicalBlock { get; set; }
    public bool IsTrap { get; set; }

    public Tile(GameObject go, int t) : this()
    {
        Prefab = go;
        GoInstance = null;
        Type = t;
        PhysicalBlock = false;
    }

    public Tile(int t, GameObject go, bool phys, bool isTrap) : this()
    {
        GoInstance = go;
        Prefab = null;
        Type = t;
        PhysicalBlock = phys;
        IsTrap = isTrap;
    }
}
