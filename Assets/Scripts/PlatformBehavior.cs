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
    public bool Left { get; set; }
    public bool Right { get; set; }
    public bool IsTrap { get; set; }
    public bool IsGrabable { get; set; }
    public bool IsSlideable { get; set; }

    public void Awake()
    {
        Tiles = new List<TileBehaviour>();
    }
}
