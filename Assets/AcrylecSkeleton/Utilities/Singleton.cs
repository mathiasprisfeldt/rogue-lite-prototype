﻿using System;
using UnityEngine;

namespace AcrylecSkeleton.Utilities
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool _isQuitting;

        public bool DontDestroyOnLoadConfig;

        private static T instance;

        /**
       Returns the instance of this singleton.
        */
        public static T Instance
        {
            get
            {
                if (_isQuitting)
                    return null;

                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                    {
                        GameObject newInstance = new GameObject(typeof(T).ToString());
                        instance = newInstance.AddComponent<T>();

                        Debug.LogWarning(String.Format("[SINGLETON] An instance of {0} is needed, instantiating {1} to compensate.", typeof(T), newInstance));

                        DontDestroyOnLoad(newInstance);

                        return instance;
                    }
                }

                return instance;
            }
        }

        /// <summary>
        /// Check if the instance has been instantiated.
        /// </summary>
        /// <returns>True if instance is not null</returns>
        public static bool CheckSanity()
        {
            if (Instance != null)
                return true;

            return false;
        }

        [ExecuteInEditMode]
        protected virtual void Awake()
        {
            if (DontDestroyOnLoadConfig)
                DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }
    }
}