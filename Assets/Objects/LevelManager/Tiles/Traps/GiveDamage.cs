using Controllers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AcrylecSkeleton.Utilities;
using AcrylecSkeleton.Utilities.Collections;
using UnityEngine;

public class GiveDamage : MonoBehaviour
{
    [SerializeField]
    private float _damage;

    [SerializeField]
    private bool _DamageOverTime;

    [SerializeField]
    private bool _autoRotate;

    [SerializeField]
    private TileBehaviour _tileBehavior;

    [SerializeField]
    private LayerMask _layersMask;

    [SerializeField]
    private Tags _tags;

    [SerializeField]
    private bool _safetyRespawn;

    private bool _hasRotated;

    public void Update()
    {
        if (_tileBehavior && _tileBehavior.SetupDone && !_hasRotated && _autoRotate)
        {
            if (_tileBehavior.BottomCollision && !_tileBehavior.BottomTile.IsTrap)
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(0, 1) * Mathf.Rad2Deg, Vector3.forward);
            else if (_tileBehavior.TopCollision && !_tileBehavior.TopTile.IsTrap)
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(0,-1) * Mathf.Rad2Deg,Vector3.forward);
            else if (_tileBehavior.LeftCollision && !_tileBehavior.LeftTile.IsTrap)
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(-1, 0) * Mathf.Rad2Deg, Vector3.forward);
            else if (_tileBehavior.RightCollision && !_tileBehavior.RightTile.IsTrap)
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(1, 0) * Mathf.Rad2Deg, Vector3.forward);
            _hasRotated = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_tags.Contains(collision.gameObject) && _layersMask.Contains(collision.gameObject.layer))
        {
            var h = collision.GetComponent<CollisionCheck>();
            if (h)
            {
                if (h.Character.HealthController && !h.Character.HealthController.TrapImmune)
                {
                    h.Character.HealthController.Damage(_damage, true, transform.position);
                    if (_safetyRespawn && h.Character.SafetyRespawn)
                    {
                        h.Character.SafetyRespawn.Respawn();
                    }
                }
            }
        }
    }
}
