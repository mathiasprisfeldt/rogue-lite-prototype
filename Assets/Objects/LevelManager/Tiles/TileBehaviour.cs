using System;
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

    public TilePos(int lX, int lY, int tX, int tY) : this()
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
    [SerializeField]
    private bool _physicalBlock;

    [SerializeField]
    private bool _trap;

    [SerializeField]
    private bool _isGrabable = true;

    [SerializeField]
    private bool _isSlideable = true;

    private Queue<TileBehaviour> _queue = new Queue<TileBehaviour>();

    private bool _isTop;


    public bool Touched { get; set; }
    [SerializeField]
    private string _targetTag;
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

    public Tile TopTile { get; set; }
    public Tile BottomTile { get; set; }
    public Tile LeftTile { get; set; }
    public Tile RightTile { get; set; }

    public bool SetupDone { get; set; }

    public TilePos TilePos { get; set; }

    public bool PhsyicalBlock
    {
        get { return _physicalBlock; }
    }

    public string TargetTag
    {
        get { return _targetTag; }
        set { _targetTag = value; }
    }

    public bool Trap
    {
        get { return _trap; }
    }

    public bool IsGrabable
    {
        get { return _isGrabable; }
    }

    public bool IsSlideable
    {
        get { return _isSlideable; }
    }

    [SerializeField]
    private LayerMask _colMask;

    public void SetupTile()
    {
        TopTile = LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.up);
        BottomTile = LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.down);
        LeftTile = LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.left);
        RightTile = LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.right);

        //Up
        if (TopTile.PhysicalBlock)
        {
            TopCollision = gameObject.tag != "Ladder";
        }
        //Down
        if (BottomTile.PhysicalBlock)
        {
            BottomCollision = true;
            if (BottomTile.GoInstance != null)
            {
                Climbable clim = BottomTile.GoInstance.GetComponent<Climbable>();
                if (clim != null)
                    clim.Top.gameObject.SetActive(false);
            }

        }

        LeftCollision = LeftTile.PhysicalBlock;
        RightCollision = RightTile.PhysicalBlock;

        if (_autoTexturize)
        {
            var spr = GetComponent<SpriteRenderer>();
            Sprite newSprite = spr.sprite;

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
        SetupDone = true;
    }

    public void StartHorizontalComposite(ref int amountOfPlatforms)
    {
        List<GameObject> targets = new List<GameObject>();
        List<TileBehaviour> tiles = new List<TileBehaviour>();
        Touched = true;

        CheckHorizontalComposite(ref targets, _isTop, ref tiles);
        if (targets.Count <= 1)
        {
            Touched = false;
            return;
        }


        if (_parent != null)
        {
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

            pb.Istop = gameObject.tag != "Ladder";
            pb.Left = true;
            pb.Right = true;
            pb.IsGrabable = true;
            pb.IsSlideable = true;

            foreach (var target in targets)
            {
                TileBehaviour tb = target.GetComponent<TileBehaviour>();

                if (tb)
                {
                    if (!tb.LeftCollision)
                        pb.Left = false;
                    if (!tb.RightCollision)
                        pb.Right = false;
                    if (tb.TopCollision)
                        pb.Istop = false;
                    if (!tb.IsGrabable)
                        pb.IsGrabable = false;
                    if (!tb.IsSlideable)
                        pb.IsSlideable = false;

                }



                pb.Tiles.Add(this);
                target.transform.SetParent(parent.transform, true);
                Platforms p = superParent.GetComponent<Platforms>();
                p.ParentToThis(parent.transform, pb.Tiles.Count, false);

                Climbable climbable = target.GetComponent<Climbable>();
                if (climbable != null)
                {
                    climbable.Top.transform.SetParent(parent.transform, true);
                }
            }
        }
        else
        {
            foreach (var tileBehaviour in tiles)
            {
                tileBehaviour.Touched = false;
            }
        }

    }

    public void CheckHorizontalComposite(ref List<GameObject> targets, bool isTop, ref List<TileBehaviour> tiles)
    {
        targets.Add(gameObject);
        TileBehaviour tLeft = null;
        TileBehaviour tRight = null;

        var left = LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.left).GoInstance;
        var right = LevelManager.Instance.CurrentLevel.GetTile(TilePos, Vector2.right).GoInstance;

        if (left != null)
            tLeft = left.GetComponent<TileBehaviour>();
        if (right != null)
            tRight = right.GetComponent<TileBehaviour>();

        if (tLeft && !tLeft.Touched && tLeft._isTop == isTop && (tLeft.TargetTag == TargetTag))
        {
            tLeft.Touched = true;
            _queue.Enqueue(tLeft);
            tiles.Add(tLeft);
        }

        if (tRight && !tRight.Touched && tRight._isTop == isTop && (tRight.TargetTag == TargetTag))
        {
            tRight.Touched = true;
            _queue.Enqueue(tRight);
            tiles.Add(tRight);
        }

        while (_queue.Count > 0)
        {
            var temp = _queue.Dequeue();
            temp.Touched = true;
            temp.CheckHorizontalComposite(ref targets, isTop, ref tiles);
        }
    }

    public void StartVerticalComposite(ref int amountOfPlatforms)
    {
        List<GameObject> targets = new List<GameObject>();
        List<TileBehaviour> tiles = new List<TileBehaviour>();
        Touched = true;

        CheckVerticalCompisite(ref targets, false, ref tiles);
        if (targets.Count <= 1)
        {
            Touched = false;
            return;
        }

        if (_parent != null)
        {
            GameObject parent = Instantiate(_parent, Vector2.zero, Quaternion.identity, transform.root);

            GameObject superParent = GameObject.FindObjectOfType<Platforms>().gameObject;

            if (!superParent)
            {
                superParent = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity);
                superParent.name = "Platforms";
            }

            parent.name = parent.name + amountOfPlatforms;
            amountOfPlatforms++;
            PlatformBehavior pb = parent.AddComponent<PlatformBehavior>();
            pb.IsGrabable = true;
            pb.IsSlideable = true;

            foreach (var target in targets)
            {
                if (target == gameObject)
                {
                    pb.Istop = _isTop;
                }

                TileBehaviour tb = target.GetComponent<TileBehaviour>();
                if (tb)
                {
                    if (tb.gameObject == gameObject)
                    {
                        pb.Left = tb.LeftCollision;
                        pb.Right = tb.RightCollision;
                    }
                        
                    if (!tb.IsGrabable)
                        pb.IsGrabable = false;
                    if (!tb.IsSlideable)
                        pb.IsSlideable = false;
                }

                Platforms p = superParent.GetComponent<Platforms>();
                p.ParentToThis(parent.transform, pb.Tiles.Count, true);

                pb.Tiles.Add(this);
                target.transform.SetParent(parent.transform);
            }


        }
        else
        {
            foreach (var tileBehaviour in tiles)
            {
                tileBehaviour.Touched = false;
            }
        }


    }

    public void CheckVerticalCompisite(ref List<GameObject> targets, bool continueDown, ref List<TileBehaviour> tiles)
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

        var goDown = _isTop && ((tLeft && tLeft.TargetTag != _targetTag) && (tRight && tRight.TargetTag != TargetTag));

        if (goDown || /*tDown && !tDown.LeftCollision && !tDown.RightCollision ||*/ nextShouldDown)
            nextShouldDown = true;

        if (nextShouldDown && tDown && (tDown.TargetTag == TargetTag))
        {
            Touched = true;
            tDown.Touched = true;
            _queue.Enqueue(tDown);
            tiles.Add(tDown);
        }

        while (_queue.Count > 0)
        {
            var temp = _queue.Dequeue();
            temp.Touched = true;
            temp.CheckVerticalCompisite(ref targets, nextShouldDown, ref tiles);
        }
    }
}