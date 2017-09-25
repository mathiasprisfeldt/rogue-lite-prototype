using Health;

namespace ItemSystem.Items
{
    /// <summary>
    /// Purpose:
    /// Creator: MP
    /// </summary>
    public class Poison : Item 
    {
        public override void OnHit(HealthController victim)
        {
            victim.Damage(5, from: ItemHandler.Owner, triggerItemhandler: false);
        }
    }
}