using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using CharacterController;
using Enemy;
using ItemSystem.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTrail : MonoBehaviour
{
    private ParticleSystem _ps;
    private float _origEmission = 0;

    public Poison Owner { get; set; }

    private void OnParticleCollision(GameObject other)
    {

        var ply = other.GetComponent<PlayerApplication>();
        var enm = other.GetComponent<EnemyApplication>();


        if (other.tag == "Enemy" && Owner.State == ItemSystem.ItemState.Player && enm)
            Owner.OnHit(enm.M.Character.HealthController);

        if (other.tag == "player" && Owner.State == ItemSystem.ItemState.Enemy && ply)
            Owner.OnHit(ply.C.Character.HealthController);
    }

    public void StartEmmision()
    {
        if (!_ps)
            _ps = GetComponent<ParticleSystem>();

        var ps = _ps.emission;

        if (_origEmission == 0)
            _origEmission = ps.rateOverDistance.constant;

        ps.rateOverDistance = _origEmission;
    }

    public void StopEmmision()
    {
        if (!_ps)
            _ps = GetComponent<ParticleSystem>();

        var ps = _ps.emission;

        if (_origEmission == 0)
            _origEmission = ps.rateOverDistance.constant;

        ps.rateOverDistance = 0;
    }
}
