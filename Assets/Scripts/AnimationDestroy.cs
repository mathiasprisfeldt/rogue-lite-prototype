using UnityEngine;

namespace AnimationDestroy
{
    /// <summary>
    /// Purpose:
    /// Creator:
    /// </summary>
    public class AnimationDestroy : MonoBehaviour 
    {
        public void DestroyObject()
        {
            Destroy(gameObject);
        }
    }
}