using System;
using AcrylecSkeleton.Utilities;
using UnityEngine;

namespace AcrylecSkeleton.Extensions
{
    public static  class Vector3Extension  {

        /// <summary>
        /// Returns a normalized direction vector from this vector to another.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector2 DirectionTo2D(this Vector3 from, Vector3 to)
        {
            return DirectionFromTo2D(@from, to);
        }

        /// <summary>
        /// Returns a normalized direction from a vector to another.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static Vector2 DirectionFromTo2D(Vector3 from, Vector3 to)
        {
            Vector2 delta = (new Vector2(to.x,to.y) - new Vector2(@from.x,@from.y)).normalized;
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
        public static float AngleBetween2D(Vector3 v1, Vector3 v2)
        {
            v1.Normalize();
            v2.Normalize();

            var dot = Vector2.Dot(new Vector2(v1.x,v1.y), new Vector2(v2.x,v2.y));
            var radians = Math.Acos(dot);
            return (float)(radians * MathUtils.RadianToDegree);
        }

        /// <summary>
        /// Return the angle between two vectors in degrees.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float AngleTo2D(this Vector3 v1, Vector3 v2)
        {
            v1.Normalize();
            v2.Normalize();

            var dot = Vector2.Dot(new Vector2(v1.x, v1.y), new Vector2(v2.x, v2.y));
            var radians = Math.Acos(dot);
            return (float)(radians * MathUtils.RadianToDegree);
        }

        public static Vector2 MultiplyVector2(this Vector3 v1, Vector2 v2)
        {
            Vector2 v1Converted = v1.ToVector2();
            return ((v1Converted.x * v2.x) * Vector2.right) + ((v1Converted.y * v2.y) * Vector2.up);
        }

        /// <summary>
        /// Rotate this vector with degrees.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static Vector2 RotateVector2Degree(this Vector3 vector, float degree)
        {
            var direction = MathUtils.VectorToDegrees(new Vector2(vector.x,vector.y));
            direction += degree;
            Vector2 v = MathUtils.DegreesToVector(direction);
            return new Vector3(v.x,v.y,vector.z);
        }

        public static Vector2 ToVector2(this Vector3 v)
        {
            return Vector2.right * v.x + Vector2.up * v.y;
        }

    }
}