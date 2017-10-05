using ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Health;
using Archon.SwissArmyLib.Utils;
using CharacterController;
using System.Linq;
using InControl;

public class Lifesteal : Item
{
    [SerializeField]
    private float _stealAmount;

    [SerializeField]
    private GameObject _gemPrefab;

    [SerializeField]
    protected int _gemCount;

    [SerializeField]
    private float _gemRadius;

    [SerializeField]
    [Tooltip("Degrees per second")]
    private float _gemSpeed = 10;

    private Dictionary<int, LifestealGem> _gems = new Dictionary<int, LifestealGem>();

    public override void OnHit(HealthController victim)
    {
        base.OnHit(victim);        

        ItemHandler.Owner.HealthController.Heal(victim.LastDamageRcieved);
    }

    private void Update()
    {
        if(Other && _slaveState == ItemSlave.Master)
            transform.Rotate(new Vector3(0, 0, _gemSpeed * BetterTime.DeltaTime));        
    }

    protected override void DoubleUp()
    {
        base.DoubleUp();

        float angle = 360 / _gemCount;
        for (int i = 0; i < _gemCount; i++)
        {
            _gems.Add(i, Instantiate(_gemPrefab,
                PlaceOnCircle(transform.position, _gemRadius, angle * i),
                Quaternion.identity, transform).GetComponent<LifestealGem>());
        }

        for (int i = 0; i < _gems.Count; i++)
        {
            _gems[i].Owner = ItemHandler.Owner;
            _gems[i].Shoot();
            _gems[i].LOwner = this;
        }

        ItemHandler.Owner.HealthController.OnDead.AddListener(OnDead);
    }

    private Vector3 PlaceOnCircle(Vector3 center, float radius, float ang)
    {
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }

    protected override void DoubleDown()
    {
        base.DoubleDown();

        foreach (var item in _gems)
        {
            item.Value.Remove();
        }

        _gems.Clear();
    }

    private void OnDead()
    {
        DoubleDown();
    }

    protected override void OnDestroy()
    {
        ItemHandler.Owner.HealthController.OnDead.RemoveListener(OnDead);

        base.OnDestroy();
    }
}
