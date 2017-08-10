using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeMaker : MonoBehaviour
{
    [SerializeField]
    private LayerMask _layers;

    [SerializeField]
    private GameObject _parent;

    private List<GameObject> _targets = new List<GameObject>();
    private Queue<CompositeMaker> _queue = new Queue<CompositeMaker>();

    public bool Touched { get; set; }

    public void Start()
    {

    }

    public void StartCheck()
    {
        //_targets.Add(gameObject);
        //Touched = true;
        //TileBehaviour tLeft = null;
        //TileBehaviour tRight = null;
        //TileBehaviour tUp = null;
        //TileBehaviour tDown = null;

        //var left = Physics2D.Raycast(transform.position, Vector2.left, 1, _layers);
        //var right = Physics2D.Raycast(transform.position, Vector2.right, 1, _layers);
        //var up = Physics2D.Raycast(transform.position, Vector2.up, 1, _layers);
        //var down = Physics2D.Raycast(transform.position, Vector2.down, 1, _layers);

        //if (left.collider != null)
        //    tLeft = left.collider.gameObject.GetComponent<TileBehaviour>();
        //if(right.collider != null)
        //    tRight = right.collider.gameObject.GetComponent<TileBehaviour>();
        //if (up.collider != null)
        //    tUp = up.collider.gameObject.GetComponent<TileBehaviour>();
        //if (right.collider != null)
        //    tDown = down.collider.gameObject.GetComponent<TileBehaviour>();

        //if(tUp || (tDown && ))

        //if (tLeft != null && !tLeft.Touched)
        //    _queue.Enqueue(tLeft);
        //if(tRight != null && !tRight.Touched)
        //    _queue.Enqueue(tRight);

        //if (tUp != null && !tUp.Touched)
        //{
        //    TileBehaviour tb = tUp.gameObject.GetComponent<TileBehaviour>();
        //    if (tb != null && !(tb.LeftCollision && tb.RightCollision))
        //    {
        //        _queue.Enqueue(tUp);
        //    }
        //}

        //if (tDown != null && !tDown.Touched)
        //{
        //    TileBehaviour tb = tDown.gameObject.GetComponent<TileBehaviour>();
        //    if (tb != null && !(tb.LeftCollision && tb.RightCollision))
        //    {
        //        _queue.Enqueue(tDown);
        //    }
        //}

        //while (_queue.Count > 0)
        //{
        //    var temp = _queue.Dequeue();
        //    temp.Touched = true;
        //    temp.CheckComposite(ref _targets);
        //}
        //var parent = Instantiate(_parent);
        //foreach (var target in _targets)
        //{
        //    target.transform.SetParent(parent.transform,true);
        //}
    }

    public void CheckSides(ref List<GameObject> targets)
    {
        targets.Add(gameObject);
        Touched = true;
        CompositeMaker cLeft = null;
        CompositeMaker cRight = null;

        var left = Physics2D.Raycast(transform.position, Vector2.left, 1, _layers);
        var right = Physics2D.Raycast(transform.position, Vector2.right, 1, _layers);

        if (left.collider != null)
            cLeft = left.collider.gameObject.GetComponent<CompositeMaker>();
        if (right.collider != null)
            cRight = right.collider.gameObject.GetComponent<CompositeMaker>();

        if (cLeft != null && !cLeft.Touched)
            _queue.Enqueue(cLeft);
        if (cRight != null && !cRight.Touched)
            _queue.Enqueue(cRight);
        while (_queue.Count > 0)
        {
            var temp = _queue.Dequeue();
            temp.Touched = true;
            temp.CheckSides(ref targets);
        }
    }
}
