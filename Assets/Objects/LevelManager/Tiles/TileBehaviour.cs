using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
    public bool TopCollision { get; set; }
    public bool BottomCollision { get; set; }
    public bool LeftCollision { get; set; }
    public bool RightCollision { get; set; }

    [SerializeField]
    private LayerMask mask;

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
}