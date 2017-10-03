using System;
using System.Collections.Generic;
using System.Linq;
using Archon.SwissArmyLib.Events;
using Controllers;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    public const int 
        ON_TRIGGER_ENTER = 0,
        ON_TRIGGER_EXIT  = 1,
        ON_TRIGGER_STAY  = 2;

#pragma warning disable 0414
    private List<Collider2D> _tempList = new List<Collider2D>();
#pragma warning restore 0414

    [SerializeField]
    private List<Collider2D> _collidersToCheck = new List<Collider2D>();

    [SerializeField]
    private LayerMask _collisionLayers;

    [SerializeField]
    private float _tolerance;

    [SerializeField]
    private Character _character;

    private bool _isDirty;

    private CollisionSides _collisionSides = new CollisionSides();
    private List<Collider2D> _collisionColliders = new List<Collider2D>();
    private Collider2D[] _contacts = new Collider2D[30];
    private ContactFilter2D _contactFilter = new ContactFilter2D();

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

    public Character Character
    {
        get { return _character; }
        set { _character = value; }
    }

    public bool Top
    {
        get
        {
            if (_isDirty)
                return Sides.Top;
            IsColliding(_collisionSides);
            return _collisionSides.Top;
        }
    }
    public bool Bottom
    {
        get
        {
            if (_isDirty)
                return Sides.Bottom;
            IsColliding(_collisionSides);
            return _collisionSides.Bottom;
        }
    }
    public bool Left
    {
        get
        {
            if (_isDirty)
                return Sides.Left;
            IsColliding(_collisionSides);
            return _collisionSides.Left;
        }
    }
    public bool Right
    {
        get
        {
            if (_isDirty)
                return Sides.Right;
            IsColliding(_collisionSides);
            return _collisionSides.Right;
        }
    }

    public CollisionSides Sides
    {
        get
        {
            if(_isDirty)
                return _collisionSides;
            IsColliding(_collisionSides);
            return _collisionSides;
        }

    }

    public readonly Event<Collider2D> TriggerEnter = new Event<Collider2D>(ON_TRIGGER_ENTER);
    public readonly Event<Collider2D> TriggerExit = new Event<Collider2D>(ON_TRIGGER_EXIT);
    public readonly Event<Collider2D> TriggerStay = new Event<Collider2D>(ON_TRIGGER_STAY);

    void Awake()
    {
        if (!CollidersToCheck.Any())
            Debug.LogWarning("CollisionCheck doesn't have any colliders to check on!", transform);
        _contactFilter.useTriggers = true;
        _contactFilter.useLayerMask = true;
    }

    public bool IsColliding()
    {
        return IsColliding(CollisionLayers);
    }

    public bool IsColliding(out List<Collider2D> colliders)
    {
        return IsColliding(CollisionLayers, out colliders);
    }

    public bool IsColliding(CollisionSides sides)
    {
        return IsColliding(CollisionLayers, out _tempList, sides);
    }

    public bool IsColliding(out List<Collider2D> colliders, CollisionSides sides)
    {
        return IsColliding(CollisionLayers, out colliders, sides);
    }

    public bool IsColliding(LayerMask layer)
    {
        return IsColliding(layer, out _tempList);
    }

    public bool IsColliding(LayerMask layer, out List<Collider2D> colliders)
    {
        return IsColliding(layer, out colliders, _collisionSides);
    }

    public bool IsColliding(LayerMask layer, out List<Collider2D> colliders, CollisionSides sides)
    {
        if (_isDirty && layer == _collisionLayers)
        {
            colliders = _collisionColliders;
            sides = Sides;
            return sides.Top || sides.Bottom || sides.Right || sides.Left;
        }

        sides.Reset();

        foreach (var c in CollidersToCheck)
        {
            for (int i = 0; i < _contacts.Length; i++)
            {
                _contacts[i] = null;
            }

            if (c == null)
                continue;
            _contactFilter.layerMask = layer;

            int numberOfCollisions = c.GetContacts(_contactFilter,_contacts);

            if (numberOfCollisions == 0)
                break;



            for (int i = 0; i < _contacts.Length; i++)
            {
                if (_contacts[i] == null)
                    continue;
                if (CollisionLayers == (CollisionLayers | (1 << _contacts[i].gameObject.layer)))
                {

                    if (sides.Top || (c.bounds.min.y <= _contacts[i].bounds.min.y
                        && (_tolerance == 0 || Mathf.Abs(c.bounds.max.y - _contacts[i].bounds.min.y) <= _tolerance)))
                    {
                        sides.Top = true;
                        sides.TopColliders.Add(_contacts[i]);

                    }

                    if (sides.Bottom || (c.bounds.max.y >= _contacts[i].bounds.max.y
                        && (_tolerance == 0 || Mathf.Abs(c.bounds.min.y - _contacts[i].bounds.max.y) <= _tolerance)))
                    {
                        sides.Bottom = true;
                        sides.BottomColliders.Add(_contacts[i]);
                    }


                    if (sides.Right || (c.bounds.min.x <= _contacts[i].bounds.min.x
                        && (_tolerance == 0 || Mathf.Abs(c.bounds.max.x - _contacts[i].bounds.min.x) <= _tolerance)))
                    {
                        sides.Right = true;
                        sides.RightColliders.Add(_contacts[i]);
                    }

                    if (sides.Left || (c.bounds.max.x >= _contacts[i].bounds.max.x
                        && (_tolerance == 0 || Mathf.Abs(c.bounds.min.x - _contacts[i].bounds.max.x) <= _tolerance)))
                    {
                        sides.Left = true;
                        sides.LeftColliders.Add(_contacts[i]);
                    }

                    //If the target collider is inside us, we check which one is superior.
                    if (sides.Left && sides.Right)
                    {
                        sides.ApproxHorizontal = _contacts[i].bounds.center.x > c.bounds.center.x
                            ? CollisionSides.ColHorizontal.Right
                            : CollisionSides.ColHorizontal.Left;
                    }
                    else
                    {
                        if (sides.Left)
                            sides.ApproxHorizontal = CollisionSides.ColHorizontal.Left;
                        else if (sides.Right)
                            sides.ApproxHorizontal = CollisionSides.ColHorizontal.Right;
                    }

                    //Also do this for top and bottom
                    if (sides.Top && sides.Bottom)
                    {
                        sides.ApproxVertical = _contacts[i].bounds.center.y > c.bounds.center.y
                            ? CollisionSides.ColVertical.Bottom
                            : CollisionSides.ColVertical.Top;
                    }
                    else
                    {
                        if (sides.Top)
                            sides.ApproxVertical = CollisionSides.ColVertical.Top;
                        else if (sides.Bottom)
                            sides.ApproxVertical = CollisionSides.ColVertical.Bottom;
                    }

                    sides.TargetColliders.Add(_contacts[i]);
                }
            }
        }
        colliders = sides.TargetColliders;

        if (layer == _collisionLayers)
            SetDirty(sides);

        return sides.TargetColliders.Any();
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
    
    /// <summary>
    /// Used for proxy invoke tunnel.
    /// See <see cref="OnTriggerEnter"/>
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerEnter.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TriggerExit.Invoke(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TriggerStay.Invoke(collision);
    }
}

public class CollisionSides
{
    public enum ColHorizontal
    {
        None,
        Left,
        Right
    }

    public enum ColVertical
    {
        None,
        Top,
        Bottom
    }

    public ColHorizontal ApproxHorizontal { get; set; }
    public ColVertical ApproxVertical { get; set; }

    public bool Top { get; set; }
    public bool Bottom { get; set; }
    public bool Right { get; set; }
    public bool Left { get; set; }

    public List<Collider2D> TopColliders { get; set; }
    public List<Collider2D> BottomColliders { get; set; }
    public List<Collider2D> RightColliders { get; set; }
    public List<Collider2D> LeftColliders { get; set; }
    public List<Collider2D> TargetColliders { get; set; }

    public CollisionSides()
    {
        TopColliders = new List<Collider2D>();
        BottomColliders = new List<Collider2D>();
        RightColliders = new List<Collider2D>();
        LeftColliders = new List<Collider2D>();
        TargetColliders = new List<Collider2D>();
    }

    public void Reset()
    {
        ApproxHorizontal = ColHorizontal.None;
        ApproxVertical = ColVertical.None;

        Top = false;
        Bottom = false;
        Left = false;
        Right = false;

        TopColliders.Clear();
        BottomColliders.Clear();
        RightColliders.Clear();
        LeftColliders.Clear();
        TargetColliders.Clear();
    }
}
