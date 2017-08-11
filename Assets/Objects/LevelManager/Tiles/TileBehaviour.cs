using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

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

    [SerializeField]
    private LayerMask _colMask;

    public void SetupTile()
    {
        var halfHeight = GetComponent<SpriteRenderer>().bounds.size.y / 2 + .1f;
        var halfWidth = GetComponent<SpriteRenderer>().bounds.size.x / 2 + .1f;

        //Up
        if (Physics2D.RaycastAll(transform.position, Vector2.up, halfHeight, _colMask).Length > 0)
        {
            TopCollision = true;
        }
        //Down
        if (Physics2D.RaycastAll(transform.position, Vector2.down, halfHeight, _colMask).Length > 0)
        {
            BottomCollision = true;
        }
        //Left
        if (Physics2D.RaycastAll(transform.position, Vector2.left, halfWidth, _colMask).Length > 0)
        {
            LeftCollision = true;
        }
        //Right
        if (Physics2D.RaycastAll(transform.position, Vector2.right, halfWidth, _colMask).Length > 0)
        {
            RightCollision = true;
        }

        if (_autoTexturize)
        {
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

        foreach (var target in targets)
        {
            if (target == gameObject)
                pb.Istop = _isTop;
            pb.Tiles.Add(this);
            target.transform.SetParent(parent.transform, true);
        }
    }

    public void CheckHorizontalComposite(ref List<GameObject> targets, bool continueDown)
    {
        bool nextShouldDown = continueDown;

        targets.Add(gameObject);
        TileBehaviour tLeft = null;
        TileBehaviour tRight = null;

        var left = Physics2D.Raycast(transform.position, Vector2.left, 1, _colMask);
        var right = Physics2D.Raycast(transform.position, Vector2.right, 1, _colMask);

        if (left.collider != null)
            tLeft = left.collider.gameObject.GetComponent<TileBehaviour>();
        if (right.collider != null)
            tRight = right.collider.gameObject.GetComponent<TileBehaviour>();


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
    }

    public void CheckVerticalCompisite(ref List<GameObject> targets, bool continueDown)
    {
        bool nextShouldDown = continueDown;

        targets.Add(gameObject);
        TileBehaviour tLeft = null;
        TileBehaviour tRight = null;
        TileBehaviour tUp = null;
        TileBehaviour tDown = null;

        var left = Physics2D.Raycast(transform.position, Vector2.left, 1, _colMask);
        var right = Physics2D.Raycast(transform.position, Vector2.right, 1, _colMask);
        var up = Physics2D.Raycast(transform.position, Vector2.up, 1, _colMask);
        var down = Physics2D.Raycast(transform.position, Vector2.down, 1, _colMask);

        if (left.collider != null)
            tLeft = left.collider.gameObject.GetComponent<TileBehaviour>();
        if (right.collider != null)
            tRight = right.collider.gameObject.GetComponent<TileBehaviour>();
        if (up.collider != null)
            tUp = up.collider.gameObject.GetComponent<TileBehaviour>();
        if (down.collider != null)
            tDown = down.collider.gameObject.GetComponent<TileBehaviour>();

        if (!tUp && (tLeft || tRight))
            _isTop = true;

        if (!_isTop || nextShouldDown)
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