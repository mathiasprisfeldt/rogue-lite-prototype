using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AcrylecSkeleton.Utilities
{
    public static class MathUtils
    {
        public const float DegreeToRadian = (float)(Math.PI / 180.0);
        public const float RadianToDegree = (float)(180.0 / Math.PI);

        /// <summary>
        /// Lerp between two values with a given step
        /// </summary>
        /// <param name="from">The starting value</param>
        /// <param name="to">The end value</param>
        /// <param name="maxDelta">The max step allowed</param>
        /// <returns></returns>
        public static float Lerp(float from, float to, float maxDelta)
        {
            return @from + (to - @from) * Clamp01(maxDelta);
        }

        /// <summary>
        /// Clamps the value between 0 and 1 value
        /// </summary>
        /// <param name="value">The value you want to clamp</param>
        /// <returns>Returns the clamped value</returns>
        public static float Clamp01(float value)
        {
            return Math.Min(Math.Max(value, 0), 1);
        }

        public static Vector2 DegreesToVector(float degree)
        {
            return RadiansToVector(degree * DegreeToRadian);
        }

        public static float VectorToDegrees(Vector2 direction)
        {
            return VectorToRadians(direction) * RadianToDegree;
        }

        public static Vector2 RadiansToVector(float rad)
        {
            rad -= (float)Math.PI * 0.5f;
            return new Vector2((float)Math.Cos(rad), (float)Math.Sin(rad));
        }

        public static float VectorToRadians(Vector2 direction)
        {
            return (float)(Math.Atan2(direction.y, direction.x) + Math.PI * 0.5f);
        }

        public static List<Vector2> FindCirclePoints(int points, double radius, Vector2 center)
        {
            var pointList = new List<Vector2>();
            var degrees = 360 / points;

            for (var i = 0; i < points; i++)
            {
                var angle = (Math.PI / 180.0) * Wrap(0, 360, (degrees * i));
                var newX = (int)(center.x + radius * Math.Cos(angle));
                var newY = (int)(center.y + radius * Math.Sin(angle));
                pointList.Add(new Vector2(newX, newY));
            }

            return pointList;
        }

        public static int Wrap(int min, int max, int value)
        {
            var rangeSize = max - min + 1;

            if (rangeSize == 0)
                return min;

            if (value < min)
                value += rangeSize * ((min - value) / rangeSize + 1);

            return min + (value - min) % rangeSize;
        }

        /// <summary>
        /// Lerps between two colors
        /// </summary>
        /// <param name="a">currenat</param>
        /// <param name="b">End</param>
        /// <param name="Step">Step</param>
        /// <returns></returns>
        public static Color LerpColor(Color a, Color b, float Step)
        {
            var newR = Lerp(a.r, b.r, Step);
            var newG = Lerp(a.g, b.g, Step);
            var newB = Lerp(a.b, b.b, Step);
            Color newColor = new Color(newR, newG, newB);

            return newColor;
        }

        public static void FisherYatesShuffle<T>(this List<T> list)
        {
            List<KeyValuePair<float, T>> preList = new List<KeyValuePair<float, T>>(list.Count); //New list with float index.
            list.ForEach(obj => preList.Add(new KeyValuePair<float, T>(Random.Range(0f, 1f), obj))); //Give a random index for each index in list to shuffle.
            var sorted = from item in preList orderby item.Key select item; //Sort the list by the given index.

            List<T> returnList = new List<T>(); //Create a list with only values.
            sorted.ToList().ForEach(pair => returnList.Add(pair.Value)); //Add the values from sort list to the new list.

            list.Clear(); //Clear unsorted list.
            list.AddRange(returnList); //Replace with shuffled values.
        }

        public static T GetRandomEntry<T>(this List<T> list)
        {
            if (!list.Any())
                return default(T);

            list.FisherYatesShuffle();
            return list[Random.Range(0,list.Count)];
        }

        public static Vector2 RotateVector2Degree(Vector2 vector, float degree)
        {
            var direction = VectorToDegrees(vector);
            direction += degree;
            return DegreesToVector(direction);
        }

        /// <summary>
        /// Rounds to nearest ratio
        /// Ex. if nearest is 2 its each halfs.
        ///     1.6 = 1.5
        ///     1.7 = 1.5
        ///     1.8 = 2
        ///     1.9 = 2
        ///     Etc
        /// </summary>
        /// <param name="number">Number to round</param>
        /// <param name="nearest">Number to round to</param>
        /// <returns></returns>
        public static float RoundToNearest(float number, float nearest)
        {
            return Mathf.Round(number * nearest) / nearest;
        }

        public static bool FastApproximately(this float a, float b, float tolerance = 0)
        {
            return Math.Abs(a - b) <= tolerance;
        }
    }
}