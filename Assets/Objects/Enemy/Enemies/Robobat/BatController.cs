using Controllers;
using Enemy;
using ItemSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : EnemyController
{
    [SerializeField]
    private List<Item> _randomItems = new List<Item>();
    [SerializeField]
    private Collider2D _groundCollider;

    protected override void Awake()
    {
        base.Awake();

        GetComponent<Character>().ItemHandler.ItemsAtStart.Add(
            _randomItems[Random.Range(0, _randomItems.Count)]);
    }

    protected override void OnDead()
    {
        base.OnDead();

        App.M.Character.Rigidbody.gravityScale = 1;

        _groundCollider.enabled = true;
    }
}
