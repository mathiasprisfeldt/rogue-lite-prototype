using Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePlayerDamage : MonoBehaviour
{
    [SerializeField]
    private float _damage;

    [SerializeField]
    private bool _DamageOverTime;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            var h = collision.GetComponent<CollisionCheck>();
            if (h)
                h.Character.HealthController.Damage(_damage);
        }
    }
}
