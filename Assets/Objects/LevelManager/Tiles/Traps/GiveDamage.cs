using AcrylecSkeleton.Utilities;
using AcrylecSkeleton.Utilities.Collections;
using Health;
using UnityEngine;

public class GiveDamage : MonoBehaviour
{
    [SerializeField]
    private bool _dontKill;

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
        if (_tags.Contains(collision.gameObject) || _layersMask.Contains(collision.gameObject.layer))
        {
            var collisionCheck = collision.GetComponent<CollisionCheck>();
            if (collisionCheck)
            {
                HealthController healthController = collisionCheck.Character.HealthController;

                if (healthController && !healthController.TrapImmune)
                {
                    float dmgToDeal = _damage;

                    if (_dontKill && healthController.WouldKill(_damage))
                        dmgToDeal = 0;

                    healthController.Damage(dmgToDeal, true, transform.position);

                    if (_safetyRespawn && collisionCheck.Character.SafetyRespawn)
                    {
                        collisionCheck.Character.SafetyRespawn.Respawn();
                    }
                }
            }
        }
    }
}
