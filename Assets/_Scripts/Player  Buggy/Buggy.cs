using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Buggy : MonoBehaviour
{
	public int playerId = 0;
	
	[HideInInspector]
	public List<GameObject> checkpointsHit = new List<GameObject>();//all the checkpoints the player has touched during the current lap
	public int lapsCompleted = 0;
	public bool finishedRace = false;//whether or not they finished all their laps and are done racing
	public int raceStanding = 0;//1 is 1st place, 2 is 2nd, etc

	private Player player;

	private void Awake()
	{
		// Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
		player = ReInput.players.GetPlayer(playerId);
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	void FixedUpdate()
	{
		
	}
}
