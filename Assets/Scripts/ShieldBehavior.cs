using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using ItemSystem;
using ItemSystem.Items;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    public Electric Owner { get; set; }

	// Use this for initialization
	void Start ()
    {
        if (Owner)
        {
            Owner.ItemHandler.Owner.HealthController.OnNonDamage.AddListener(OnNonDamage);
            Owner.ItemHandler.Owner.HealthController.OnDamage.AddListener(OnDamage);
        }
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    private void OnDamage(Character arg0)
    {
        _animator.SetTrigger("Destroy");

    }

    private void OnNonDamage(Character arg0)
    {
        _animator.SetTrigger("Destroy");
    }
}
