﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for containing a level
/// </summary>
public class Level
{
    /// <summary>
    /// The layouts in the level
    /// </summary>
    public Layout[,] Layouts { get; set; }

    /// <summary>
    /// ID
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Background image, not used currently
    /// </summary>
    public Texture2D BG { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="layouts">Layouts array</param>
    /// <param name="id">id</param>
    public Level(Layout[,] layouts, int id)
    {
        Layouts = layouts;
        ID = id;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="l">Layout array described in an int array with ids for layouts</param>
    /// <param name="id">id</param>
    public Level(int[,] l, int id)
    {
        //Create new array and fill with layouts corresponding to the ids given
        Layouts = new Layout[l.GetLength(0), l.GetLength(1)];
        for (int i = 0; i < l.GetLength(0); i++)
        {
            for (int j = 0; j < l.GetLength(1); j++)
            {
                //TODO: make error handling for index out of range
                Layouts[i, j] = LevelDataManager.Instance.Layouts.Find(x => x.ID == l[i, j]);
            }
        }

        ID = id;
    }

    /// <summary>
    /// Spawns the current layouts in this level according to a given transform
    /// </summary>
    /// <param name="transform">transform to spawn under</param>
    public void Spawn(Transform transform)
    {
        //TODO: Find the size of one tile, make better in future
        var test = LevelDataManager.Instance.Tiles[0].GetComponent<SpriteRenderer>();
        var tileHeight = test.bounds.size.y;
        var tileWidth = test.bounds.size.x;

        bool top = false;
        bool bottom = false;
        bool left = false;
        bool right = false;

        List<Vector2> borderQueue = new List<Vector2>();

        //Go through each tile and spawn, if not null
        for (int i = 0; i < Layouts.GetLength(0); i++)
        {
            for (int j = 0; j < Layouts.GetLength(1); j++)
            {
                // Each layout
                var parent = new GameObject("Layout " + Layouts[i, j].ID);
                parent.transform.SetParent(transform);

                for (int x = 0; x < Layouts[i, j].Tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < Layouts[i, j].Tiles.GetLength(1); y++)
                    {
                        var tilePos =
                            new Vector3((i * Layouts[i, j].Tiles.GetLength(0)) * tileWidth + x * tileWidth,
                            ((j * Layouts[i, j].Tiles.GetLength(1)) * tileHeight + y * tileHeight) * -1);

                        // Each tile
                        Tile t = Layouts[i, j].Tiles[x, y];
                        if (t.Prefab != null)
                        {
                            GameObject go = GameObject.Instantiate(
                                t.Prefab,
                                transform.position + tilePos,
                                Quaternion.identity,
                                parent.transform);

                            t.GoInstance = go;
                            go.name = i.ToString() + j.ToString() + x.ToString() + y.ToString();
                        }

                        left = x == 0 && i == 0;
                        top = y == 0 && j == 0;
                        right = x == Layouts[i, j].Tiles.GetLength(0) - 1 && i == Layouts.GetLength(0) - 1;
                        bottom = y == Layouts[i, j].Tiles.GetLength(1) - 1 && j == Layouts.GetLength(1) - 1;

                        //Place border blocks
                        if (left)
                            borderQueue.Add(new Vector2(tilePos.x - tileWidth, tilePos.y));

                        if (top)
                            borderQueue.Add(new Vector2(tilePos.x, tilePos.y + tileHeight));

                        if (right)
                            borderQueue.Add(new Vector2(tilePos.x + tileWidth, tilePos.y));

                        if (bottom)
                            borderQueue.Add(new Vector2(tilePos.x, tilePos.y - tileHeight));


                        //check corners
                        if (top && left)
                            borderQueue.Add(new Vector2(tilePos.x - tileWidth, tilePos.y + tileHeight));

                        if (top && right)
                            borderQueue.Add(new Vector2(tilePos.x + tileWidth, tilePos.y + tileHeight));

                        if (bottom && left)
                            borderQueue.Add(new Vector2(tilePos.x - tileWidth, tilePos.y - tileHeight));

                        if (bottom && right)
                            borderQueue.Add(new Vector2(tilePos.x + tileWidth, tilePos.y - tileHeight));


                        foreach (var item in borderQueue)
                        {
                            SpawnBlock(item, parent.transform);
                        }

                        borderQueue.Clear();
                    }
                }
            }
        }

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
                        if (t.GoInstance)
                            t.GoInstance.GetComponent<TileBehaviour>().SetupTile();
                    }
                }
            }
        }

        //TODO: Include borders in composites
        MakeComposites();
    }

    private void MakeComposites()
    {
        Queue<TileBehaviour> horizontalQueue = new Queue<TileBehaviour>();
        int amountOfPlatforms = 0;
        for (int i = 0; i < Layouts.GetLength(0); i++)
        {
            for (int j = 0; j < Layouts.GetLength(1); j++)
            {
                // Each layout
                for (int x = 0; x < Layouts[i, j].Tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < Layouts[i, j].Tiles.GetLength(1); y++)
                    {

                        Tile c = Layouts[i, j].Tiles[x, y];
                        if (c.GoInstance)
                        {
                            // Each tile
                            Tile t = Layouts[i, j].Tiles[x, y];
                            TileBehaviour temp = null;
                            if (t.GoInstance)
                                temp = t.GoInstance.GetComponent<TileBehaviour>();
                            if (temp && !temp.Touched)
                            {
                                temp.StartVerticalComposite(ref amountOfPlatforms);
                                if (!temp.Touched)
                                    horizontalQueue.Enqueue(temp);
                            }
                        }
                    }
                }
            }
        }

        while (horizontalQueue.Count > 0)
        {
            TileBehaviour tb = horizontalQueue.Dequeue();
            if (!tb.Touched)
                tb.StartHorizontalComposite(ref amountOfPlatforms);
        }
        LevelManager.Instance.SpawnBackGround(new Vector2(
            (Layouts.GetLength(0) / 2 *
            Layouts[0, 0].Tiles.GetLength(0)),
            (Layouts.GetLength(1) / 2 *
            Layouts[0, 0].Tiles.GetLength(1)) * -1));
    }

    private void SpawnBlock(Vector2 v, Transform t)
    {
        GameObject.Instantiate(LevelManager.Instance.BorderTile, v, Quaternion.identity, t);
    }

    /// <summary>
    /// Spawn only one layout
    /// </summary>
    /// <param name="transform">parent to spawn under</param>
    /// <param name="layout">layout to spawn</param>
    public void Spawn(Transform transform, Layout layout)
    {
        //Find the size of one tile, make better in future
        var test = LevelDataManager.Instance.Tiles[0].transform;
        var tileHeight = test.localScale.y;
        var tileWidth = test.localScale.x;

        for (int i = 0; i < layout.Tiles.GetLength(0); i++)
        {
            for (int j = 0; j < layout.Tiles.GetLength(1); j++)
            {
                Tile t = layout.Tiles[i, j];
                if (t.Prefab != null)
                    GameObject.Instantiate(t.Prefab,
                        transform.position + new Vector3(i * tileWidth, ((j * tileHeight) * -1)),
                        Quaternion.identity,
                        transform);
            }
        }
    }
}
