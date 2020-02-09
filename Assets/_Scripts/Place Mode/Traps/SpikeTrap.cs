using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
	public bool isPlaced = false;
	bool readyToTrigger = true;
	public float cooldownTime = 2f;//how long before the trap can trigger again
	public bool hurtsFriendly = false;//whether or not the trap can hurt friendly targets (owner)
	public bool triggersFromFriendly = false;//whether or not the trap can be triggered by friendly targets
	public bool continuouslyTriggers = false;//traps that keep trying to trigger as long as a player is inside
	
	List<GameObject> playersInTrapTriggerRadius = new List<GameObject>();//if traps continuously trigger, they need to know if there are players inside the trigger radius
	
	GameObject owner;//the person who placed the trap
	int ownerPlayerId;
	
	
    // Start is called before the first frame update
    void Start()
    {
	    if(owner = null)
	    {
	    	owner = GameManager.Instance.gameObject;
	    }
    }

    // Update is called once per frame
    void Update()
    {
	    if(playersInTrapTriggerRadius != null && continuouslyTriggers)
	    {
	    	foreach(GameObject p in playersInTrapTriggerRadius)
	    	{
	    		TriggerTrap(p);
	    	}
	    }
    }
    
	// OnTriggerEnter is called when the Collider other enters the trigger.
	protected void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			TriggerTrap(other.gameObject);
			playersInTrapTriggerRadius.Add(other.gameObject);
		}
	}
	
	// OnTriggerExit is called when the Collider other has stopped touching the trigger.
	protected void OnTriggerExit(Collider other)
	{
		if(other.tag == "Player")
		{
			playersInTrapTriggerRadius.Remove(other.gameObject);
		}
	}
	
	void TriggerTrap(GameObject thePlayer)
	{
		Debug.Log("Trap attempting to trigger");
		if(GameManager.Instance.mode == GameManager.Mode.Place)
		{
			//can't trigger traps during place mode?
			return;
		}
		Debug.Log("a");
		if(!isPlaced)
		{
			return;
		}
		Debug.Log("b");
		if(!readyToTrigger)
		{
			return;
		}
		Debug.Log("c");
		if(thePlayer == owner && !triggersFromFriendly)//TODO: change to check ownerPlayerId against the player's id in their kart script once it's added
		{
			return;
		}
		
		//trigger the trap
		readyToTrigger = false;
		Debug.Log("Trap triggered!");
		TemporaryTrapTriggerEffect();
		StartCoroutine(TrapResetCooldownCoroutine());
		
	}
	
	void TemporaryTrapTriggerEffect()
	{
		transform.localScale += new Vector3(0,4,0);
		StartCoroutine(TemporaryCoroutine());
	}
	
	IEnumerator TemporaryCoroutine()
	{
		yield return new WaitForSeconds(cooldownTime);
		transform.localScale -= new Vector3(0,4,0);
	}
	
	IEnumerator TrapResetCooldownCoroutine()
	{
		yield return new WaitForSeconds(cooldownTime);
		readyToTrigger = true;
	}
	
}
