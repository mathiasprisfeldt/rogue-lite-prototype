using UnityEngine;

namespace DontDestoryOnLoad
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class DontDestoryOnLoad : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}