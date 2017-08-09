using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehaviour : MonoBehaviour
{
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

    public bool TopCollision;
    public bool BottomCollision;
    public bool LeftCollision;
    public bool RightCollision;

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

    private void OnDrawGizmosSelected()
    {
        var halfHeight = GetComponent<SpriteRenderer>().bounds.size.y / 2 + .1f;
        var halfWidth = GetComponent<SpriteRenderer>().bounds.size.x / 2 + .1f;

        Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * halfHeight));
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * halfHeight));
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.left * halfWidth));
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.right * halfWidth));
    }
}