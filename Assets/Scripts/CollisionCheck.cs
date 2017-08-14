using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{

    private bool _isDirty;

    private CollisionSides _collisionSides = new CollisionSides(false, false, false, false);
    private List<Collider2D> _collisionColliders = new List<Collider2D>();

    [SerializeField]
    private List<Collider2D> _collidersToCheck = new List<Collider2D>();

    [SerializeField]
    private LayerMask _collisionLayers;

    [SerializeField]
    private float _tolerance;

    public List<Collider2D> CollidersToCheck
    {
        get { return _collidersToCheck; }
        set { _collidersToCheck = value; }
    }

    public LayerMask CollisionLayers
    {
        get { return _collisionLayers; }
        set { _collisionLayers = value; }
    }

    public bool Top
    {
        get
        {
            if (_isDirty)
                return Sides.Top;
            CollisionSides temp = new CollisionSides(false, false, false, false);
            IsColliding(out temp);
            return temp.Top;
        }
    }
    public bool Bottom
    {
        get
        {
            if (_isDirty)
                return Sides.Bottom;
            CollisionSides temp = new CollisionSides(false, false, false, false);
            IsColliding(out temp);
            return temp.Bottom;
        }
    }
    public bool Left
    {
        get
        {
            if (_isDirty)
                return Sides.Left;
            CollisionSides temp = new CollisionSides(false, false, false, false);
            IsColliding(out temp);
            return temp.Left;
        }
    }
    public bool Right
    {
        get
        {
            if (_isDirty)
                return Sides.Right;
            CollisionSides temp = new CollisionSides(false, false, false, false);
            IsColliding(out temp);
            return temp.Right;
        }
    }

    public CollisionSides Sides
    {
        get
        {
            if(_isDirty)
                return _collisionSides;
            IsColliding(out _collisionSides);
            return _collisionSides;
        }

    }

    public bool IsColliding()
    {
        return IsColliding(CollisionLayers);
    }
    public bool IsColliding(out List<Collider2D> colliders)
    {
        return IsColliding(CollisionLayers, out colliders);
    }
    public bool IsColliding(out CollisionSides sides)
    {
        var temp = new List<Collider2D>();
        return IsColliding(CollisionLayers, out temp, out sides);
    }
    public bool IsColliding(out List<Collider2D> colliders, out CollisionSides sides)
    {
        return IsColliding(CollisionLayers, out colliders, out sides);
    }
    public bool IsColliding(LayerMask layer)
    {
        var temp = new List<Collider2D>();
        return IsColliding(layer, out temp);
    }
    public bool IsColliding(LayerMask layer, out List<Collider2D> colliders)
    {
        var temp = new CollisionSides(false, false, false, false);
        return IsColliding(layer, out colliders, out temp);
    }
    public bool IsColliding(LayerMask layer, out List<Collider2D> colliders, out CollisionSides sides)
    {
        sides = new CollisionSides(false, false, false, false);
        bool collision = false;

        if (_isDirty && layer == _collisionLayers)
        {
            colliders = _collisionColliders.ToList();
            sides = Sides;
            return sides.Top || sides.Bottom || sides.Right || sides.Left;
        }
        sides.Colliders = CollidersToCheck.ToList();
        foreach (var c in CollidersToCheck)
        {
            Collider2D[] t = new Collider2D[10];
            if (c == null)
                continue;
            int numberOfCollisions = c.GetContacts(t);

            if (numberOfCollisions == 0)
                break;



            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] == null)
                    continue;
                if (CollisionLayers == (CollisionLayers | (1 << t[i].gameObject.layer)))
                {

                    if (sides.Top || (c.bounds.min.y <= t[i].bounds.min.y
                        && (_tolerance == 0 || Mathf.Abs(c.bounds.max.y - t[i].bounds.min.y) <= _tolerance)))
                    {
                        sides.Top = true;
                        sides.TopColliders.Add(t[i]);

                    }

                    if (sides.Bottom || (c.bounds.max.y >= t[i].bounds.max.y
                        && (_tolerance == 0 || Mathf.Abs(c.bounds.min.y - t[i].bounds.max.y) <= _tolerance)))
                    {
                        sides.Bottom = true;
                        sides.BottomColliders.Add(t[i]);
                    }


                    if (sides.Right || (c.bounds.min.x <= t[i].bounds.min.x
                        && (_tolerance == 0 || Mathf.Abs(c.bounds.max.x - t[i].bounds.min.x) <= _tolerance)))
                    {
                        sides.Right = true;
                        sides.RightColliders.Add(t[i]);
                    }

                    if (sides.Left || (c.bounds.max.x >= t[i].bounds.max.x
                        && (_tolerance == 0 || Mathf.Abs(c.bounds.min.x - t[i].bounds.max.x) <= _tolerance)))
                    {
                        sides.Left = true;
                        sides.LeftColliders.Add(t[i]);
                    }
                    collision = sides.Top || sides.Bottom || sides.Right || sides.Left;
                    if (collision)
                        sides.TargetColliders.Add(t[i]);
                }
            }
        }
        colliders = sides.TargetColliders.ToList();

        if (layer == _collisionLayers)
            SetDirty(sides);
        return collision;
    }

    private void SetDirty(CollisionSides sides)
    {
        _collisionSides = sides;
        _collisionColliders = sides.TargetColliders;
        _isDirty = true;

    }

    public void LateUpdate()
    {
        _isDirty = false;
    }
}

public struct CollisionSides
{
    public bool Top { get; set; }
    public bool Bottom { get; set; }
    public bool Right { get; set; }
    public bool Left { get; set; }

    public List<Collider2D> TopColliders { get; set; }
    public List<Collider2D> BottomColliders { get; set; }
    public List<Collider2D> RightColliders { get; set; }
    public List<Collider2D> LeftColliders { get; set; }
    public List<Collider2D> TargetColliders { get; set; }
    public List<Collider2D> Colliders { get; set; }

    public CollisionSides(bool top, bool bottom, bool right, bool left) : this()
    {
        Top = top;
        Bottom = bottom;
        Right = right;
        Left = left;

        TopColliders = new List<Collider2D>();
        BottomColliders = new List<Collider2D>();
        RightColliders = new List<Collider2D>();
        LeftColliders = new List<Collider2D>();
        TargetColliders = new List<Collider2D>();
        Colliders = new List<Collider2D>();
    }


}
