using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCheckpoints : MonoBehaviour
{
	public int checkpointNumber = 0;
	//0 is the finish line, 1 through GameManager.Instance.checkpointLast are the checkpoints
	//if a map has a finish line and 3 checkpoints, the "checkpointLast" in game manager is 3
	//when a buggy hits a checkpoint it's added to their list of checkpoints in Buggy script
	//0 (the finish line) can't be counted as being hit until all (or most?) other checkpoints have been hit
	//the buggy's checkpoint list is emptied once they hit 0 and complete the lap
	//we can use either HitAllCheckpoints or HitMostCheckpoints depending on if we want the player to have to hit every checkpoint to be allowed to complete a lap
	
	// OnTriggerEnter is called when the Collider other enters the trigger.
	protected void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			Buggy buggyScript = other.gameObject.GetComponent<Buggy>();
			if(checkpointNumber == 0)//if this is the finish line
			{
				if(HitMostCheckpoints(buggyScript, 1))
				{
					GameManager.Instance.LapComplete(buggyScript);
				}
			}
			else //regular checkpoint
			{
				HitCheckpoint(buggyScript);
			}
			
		}
	}
	
	void HitCheckpoint(Buggy buggyScript)//add this checkpoint to the buggy's list of checkpoints hit
	{
		if(!buggyScript.checkpointsHit.Contains(this.gameObject))
		{
			buggyScript.checkpointsHit.Add(this.gameObject);
			Debug.Log("Player " + buggyScript.playerId + " hit checkpoint " + checkpointNumber);
		}
		else
		{
			//player is going backwards - give them some kind of "Wrong way!" text overhead?
			Debug.Log("Player " + buggyScript.playerId + " is going the wrong way!");
		}
	}
	
	void LapComplete(Buggy buggyScript)//this buggy completed a lap
	{
		Debug.Log("Player " + buggyScript.playerId + " completed a lap!");
		buggyScript.checkpointsHit = null;
		
		
	}
	
	bool HitAllCheckpoints(Buggy buggyScript)//check if they hit every single checkpoint (except the finish line, they're hitting that right now)
	{
		if(GameManager.Instance.checkpoints.Count-1 <= buggyScript.checkpointsHit.Count)//Count-1 to exclude the finish line they haven't hit until just now
		{
			return true;
		}
		else return false;
	}
	bool HitMostCheckpoints(Buggy buggyScript, int leniency)//if they hit all but (leniency number) amount of checkpoints
	{
		if(GameManager.Instance.checkpoints.Count - (leniency+1) <= buggyScript.checkpointsHit.Count)
		{
			return true;
		}
		else return false;
	}
	
	
    // Start is called before the first frame update
    void Start()
    {
	    GameManager.Instance.LoadMapCheckpoint(this.gameObject);//each checkpoint reports its existence to the game manager when they are loaded
	    //if all maps are shown in one single scene, this will need to be changed to avoid bugs
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
