using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public Layout[,] Layouts { get; set; }
    public int ID { get; set; }
    public Texture2D BG { get; set; }

    public Level(Layout[,] layouts, int id)
    {
        Layouts = layouts;
        ID = id;
    }

    public Level(int[,] l, int id)
    {
        Layouts = new Layout[l.GetLength(0), l.GetLength(1)];

        for (int i = 0; i < l.GetLength(0); i++)
        {
            for (int j = 0; j < l.GetLength(0); j++)
            {
                Layouts[i, j] = LevelDataManager.Instance.Layouts.Find(x => x.ID == l[i, j]);
            }
        }

        ID = id;
    }

    public void Spawn(Transform transform)
    {
        //Find the size of one tile, make better in future
        var test = LevelDataManager.Instance.Tiles[0].transform;
        var tileHeight = test.localScale.y;
        var tileWidth = test.localScale.x;

        for (int i = 0; i < Layouts.GetLength(0); i++)
        {
            for (int j = 0; j < Layouts.GetLength(1); j++)
            {
                // Each layout
                for (int x = 0; x < Layouts[i, j].Tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < Layouts[i, j].Tiles.GetLength(1); y++)
                    {
                        // Each tile
                        Tile t = Layouts[i, j].Tiles[x, y];
                        if (t.GO != null)
                            GameObject.Instantiate(t.GO,
                                transform.position + new Vector3(
                                    (i * Layouts[i, j].Tiles.GetLength(0)) * tileWidth + x * tileWidth,
                                    ((j * Layouts[i, j].Tiles.GetLength(1)) * tileHeight + y * tileHeight) * -1),
                                Quaternion.identity,
                                transform);
                    }
                }
            }
        }
    }
}
