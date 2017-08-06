using System;
using System.Collections;
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
    }
}

