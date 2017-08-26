using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for containing a layout
/// </summary>
public struct Layout
{
    /// <summary>
    /// ID used for linking numbers in files to this layout
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// array of tiles desciping the layout
    /// </summary>
    public Tile[,] Tiles { get; set; }

    public Layout(int id, Tile[,] tiles)
    {
        ID = id;
        Tiles = tiles;
    }
}
