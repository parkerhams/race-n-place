using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the general placement bheaviors that the traps all follow
public class AbstractTrapBehavior : MonoBehaviour
{
	public delegate void TriggerTrapBehavior(GameObject trap);
	public event TriggerTrapBehavior OnTriggerTrap; 
	public event TriggerTrapBehavior OnRestoreTrap;
	
	public bool isPlaced = false;
	public Vector3 placedPosition;
	public bool readyToTrigger = true;
	public float cooldownTime = 2f;//how long before the trap can trigger again
	public bool hurtsFriendly = false;//whether or not the trap can hurt friendly targets (owner)
	public bool triggersFromFriendly = false;//whether or not the trap can be triggered by friendly targets
	public bool continuouslyTriggers = false;//traps that keep trying to trigger as long as a player is inside
	public float damage = 1;//how much damage the trap directly deals to victims when triggered
	public bool despawnsAfterTrigger = false;//for shrapnel and similar objects that permanen
	
	public bool breaksUntilRaceEnd = false;//traps that can only be triggered once per race
	public bool isBroken = false;//broken traps can't trigger at all
	
	List<GameObject> playersHitRecently = new List<GameObject>();//all the players that this trap recently hit
	public float hitSamePlayerCooldown = 2f;//when a player is hit by a trap, they're added to the trap's playersHitRecently for hitSamePlayerCooldown duration, then removed from the list
	//traps can't hit players in the playersHitRecently list
	
	public Vector3 damageRadius = new Vector3(1,1,1);//the area around the trap that players are affected
	
	List<GameObject> playersInTrapTriggerRadius = new List<GameObject>();//if traps continuously trigger, they need to know if there are players inside the trigger radius
	
	public GameObject owner;//the person who placed the trap
	
	// Start is called before the first frame update
	protected virtual void Start()
	{
		
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
	
	void OnEnable()
	{
		GameManager.ModeSwapped += RestoreTrap;
	}
	void OnDisable()
	{
		GameManager.ModeSwapped -= RestoreTrap;
	}
	
	protected void RestoreTrap()
	{
		if(!breaksUntilRaceEnd)
		{
			return;//trap doesn't break so it never needs to be restored
		}
		if(!isBroken)
		{
			return;//trap didn't break
		}
		else
		{
			//set it to its placedPosition
			if(placedPosition != null) {transform.position = placedPosition;}
			if(OnRestoreTrap != null)
			{
				OnRestoreTrap(this.gameObject);
			}
			//restore variables
			isBroken = false;
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
	
	public List<Buggy> buggiesInRadius(Vector3 center, float radius)
	{
		Collider[] hitColliders = Physics.OverlapSphere(center, radius);
		int i=0;
		List<Buggy> buggies = new List<Buggy>();
		while(i<hitColliders.Length)
		{
			if(hitColliders[i].gameObject.tag == "Player")
			{
				Buggy buggyScript = hitColliders[i].gameObject.GetComponentInParent<Buggy>();
				buggies.Add(buggyScript);
			}
			i++;
		}
		return buggies;
	}
	
	public virtual void TriggerTrap(GameObject thePlayer)
	{
		
		if(GameManager.Instance.mode == GameManager.Mode.Place)
		{
			//can't trigger traps during place mode?
			//return;
		}
		if(!isPlaced)
		{
			return;
		}
		if(!readyToTrigger || isBroken)
		{
			return;
		}
		if(thePlayer == owner && !triggersFromFriendly)//TODO: change to check ownerPlayerId against the player's id in their kart script once it's added
		{
			return;
		}
		
		//trigger the trap
		
		if(OnTriggerTrap != null)
			OnTriggerTrap(this.gameObject);
		
		readyToTrigger = false;
		if(breaksUntilRaceEnd)
		{ 
			isBroken = true;
		}
		//TemporaryTrapTriggerEffect();
		StartCoroutine(TrapResetCooldownCoroutine());
		
	}
	
	public void TemporaryTrapTriggerEffect()
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
