using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class containing a tile
/// </summary>
public class Tile
{
    //Prefab
    public GameObject GO { get; set; }

    public Tile(GameObject go)
    {
        GO = go;
    }
}
