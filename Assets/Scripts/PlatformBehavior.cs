using System.Collections.Generic;
using UnityEngine;


    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class PlatformBehavior : MonoBehaviour 
    {
        public List<TileBehaviour> Tiles { get; set; }
        public bool Istop { get; set; }

        public void Awake()
        {
            Tiles = new List<TileBehaviour>();
        }
    }
