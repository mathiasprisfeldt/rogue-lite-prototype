using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public struct TilePos
{
    public int LX { get; set; }
    public int LY { get; set; }
    public int TX { get; set; }
    public int TY { get; set; }

    public TilePos(int lX, int lY, int tX, int tY)
    {
        LX = lX;
        LY = lY;
        TX = tX;
        TY = tY;
    }
}

public class TileBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _parent;

    private Queue<TileBehaviour> _queue = new Queue<TileBehaviour>();

    private bool _isTop;

    public bool Touched { get; set; }
    [SerializeField]
    private bool _autoTexturize;
    [SerializeField]
    private Sprite _leftTexture;
    [SerializeField]
    private Sprite _middleTexture;
    [SerializeField]
    private Sprite _rightTexture;
    [SerializeField]
    private Sprite _centerTexture;

    public bool TopCollision { get; set; }
    public bool BottomCollision { get; set; }
    public bool LeftCollision { get; set; }
    public bool RightCollision { get; set; }

    public TilePos TilePos { get; set; }

    [SerializeField]
    private LayerMask _colMask;

    public void SetupTile()
    {
        if (_autoTexturize)
        {
            var halfHeight = GetComponent<SpriteRenderer>().bounds.size.y / 2 + .1f;
            var halfWidth = GetComponent<SpriteRenderer>().bounds.size.x / 2 + .1f;

            //Up
            if (LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.up).Type == 1)
            {
                TopCollision = true;
            }
            //Down
            if (LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.down).Type == 1)
            {
                BottomCollision = true;
            }
            //Left
            if (LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.left).Type == 1)
            {
                LeftCollision = true;
            }
            //Right
            if (LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.right).Type == 1)
            {
                RightCollision = true;
            }

            var spr = GetComponent<SpriteRenderer>();
            Sprite newSprite = null;

            if (LeftCollision && !RightCollision)
                newSprite = _rightTexture;

            if (!LeftCollision && RightCollision)
                newSprite = _leftTexture;

            if (BottomCollision || (LeftCollision && RightCollision))
                newSprite = _middleTexture;

            if (TopCollision)
                newSprite = _centerTexture;


            spr.sprite = newSprite;
        }
    }

    public void StartHorizontalComposite(ref int amountOfPlatforms)
    {
        List<GameObject> targets = new List<GameObject>();
        Touched = true;

        CheckHorizontalComposite(ref targets, false);
        if (targets.Count <= 1)
        {
            Touched = false;
            return;
        }

        var parent = Instantiate(_parent, Vector2.zero, Quaternion.identity, transform.root);
        parent.name = parent.name + amountOfPlatforms;
        amountOfPlatforms++;
        PlatformBehavior pb = parent.AddComponent<PlatformBehavior>();

        GameObject superParent = GameObject.FindObjectOfType<Platforms>().gameObject;

        if (!superParent)
        {
            superParent = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
            superParent.name = "Platforms";
        }

        foreach (var target in targets)
        {
            if (target == gameObject)
                pb.Istop = _isTop;
            pb.Tiles.Add(this);
            target.transform.SetParent(parent.transform, true);
        }

        Platforms p = superParent.GetComponent<Platforms>();
        p.ParentToThis(parent.transform, pb.Tiles.Count, false);
    }

    public void CheckHorizontalComposite(ref List<GameObject> targets, bool continueDown)
    {
        bool nextShouldDown = continueDown;

        targets.Add(gameObject);
        TileBehaviour tLeft = null;
        TileBehaviour tRight = null;

        var left = LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.left).GoInstance;
        var right = LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.right).GoInstance;

        if (left != null)
            tLeft = left.GetComponent<TileBehaviour>();
        if (right != null)
            tRight = right.GetComponent<TileBehaviour>();

        if (tLeft && !tLeft.Touched)
        {
            tLeft.Touched = true;
            _queue.Enqueue(tLeft);
        }

        if (tRight && !tRight.Touched)
        {
            tRight.Touched = true;
            _queue.Enqueue(tRight);
        }

        while (_queue.Count > 0)
        {
            var temp = _queue.Dequeue();
            temp.Touched = true;
            temp.CheckHorizontalComposite(ref targets, nextShouldDown);
        }
    }

    public void StartVerticalComposite(ref int amountOfPlatforms)
    {
        List<GameObject> targets = new List<GameObject>();
        Touched = true;

        CheckVerticalCompisite(ref targets, false);
        if (targets.Count <= 1)
        {
            Touched = false;
            return;
        }

        var parent = Instantiate(_parent, Vector2.zero, Quaternion.identity, transform.root);

        GameObject superParent = GameObject.FindObjectOfType<Platforms>().gameObject;

        if (!superParent)
        {
            superParent = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
            superParent.name = "Platforms";
        }

        parent.name = parent.name + amountOfPlatforms;
        amountOfPlatforms++;
        PlatformBehavior pb = parent.AddComponent<PlatformBehavior>();

        foreach (var target in targets)
        {
            if (target == gameObject)
                pb.Istop = _isTop;
            pb.Tiles.Add(this);
            target.transform.SetParent(parent.transform, true);
        }

        Platforms p = superParent.GetComponent<Platforms>();
        p.ParentToThis(parent.transform, pb.Tiles.Count, true);

    }

    public void CheckVerticalCompisite(ref List<GameObject> targets, bool continueDown)
    {
        bool nextShouldDown = continueDown;

        targets.Add(gameObject);
        TileBehaviour tLeft = null;
        TileBehaviour tRight = null;
        TileBehaviour tUp = null;
        TileBehaviour tDown = null;

        var left = LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.left).GoInstance;
        var right = LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.right).GoInstance;
        var up = LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.up).GoInstance;
        var down = LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.down).GoInstance;

        if (left != null)
            tLeft = left.GetComponent<TileBehaviour>();
        if (right != null)
            tRight = right.GetComponent<TileBehaviour>();
        if (up != null)
            tUp = up.GetComponent<TileBehaviour>();
        if (down != null)
            tDown = down.GetComponent<TileBehaviour>();

        _isTop = !tUp;

        var goDown = _isTop && (!tLeft && !tRight);

        if ( goDown || !goDown && (!tLeft && !tRight) || nextShouldDown)
            nextShouldDown = true;

        if (nextShouldDown && tDown)
        {
            Touched = true;
            tDown.Touched = true;
            _queue.Enqueue(tDown);
        }

        while (_queue.Count > 0)
        {
            var temp = _queue.Dequeue();
            temp.Touched = true;
            temp.CheckVerticalCompisite(ref targets, nextShouldDown);
        }
    }
}