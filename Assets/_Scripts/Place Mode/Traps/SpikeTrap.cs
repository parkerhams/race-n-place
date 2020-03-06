using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : AbstractTrapBehavior
{
	public GameObject[] shrapnel;//shrapnel are originally child objects of the trap, but are unparented when the trap explodes.
	//need to have a reference to all the shrapnel to remove them when the trap is removed/restored
	
	public float explosionRadius = 3f;
	public float explosionKnockback = 5f;
	
	public GameObject explodedCactus;
	public GameObject unexplodedCactus;
	
	protected override void Start()
	{
		//run Start() in the AbstractTrapBehavior first
		base.Start();
		hurtsFriendly = false;
		triggersFromFriendly = false;
		explodedCactus.SetActive(false);
		damageRadius = new Vector3(5f, 5f, 5f);//adjust this - temporary arbitrary value
		
		base.OnTriggerTrap += OnTriggerTrap;
		base.OnRestoreTrap += OnRestoreTrap;
	}
	
	private void OnTriggerTrap(GameObject trap)
	{
		//specific trap behaviors for spike trap go here - 
		//first, run the base TriggerTrap() function that all traps have, and run this
		trap = this.gameObject;
		Explode();
	}
	
	private void OnRestoreTrap(GameObject trap)
	{
		trap = this.gameObject;
		unexplodedCactus.SetActive(true);
		explodedCactus.SetActive(false);
		//restore shrapnel
		foreach(GameObject s in shrapnel)
		{
			Shrapnel shrapnelScript = s.GetComponent<Shrapnel>();
			shrapnelScript.ToggleActive(true);
			s.transform.position = shrapnelScript.startingPos;
			s.transform.rotation = shrapnelScript.startingRot;
			s.GetComponent<Rigidbody>().isKinematic = true;
			s.GetComponent<AbstractTrapBehavior>().isPlaced = false;
			s.transform.parent = this.transform;
		}
	}
	
	void Explode()
	{
		explodedCactus.SetActive(true);
		Debug.Log(explodedCactus);
		unexplodedCactus.SetActive(false);
		
		List<Buggy> buggies = buggiesInRadius(transform.position, explosionRadius);
		//deal damage to nearby buggies
		foreach(Buggy b in buggies)
		{
			b.TakeDamage(damage, owner);
			Debug.Log("hit by explosion" + damage);
			b.TakeKnockback(explosionKnockback, this.gameObject);
		}
		foreach(GameObject s in shrapnel)
		{
			s.GetComponent<Shrapnel>().LaunchOffTrap();
		}
	}
}
