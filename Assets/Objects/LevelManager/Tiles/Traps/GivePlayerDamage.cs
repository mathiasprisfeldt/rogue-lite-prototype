using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePlayerDamage : MonoBehaviour
{
    [SerializeField]
    private float _damage;

    [SerializeField]
    private bool _DamageOverTime;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Do damage to player

        }
    }
}
