using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using AcrylecSkeleton.Utilities;

/// <summary>
/// Manages and loads tiles and layouts form resources
/// </summary>
public class LevelDataManager : Singleton<LevelDataManager>
{
    private Dictionary<int, GameObject> tiles;
    /// <summary>
    /// All the loaded tiles
    /// </summary>
    public Dictionary<int, GameObject> Tiles
    {
        get
        {
            //If not loaded, load all in tile folder
            if (tiles == null)
            {
                GameObject[] temp = Resources.LoadAll<GameObject>("Tiles");

                tiles = new Dictionary<int, GameObject>();

                foreach (GameObject tile in temp)
                {
                    tiles.Add(Convert.ToInt16(tile.name), tile);
                }
            }

            return tiles;
        }
    }

    private List<Layout> layouts;
    /// <summary>
    /// All the loaded layouts
    /// </summary>
    public List<Layout> Layouts
    {
        get
        {
            //If not loaded, load all in layout folder
            if (layouts == null)
            {
                TextAsset[] temp = Resources.LoadAll<TextAsset>("Layouts");

                layouts = new List<Layout>();

                foreach (var item in temp)
                {
                    int[,] grid = CSVReader.SplitCsvGridToInt(item.text, true);

                    Tile[,] tiles = new Tile[grid.GetLength(0), grid.GetLength(1)];

                    for (int i = 0; i < grid.GetLength(0); i++)
                    {
                        for (int j = 0; j < grid.GetLength(1); j++)
                        {
                            if (grid[i, j] >= 0)
                                tiles[i, j] = new Tile(Tiles[grid[i, j]]);
                            else
                                tiles[i, j] = new Tile(null);
                        }
                    }

                    layouts.Add(new Layout(Convert.ToInt16(item.name), tiles));
                }
            }
            return layouts;
        }
    }
}
