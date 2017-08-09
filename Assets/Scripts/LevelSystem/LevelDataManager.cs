using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using AcrylecSkeleton.Utilities;

public class LevelDataManager : Singleton<LevelDataManager>
{
    private Dictionary<int, GameObject> tiles;
    public Dictionary<int, GameObject> Tiles
    {
        get
        {
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
    public List<Layout> Layouts
    {
        get
        {
            if (layouts == null)
            {
                TextAsset[] temp = Resources.LoadAll<TextAsset>("Layouts");

                layouts = new List<Layout>();

                foreach (var item in temp)
                {
                    var grid = CSVReader.SplitCsvGridTiled(item.text);


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
