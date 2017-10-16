using ItemSystem.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonTrail : MonoBehaviour
{
	private ParticleSystem _ps;

	public Poison Owner { get; set; }

	private void OnParticleCollision(GameObject other)
	{
		//Make the other take damage
	}

	public void StartEmmision()
	{
		if (!_ps)
			_ps = GetComponent<ParticleSystem>();
	}

	public void StopEmmision()
	{
		if (!_ps)
			_ps = GetComponent<ParticleSystem>();
	}
}
