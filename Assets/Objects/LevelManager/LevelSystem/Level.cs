using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Class for containing a level
/// </summary>
public class Level
{
    /// <summary>
    /// The layouts in the level
    /// </summary>
    public Layout[,] Layouts { get; set; }

    public Dictionary<TilePos, TileBehaviour> BorderTiles { get; set; }

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
                var layoutCandidate = LevelDataManager.Instance.Layouts.FirstOrDefault(x => x.ID == l[i, j]);

                Layouts[i, j] = new Layout(layoutCandidate.ID, layoutCandidate.Tiles.Clone() as Tile[,]);
            }
        }

        ID = id;
    }

    /// <summary>
    /// Get a tile in the grid
    /// </summary>
    /// <param name="pos">Position of the tile</param>
    /// <returns>the tile at that position</returns>
    public Tile GetTile(TilePos pos)
    {
        return Layouts[pos.LX, pos.LY].Tiles[pos.TX, pos.TY];
    }

    /// <summary>
    /// Get a tile vi a position and a vector direction
    /// </summary>
    /// <param name="pos">Start position</param>
    /// <param name="dir">direction, whole numbers only</param>
    /// <returns>Tile at that position, null if outside borders</returns>
    public Tile GetTile(TilePos pos, Vector2 dir)
    {
        TilePos t = new TilePos(pos.LX, pos.LY, pos.TX + (int)dir.x, pos.TY - (int)dir.y);

        bool left = t.TX < 0 && t.LX == 0;
        bool top = t.TY < 0 && t.LY == 0;
        bool right = t.TX > Layouts[t.LX, t.LY].Tiles.GetLength(0) - 1 && pos.LX == Layouts.GetLength(0) - 1;
        bool bottom = t.TY > Layouts[t.LX, t.LY].Tiles.GetLength(1) - 1 && pos.LY == Layouts.GetLength(1) - 1;

        //Border tile
        if (left || right || top || bottom)
        {
            if (BorderTiles.Keys.Contains(t))
                return new Tile(1, BorderTiles[t].gameObject);
            else
                return new Tile(null, -1);
        }

        //Left
        if (t.TX < 0 && t.LX > 0)
        {
            t.LX--;
            t.TX = (Layouts[t.LX, t.LY].Tiles.GetLength(0) - 1) + t.TX;
        }
        //Right
        else if (t.TX > Layouts[t.LX, t.LY].Tiles.GetLength(0) - 1 && t.LX < Layouts.GetLength(0) - 1)
        {
            t.TX = t.TX - (Layouts[t.LX, t.LY].Tiles.GetLength(0) - 1);
            t.LX++;
        }
        //Up
        if (t.TY < 0 && t.LY > 0)
        {
            t.LY--;
            t.TY = (Layouts[t.LX, t.LY].Tiles.GetLength(1) - 1) + t.TY;
        }
        //Down
        else if (t.TY > Layouts[t.LX, t.LY].Tiles.GetLength(1) - 1 && t.LY < Layouts.GetLength(1) - 1)
        {
            t.TY = t.TY - (Layouts[t.LX, t.LY].Tiles.GetLength(1) - 1);
            t.LY++;
        }

        return Layouts[t.LX, t.LY].Tiles[t.TX, t.TY];
    }

    /// <summary>
    /// Spawns the current layouts in this level according to a given transform
    /// </summary>
    /// <param name="transform">transform to spawn under</param>
    public void Spawn(Transform transform)
    {
        BorderTiles = new Dictionary<TilePos, TileBehaviour>();

        var test = LevelDataManager.Instance.Tiles[0].GetComponent<SpriteRenderer>();
        var tileHeight = test.bounds.size.y;
        var tileWidth = test.bounds.size.x;

        bool top = false;
        bool bottom = false;
        bool left = false;
        bool right = false;

        List<Vector2> borderQueue = new List<Vector2>();
        List<TileBehaviour> tileList = new List<TileBehaviour>();
        List<TileBehaviour> borderTileList = new List<TileBehaviour>();

        //A number to name borders
        int numberOfBorders = 0;

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
                            GameObject go = LevelManager.Instance.SpawnTile(transform.position + tilePos, t.Prefab, parent: parent.transform);
                            go.name = i.ToString() + j.ToString() + x.ToString() + y.ToString();

                            Layouts[i, j].Tiles[x, y] = new Tile(t.Type, go);

                            TileBehaviour tb = go.GetComponent<TileBehaviour>();
                            if (tb)
                            {
                                tileList.Add(tb);
                                tb.TilePos = new TilePos(i, j, x, y);
                            }
                        }

                        left = x == 0 && i == 0;
                        top = y == 0 && j == 0;
                        right = x == Layouts[i, j].Tiles.GetLength(0) - 1 && i == Layouts.GetLength(0) - 1;
                        bottom = y == Layouts[i, j].Tiles.GetLength(1) - 1 && j == Layouts.GetLength(1) - 1;

                        //Place border blocks
                        if (left)
                            borderTileList.Add(SpawnBorder(new Vector2(tilePos.x - tileWidth, tilePos.y),
                                new TilePos(i, j, x - 1, y), parent.transform));

                        if (top)
                            borderTileList.Add(SpawnBorder(new Vector2(tilePos.x, tilePos.y + tileHeight),
                                new TilePos(i, j, x, y - 1), parent.transform));

                        if (right)
                            borderTileList.Add(SpawnBorder(new Vector2(tilePos.x + tileWidth, tilePos.y),
                                new TilePos(i, j, x + 1, y), parent.transform));

                        if (bottom)
                            borderTileList.Add(SpawnBorder(new Vector2(tilePos.x, tilePos.y - tileHeight),
                                new TilePos(i, j, x, y + 1), parent.transform));


                        //check corners
                        if (top && left)
                            borderTileList.Add(SpawnBorder(new Vector2(tilePos.x - tileWidth, tilePos.y + tileHeight),
                                new TilePos(i, j, x - 1, y - 1), parent.transform));

                        if (top && right)
                            borderTileList.Add(SpawnBorder(new Vector2(tilePos.x + tileWidth, tilePos.y + tileHeight),
                                new TilePos(i, j, x + 1, y - 1), parent.transform));

                        if (bottom && left)
                            borderTileList.Add(SpawnBorder(new Vector2(tilePos.x - tileWidth, tilePos.y - tileHeight),
                                new TilePos(i, j, x - 1, y + 1), parent.transform));

                        if (bottom && right)
                            borderTileList.Add(SpawnBorder(new Vector2(tilePos.x + tileWidth, tilePos.y - tileHeight),
                                new TilePos(i, j, x + 1, y + 1), parent.transform));
                    }
                }
            }
        }

        foreach (var item in tileList)
        {
            item.SetupTile();
        }
        tileList.AddRange(borderTileList);

        MakeComposites(tileList);
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
                    LevelManager.Instance.SpawnTile(
                        transform.position + new Vector3(i * tileWidth, ((j * tileHeight) * -1)),
                        t.Prefab,
                        parent: transform);
            }
        }
    }

    /// <summary>
    /// Spawns a border tile
    /// </summary>
    /// <param name="pos">position of bordertile</param>
    /// <param name="tPos">tilepos of the tile</param>
    /// <param name="parent">Parent</param>
    /// <returns></returns>
    private TileBehaviour SpawnBorder(Vector2 pos, TilePos tPos, Transform parent)
    {
        TileBehaviour tb = LevelManager.Instance.SpawnTile(pos, parent: parent).GetComponent<TileBehaviour>();
        if (tb)
        {
            tb.gameObject.name = tPos.LX.ToString() + tPos.LY.ToString() + tPos.TX.ToString() + tPos.TY.ToString();
            tb.TilePos = tPos;
            BorderTiles.Add(tPos, tb);
        }
        return tb;
    }

    private void MakeComposites(List<TileBehaviour> list)
    {
        Queue<TileBehaviour> horizontalQueue = new Queue<TileBehaviour>();
        int amountOfPlatforms = 0;
        foreach (var item in list)
        {
            if (!item.Touched)
            {
                item.StartVerticalComposite(ref amountOfPlatforms);
                if (!item.Touched)
                    horizontalQueue.Enqueue(item);
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

    /// <summary>
    /// Despawn
    /// </summary>
    public void Despawn()
    {
        //Do despawn stuff...
    }
}
