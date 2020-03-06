using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrapnel : MonoBehaviour
{
	Rigidbody rb;
	GameObject trap;//the trap this shrapnel belonged to
	Vector3 explodeDistance = new Vector3(10,0,0);
	public Vector3 startingPos;
	public Quaternion startingRot;
	
	public float lifetime = 4f;
	public float damage = 1f;
	public bool hasLaunched = false;
	
	Vector3 normalScale;
	
	void OnEnable()
	{
		GameManager.ModeSwapped += StopAllCoroutines;
	}
	void OnDisable()
	{
		GameManager.ModeSwapped -= StopAllCoroutines;
	}
	
	
	//TODO: disable colliders until cactus launches
	protected void Start()
	{
		rb = GetComponent<Rigidbody>();
		trap = transform.parent.gameObject;
		//StartCoroutine(TestCoroutine());
		normalScale = transform.localScale;
	}
	public void LaunchOffTrap()
	{
		startingPos = transform.position;
		startingRot = transform.rotation;
		this.transform.parent = null;
		rb.isKinematic = false;
		hasLaunched = true;
		rb.AddForce(transform.forward * 10, ForceMode.Impulse);
		GetComponent<AbstractTrapBehavior>().isPlaced = true;
		GetComponent<AbstractTrapBehavior>().readyToTrigger = true;
		StartCoroutine(FadeOutCoroutine());
		//GetComponent<BoxCollider>().isTrigger = false;
	}
	
	
	IEnumerator TestCoroutine()
	{
		yield return new WaitForSeconds(2f);
		LaunchOffTrap();
	}
	
	public IEnumerator FadeOutCoroutine()
	{
		yield return new WaitForSeconds(lifetime);
		ToggleActive(false);
	}
	
	public void ToggleActive(bool active)
	{
		if(active)
		{
			transform.localScale = normalScale;
			hasLaunched = false;
			GetComponent<BoxCollider>().enabled = true;
		}
		
		else
		{
			transform.localScale = new Vector3(0,0,0);
			GetComponent<BoxCollider>().enabled = false;
		}
	}
	
	private void OnTriggerTrap(GameObject trap)
	{
		trap = this.gameObject;
		
	}
	
	// OnTriggerEnter is called when the Collider other enters the trigger.
	protected void OnTriggerEnter(Collider collisionInfo)
	{
		if(!hasLaunched) {return;}
		if(collisionInfo.gameObject.tag == "Player")
		{
			Debug.Log("hit by shrap" + damage);
			Buggy buggyScript = collisionInfo.gameObject.GetComponentInParent<Buggy>();
			buggyScript.TakeDamage(damage, trap.GetComponent<AbstractTrapBehavior>().owner);
			ToggleActive(false);
		}
	}
}
