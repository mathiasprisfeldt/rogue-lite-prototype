using Controllers;
using Health;
using UnityEngine;

namespace ItemSystem.Items
{
    /// <summary>
    /// Purpose:
    /// Creator: MP
    /// </summary>
    public class Poison : Item
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Activate();
            }
        }
    }
}