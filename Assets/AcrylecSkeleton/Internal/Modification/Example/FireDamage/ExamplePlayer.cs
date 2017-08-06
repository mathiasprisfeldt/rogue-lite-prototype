using UnityEngine;

namespace AcrylecSkeleton.ModificationSystem.Examples
{
    [RequireComponent(typeof(ModificationHandler))]
    public class ExamplePlayer : MonoBehaviour {

        private StatsExample _stats;
        private ModificationHandler _modificationHandler;

        // Use this for initialization
        void Start()
        {
            _modificationHandler = GetComponent<ModificationHandler>();

            //Stats is initialized and values are set
            _stats = new StatsExample();
            _stats.Damage = 30;
            _stats.Health = 100;
            _stats.Speed = 20;

            _modificationHandler.AddModification(new FireDamageModification(4, 10, Modification.ModificationTypeEnum.Timed, _stats));
            _modificationHandler.AddModification(new MovementSpeedModification(5, 30, Modification.ModificationTypeEnum.Timed, _stats));
            _modificationHandler.AddModification(new DamageBoostModification(0, 40, Modification.ModificationTypeEnum.Infinite, _stats));
        }
    }
}

public class StatsExample
{
    public float Health { get; set; }
    public float Damage { get; set; }
    public float Speed { get; set; }
}
