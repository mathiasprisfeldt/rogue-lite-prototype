using AcrylecSkeleton.Utilities;
using Archon.SwissArmyLib.Events;
using Archon.SwissArmyLib.Utils;
using CharacterController;
using Controllers;
using Projectiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifestealGem : Projectile
{
    [Header("Lifesteal")]
    [SerializeField]
    private bool _spin;

    [SerializeField]
    private float _rotSpeed = 90;

    [SerializeField]
    private float _respawnTime = 5;

    private SpriteRenderer _renderer;
    private CircleCollider2D _collider;

    public int index { get; set; }
    public Lifesteal LOwner { get; set; }

    private void Start()
    {
        _rotSpeed *= Random.value > .5f ? 1 : -1;
        _collider = GetComponent<CircleCollider2D>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public override void Update()
    {
        base.Update();

        transform.Rotate(new Vector3(0, 0, _rotSpeed * BetterTime.DeltaTime));
    }

    public override void Shoot()
    {
        if (Owner is ActionsController)
        {
            _tagsToHit = new List<string>() { "Enemy" };
            _layerToDestoryOn = new LayerMask();
        }
        else
        {
            _tagsToHit = new List<string>() { "Player" };
        }
    }

    protected override void Kill()
    {
        TellMeWhen.Seconds(_respawnTime, this);
        _collider.enabled = false;
        _renderer.enabled = false;
        _used = false;
    }

    public override void OnTimesUp(int id, object args)
    {
        if (this)
        {
            _collider.enabled = true;
            _renderer.enabled = true;
        }
    }
}
