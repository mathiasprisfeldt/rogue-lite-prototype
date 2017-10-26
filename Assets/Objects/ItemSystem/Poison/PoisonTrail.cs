using Assets.Objects.PlayerMovement.Player.Prefab.Player;
using CharacterController;
using Enemy;
using ItemSystem.Items;
using System.Collections;
using System.Collections.Generic;
using AcrylecSkeleton.ModificationSystem;
using AcrylecSkeleton.Utilities;
using UnityEngine;

public class PoisonTrail : MonoBehaviour
{
    [SerializeField]
    private ModificationHandler _modificationHandler;

    [SerializeField]
    private float _poisonInterval;

    [SerializeField]
    private Timer _timer;

    private ParticleSystem _ps;
    private float _origEmission = 0;

    public Poison Owner { get; set; }

    private LinkedList<GameObject> _objectTouched = new LinkedList<GameObject>();

    private void OnParticleCollision(GameObject other)
    {

        var ply = other.GetComponent<PlayerApplication>();
        var enm = other.GetComponent<EnemyApplication>();

        var valid = (other.tag == "Enemy" && Owner.State == ItemSystem.ItemState.Player && enm ||
                     other.tag == "player" && Owner.State == ItemSystem.ItemState.Enemy && ply) &&
                    !_objectTouched.Contains(other);
        if (valid)
        {
            if(ply)
                Owner.OnHit(ply.C.Character.HealthController);
            if(enm)
                Owner.OnHit(enm.M.Character.HealthController);
            if (_modificationHandler)
                _modificationHandler.AddModification(new RemoveFromList(other,ref _objectTouched,new Timer(_poisonInterval), other.name + " Remove from " + _objectTouched));
        }
           
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

    private class RemoveFromList : Modification
    {
        private readonly GameObject _gameObject;

        private LinkedList<GameObject> _list;

        public RemoveFromList(GameObject go, ref LinkedList<GameObject> list,Timer time, string name) : base(time, name)
        {
            _gameObject = go;
            _list = list;
            _list.AddLast(_gameObject);
        }

        public override void RemoveModificaiton()
        {
            base.RemoveModificaiton();
            if(_gameObject != null && _list != null && _list.Contains(_gameObject))
                _list.Remove(_gameObject);
        }
    }
}
