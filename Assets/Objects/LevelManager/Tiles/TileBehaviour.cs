using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    
    [SerializeField]
    private GameObject _parent;

    [SerializeField]
    private LayerMask mask;

    private List<GameObject> _targets = new List<GameObject>();
    private Queue<TileBehaviour> _queue = new Queue<TileBehaviour>();

    private bool Vertical;
    private bool _isTop;

    public bool Touched { get; set; }

    public bool TopCollision { get; set; }
    public bool BottomCollision { get; set; }
    public bool LeftCollision { get; set; }
    public bool RightCollision { get; set; }

    public void CheckSides()
    {
        var halfHeight = GetComponent<SpriteRenderer>().bounds.size.y / 2 + .1f;
        var halfWidth = GetComponent<SpriteRenderer>().bounds.size.x / 2 + .1f;

        if (Physics2D.RaycastAll(transform.position, Vector2.up, halfHeight, mask).Length > 0)
            TopCollision = true;
        if (Physics2D.RaycastAll(transform.position, Vector2.down, halfHeight, mask).Length > 0)
            BottomCollision = true;
        if (Physics2D.RaycastAll(transform.position, Vector2.left, halfWidth, mask).Length > 0)
            LeftCollision = true;
        if (Physics2D.RaycastAll(transform.position, Vector2.right, halfWidth, mask).Length > 0)
            RightCollision = true;
    }


    public void StartHorizontalComposite()
    {
        List<GameObject> targets = new List<GameObject>();

        CheckHorizontalComposite(ref targets, false);
        if (targets.Count <= 1)
            return;

        var parent = Instantiate(_parent);
        foreach (var target in targets)
        {
            target.transform.SetParent(parent.transform, true);
        }
    }

    public void CheckHorizontalComposite(ref List<GameObject> targets, bool continueDown)
    {
        bool nextShouldDown = continueDown;

        targets.Add(gameObject);
        TileBehaviour tLeft = null;
        TileBehaviour tRight = null;
        TileBehaviour tUp = null;
        TileBehaviour tDown = null;

        var left = Physics2D.Raycast(transform.position, Vector2.left, 1, mask);
        var right = Physics2D.Raycast(transform.position, Vector2.right, 1, mask);
        var up = Physics2D.Raycast(transform.position, Vector2.up, 1, mask);
        var down = Physics2D.Raycast(transform.position, Vector2.down, 1, mask);

        if (left.collider != null)
            tLeft = left.collider.gameObject.GetComponent<TileBehaviour>();
        if (right.collider != null)
            tRight = right.collider.gameObject.GetComponent<TileBehaviour>();
        if (up.collider != null)
            tUp = up.collider.gameObject.GetComponent<TileBehaviour>();
        if (down.collider != null)
            tDown = down.collider.gameObject.GetComponent<TileBehaviour>();

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

    public void StartVerticalComposite()
    {
        List<GameObject> targets = new List<GameObject>();

        CheckVerticalCompisite(ref targets,false);
        if(targets.Count <= 1)
            return;

        var parent = Instantiate(_parent);
        foreach (var target in targets)
        { 
            target.transform.SetParent(parent.transform, true);
            if (target == gameObject)
            {
                TileBehaviour temp = target.GetComponent<TileBehaviour>();
                if (temp)
                {
                    TileBehaviour tb = (TileBehaviour)parent.AddComponent(temp.GetType());
                    if (tb)
                    {
                        tb.LeftCollision = temp.LeftCollision;
                        tb.RightCollision = temp.RightCollision;
                        tb.TopCollision = temp.TopCollision;
                        tb.BottomCollision = temp.BottomCollision;
                    }
                }
            }
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

        var left = Physics2D.Raycast(transform.position, Vector2.left, 1, mask);
        var right = Physics2D.Raycast(transform.position, Vector2.right, 1, mask);
        var up = Physics2D.Raycast(transform.position, Vector2.up, 1, mask);
        var down = Physics2D.Raycast(transform.position, Vector2.down, 1, mask);

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