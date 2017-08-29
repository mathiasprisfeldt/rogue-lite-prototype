using Controllers;
using System.Collections;
using System.Collections.Generic;
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
    private List<string> _tags = new List<string>();

    private bool _hasRotated;

    public void Update()
    {
        if (_tileBehavior && _tileBehavior.SetupDone && !_hasRotated && _autoRotate)
        {
            if (_tileBehavior.BottomCollision)
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(0, 1) * Mathf.Rad2Deg, Vector3.forward);
            else if (_tileBehavior.TopCollision)
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(0,-1) * Mathf.Rad2Deg,Vector3.forward);
            else if (_tileBehavior.LeftCollision)
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(-1, 0) * Mathf.Rad2Deg, Vector3.forward);
            else if (_tileBehavior.RightCollision)
                transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(1, 0) * Mathf.Rad2Deg, Vector3.forward);
            _hasRotated = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_tags.Contains(collision.gameObject.tag) && _layersMask == (_layersMask | (1 << collision.gameObject.layer)))
        {
            var h = collision.GetComponent<CollisionCheck>();
            if (h)
                h.Character.HealthController.Damage(_damage,true,transform.position);
        }
    }
}
