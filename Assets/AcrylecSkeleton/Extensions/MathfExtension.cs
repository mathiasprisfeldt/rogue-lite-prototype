using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RigidbodyExtension 
{
    public static float CalculateVerticalSpeed(this Rigidbody2D rig, float targetHeight)
    {
        return Mathf.Sqrt(2f * targetHeight * -Physics2D.gravity.y * rig.gravityScale);
    }

    public static float CounterGravity(this Rigidbody2D rig, float amount)
    {
        return -Physics2D.gravity.y*rig.gravityScale + amount;
    }
}
