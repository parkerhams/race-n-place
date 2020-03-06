using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scrap : MonoBehaviour
{
	bool isAvailable = true;//scrap is available if a buggy hasn't picked it up yet during a race
	
	public int value = 1;//how many points of scrap they get for picking it up
	
	public GameObject buggyToIgnore;
	
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	// OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
	protected void OnCollisionEnter(Collision collisionInfo)
	{
		if(collisionInfo.gameObject == buggyToIgnore)
		{
			Physics.IgnoreCollision(collisionInfo.gameObject.GetComponent<Buggy>().chassis.GetComponent<BoxCollider>(), this.gameObject.GetComponent<BoxCollider>());
		}
	}
	
    
	// OnTriggerEnter is called when the Collider other enters the trigger.
	protected void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			Buggy buggyScript = other.gameObject.GetComponentInParent<Buggy>();
			if(buggyScript.hasExploded)
			{
				return;
			}
			buggyScript.PickupScrap(value);
			if(GameManager.Instance.excessScrap.Contains(this.gameObject))
			{
				GameManager.Instance.excessScrap.Remove(this.gameObject);
			}
			gameObject.SetActive(false);
			
		}
	}
	
	
	public void RestockScrap(bool activating)
	{
		if(activating)
		{
			gameObject.SetActive(true);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
	
	void DisableUntilNextRace()
	{
		gameObject.SetActive(false);
	}
}
