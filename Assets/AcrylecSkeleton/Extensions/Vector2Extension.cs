using System;
using AcrylecSkeleton.Utilities;
using UnityEngine;

namespace AcrylecSkeleton.Extensions
{
    public static class Vector2Extension
    {
        /// <summary>
        /// Return a normalized direction from this vector to another.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector2 DirectionTo(this Vector2 from, Vector2 to)
        {
            return DirectionFromTo(@from, to);
        }

        /// <summary>
        /// Returns a normalized direction from a vector to another.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector2 DirectionFromTo(Vector2 from, Vector2 to)
        {
            Vector2 delta = (to - @from).normalized;
            if (delta == Vector2.zero)
                return Vector2.zero;

            return delta;
        }

        /// <summary>
        /// Return the angle between two vectors in degrees.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float AngleBetween(Vector2 v1, Vector2 v2)
        {
            v1.Normalize();
            v2.Normalize();

            var dot = Vector2.Dot(v1, v2);
            var radians = Math.Acos(dot);
            return (float)(radians * MathUtils.RadianToDegree);
        }

        /// <summary>
        /// Return the angle between this vector and another in degrees.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float AngleBetweenTO(this Vector2 v1, Vector2 v2)
        {
            v1.Normalize();
            v2.Normalize();

            var dot = Vector2.Dot(v1, v2);
            var radians = Math.Acos(dot);
            return (float)(radians * MathUtils.RadianToDegree);
        }

        /// <summary>
        /// Rotates this vector with degrees.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Vector2 RotateVector2Degree(this Vector2 vector, float degree)
        {
            var direction = MathUtils.VectorToDegrees(vector);
            direction += degree;
            return MathUtils.DegreesToVector(direction);
        }

        /// <summary>
        /// Return this vector to degree.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float Vector2ToDegree(this Vector2 vector)
        {
            return Mathf.Atan2(vector.x, vector.y) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Return this vector to radian.
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float Vector2ToRadian(this Vector2 vector)
        {
            return Mathf.Atan2(vector.x, vector.y);
        }

    }
}