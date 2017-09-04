using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RigidbodyExtension 
{
    public static float CalculateVerticalSpeed(this Rigidbody2D rig, float targetHeight)
    {
        return Mathf.Sqrt(2f * targetHeight * -Physics2D.gravity.y * rig.gravityScale);
    }

    public static Vector2 CalculateVerticalSpeed(this Rigidbody2D rig, Vector2 targetHeight)
    {
        float x = targetHeight.x;
        float y = CalculateVerticalSpeed(rig,targetHeight.y);

        return new Vector2(x,y);
    }

    public static float CounterGravity(this Rigidbody2D rig, float amount)
    {
        return -Physics2D.gravity.y*rig.gravityScale + amount;
    }

    public static Vector2 CounterGravity(this Rigidbody2D rig, Vector2 velocity)
    {
        var x = velocity.x;
        var y = CounterGravity(rig,velocity.y);
        return new Vector2(x,y);
    }
}
