using System;
using System.Collections;
using System.Diagnostics;
using AcrylecSkeleton.Managers;
using UnityEngine;

namespace AcrylecSkeleton.Utilities
{
    public static class Utils
    {
        public static IEnumerator WaitForExecution(Action action, float timeToWait, bool ignorePause = false)
        {
            float timer = 0;

            while (timer < 1)
            {
                yield return new WaitForEndOfFrame();

                if (!ignorePause)
                {
                    if (!TimeManager.Instance.IsPaused)
                        timer += Time.unscaledDeltaTime / timeToWait;
                }
                else
                    timer += Time.unscaledDeltaTime / timeToWait;
            }

            action();
        }

        /// <summary>
        /// Checks if a layer mask contains another layer mask.
        /// </summary>
        /// <param name="this">The mask that contains</param>
        /// <param name="other">The mask to check against</param>
        /// <returns></returns>
        public static bool Contains(this LayerMask @this, LayerMask other)
        {
            return @this == (@this | (1 << other.value));
        }

        /// <summary>
        /// Searches after a specific component in its parents until is finds a match or dead end.
        /// </summary>
        /// <typeparam name="T">Type of component</typeparam>
        /// <param name="go">Gameobject to start the search on</param>
        public static T GetComponentUp<T>(this GameObject go) where T : MonoBehaviour
        {
            Transform parent = go.transform;
            T foundComp = null;

            while (parent)
            {
                foundComp = parent.GetComponentInChildren<T>();

                if (!foundComp)
                    parent = parent.parent;
                else
                    break;
            }

            return foundComp;
        }
    }
}

