using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layout
{
    public int ID { get; set; }
    public Tile[,] Tiles { get; set; }

    public Layout(int id, Tile[,] tiles)
    {
        ID = id;
        Tiles = tiles;
    }
}
