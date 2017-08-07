using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCollisionSides : MonoBehaviour
{
    [SerializeField]
    private bool _top;

    [SerializeField]
    private bool _bottom;

    [SerializeField]
    private bool _right;

    [SerializeField]
    private bool _left;

    public bool Top
    {
        get { return _top; }
        set { _top = value; }
    }

    public bool Bottom
    {
        get { return _bottom; }
        set { _bottom = value; }
    }

    public bool Right
    {
        get { return _right; }
        set { _right = value; }
    }

    public bool Left
    {
        get { return _left; }
        set { _left = value; }
    }
}
