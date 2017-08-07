using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    [SerializeField]
    private List<Collider2D> _colliders = new List<Collider2D>();

    [SerializeField]
    private LayerMask _collisionLayers;

    [SerializeField]
    private float _sensitivevity;

    [Header("Collision Types")]
    [SerializeField]
    private bool _topCollision;

    [SerializeField]
    private bool _bottomCollision;

    [SerializeField]
    private bool _leftCollision;

    [SerializeField]
    private bool _rightCollision;

    public bool IsColliding()
    {
        return IsColliding(_collisionLayers);
    }

    public bool IsColliding(out Collider2D col)
    {
        return IsColliding(_collisionLayers, out col);
    }

    public bool IsColliding(out CollisionSides sides)
    {
        var temp = new Collider2D();
        return IsColliding(_collisionLayers, out temp, out sides);
    }

    public bool IsColliding(out Collider2D col, out CollisionSides sides)
    {
        return IsColliding(_collisionLayers, out col,out sides);
    }

    public bool IsColliding(LayerMask layer)
    {
        var temp = new Collider2D();
        return IsColliding(layer, out temp);
    }

    public bool IsColliding(LayerMask layer, out Collider2D col)
    {
        var temp = new CollisionSides(false, false, false, false); 
        return IsColliding(layer, out col, out temp);
    }

    public bool IsColliding(LayerMask layer, out Collider2D col, out CollisionSides sides)
    {
        sides = new CollisionSides(false, false, false, false);
        bool collision = false;
            

        foreach (var c in _colliders)
        {
            Collider2D[] t = new Collider2D[10];
            if(c == null)
                continue;
            int numberOfCollisions = c.GetContacts(t);
            
            if (numberOfCollisions == 0)
                break;

            

            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] == null)
                    continue;
                if (_collisionLayers == (_collisionLayers | (1 << t[i].gameObject.layer)))
                {
                    
                    if (sides.Top || (_topCollision && c.bounds.min.y <= t[i].bounds.min.y
                        && (_sensitivevity == 0 || Mathf.Abs(c.bounds.max.y - t[i].bounds.min.y) <= _sensitivevity)))
                    {
                        sides.Top = true;
                        sides.TopColliders.Add(t[i]);
                    }

                    if (sides.Bottom || (_bottomCollision && c.bounds.max.y >= t[i].bounds.max.y 
                        && (_sensitivevity == 0 || Mathf.Abs(c.bounds.min.y - t[i].bounds.max.y) <= _sensitivevity)))
                    {
                        sides.Bottom = true;
                        sides.BottomColliders.Add(t[i]);
                    }
                    

                    if (sides.Right || (_rightCollision && c.bounds.min.x <= t[i].bounds.min.x 
                        && (_sensitivevity == 0 || Mathf.Abs(c.bounds.max.x - t[i].bounds.min.x) <= _sensitivevity)))
                    {
                        sides.Right = true;
                        sides.RightColliders.Add(t[i]);
                    }

                    if (sides.Left || (_leftCollision && c.bounds.max.x >= t[i].bounds.max.x 
                        && (_sensitivevity == 0 || Mathf.Abs(c.bounds.min.x - t[i].bounds.max.x) <= _sensitivevity)))
                    {
                        sides.Left = true;
                        sides.LeftColliders.Add(t[i]);
                    }
                    collision = sides.Top || sides.Bottom || sides.Right || sides.Left;
                    col = t[i];
                }
            }
        }
        col = null;
        return collision;
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
    }
}
