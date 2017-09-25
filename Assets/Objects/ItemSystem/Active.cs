using ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Health;

namespace ItemSystem
{
    public abstract class Active : Item
    {
        public abstract void OnTrigger();
    }
}
