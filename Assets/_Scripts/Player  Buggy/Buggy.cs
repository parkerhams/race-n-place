using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Buggy : MonoBehaviour
{
	public int playerId = 0;
	
	
	GameObject myGenericPlayer;//the generic player this buggy belongs will hold all the persistent information about the player (race standing, laps completed, scrap, etc)
	public PlayerControls playerControlsScript;//the generic player script

	public Player player;
	
	[HideInInspector]
	public float durability = 10f;//amount of hits they can take before exploding
	public float maxDurability = 10f;//their durability is reset to this when they respawn
	public bool hasExploded = false;//prevent the buggy from re-exploding while waiting to respawn
	Vector3 lastValidPosition;//where to put the buggy when they respawn
	bool isInvulnerable = false;
	float respawnDelay = 2f;//how long before the buggy respawns after exploding
	float respawnInvulnDuration = 2f;//how long the player is invuln after respawning
	public GameObject tempExplosion;
	public GameObject explosionA;
	GameObject playerLastDamagedBy;//the player who most recently damage to this buggy via traps
	public bool inOil = false;
	
	float turn;
	float acceleration;
	float brakePressure;
	
	[Header("Driving Variables")]
	public float torque = 100f;
	public float reverseTorque = 75f;
	public float brakeForce = 75f;
	public float maxRPM = 100f;
	public float maxSteerAngle = 30f;
	
	[Header("Vehicle Substeps")]
	public int speedThreshold;
	public int substepsBelow;
	public int substepsOver;
	
	[Header("Buggy Sub-Objects")]
	public WheelCollider[] wheelColliders;
	public Transform[] wheelMeshTransforms;
	public Transform CenterOfMass;
	private Rigidbody buggyRigidbody;
	public GameObject chassis;

	private void Awake()
	{
		// Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
		//player = ReInput.players.GetPlayer(playerId);
		//foreach(GameObject g in GameManager.Instance.genericPlayers)//find the player this buggy belongs to and assign it
		//{
		//	if(g.GetComponent<PlayerControls>().playerId == playerId)
		//	{
		//		myGenericPlayer = g;
		//		Debug.Log("Assigned buggy to player " + playerId);
		//	}
		//}
		//playerControlsScript = myGenericPlayer.GetComponent<PlayerControls>();
		
	}
	
    // Start is called before the first frame update
    void Start()
	{
		//playerControlsScript = myGenericPlayer.GetComponent<PlayerControls>();
		//Debug.Log("Player " + playerId + " buggy script assigned in start.");
		//Set custom center of mass (??)q
		buggyRigidbody = this.GetComponent<Rigidbody>();
		//buggyRigidbody.centerOfMass = CenterOfMass.position;
		
		foreach (var collider in wheelColliders)
		{
			collider.ConfigureVehicleSubsteps(speedThreshold, substepsBelow, substepsOver);
		}
		Respawn();
		GameManager.Instance.buggyScripts.Add(this);
    }

    // Update is called once per frame
    void Update()
	{
		SetWheelPositions();
		GetInput();
	}
    
	void FixedUpdate()
	{
		UpsideDownCheck();
		Steer();
		Drive();
		Brake();
		Reverse();
	}
	
	void UpsideDownCheck()
	{
		Vector3 up = transform.TransformDirection(Vector3.up);
		if(Physics.Raycast(transform.position, up, 10))
		{

		}
		else
		{
			//Debug.Log("Upside down!");
		}
	}
	
	void GetInput()//grab all the inputs here so that functions that run in FixedUpdate are still reading input from Update to prevent losing player input during some frames
	{
		turn = player.GetAxis("Steer");
		if(GameManager.Instance.raceCountingDown)
		{
			return;//only let them move their wheels around before race has begun. maybe change this later so they can rev up their engines too
		}
		acceleration = player.GetAxis("Accelerate");
		brakePressure = player.GetAxis("Brake");
		if(player.GetButtonDown("DebugForceExplode"))
		{
			DebugForceExplode();
		}
	}
	
	//set each wheel to the position and rotation of its corresponding collider
	private void SetWheelPositions()
	{
		//iterate through both the wheel colliders and meshes
		for (int i = 0; i < 4; i++) 
		{
			Quaternion rotation;
			Vector3 position;
			wheelColliders[i].GetWorldPose(out position, out rotation);
			wheelMeshTransforms[i].transform.position = position;
			wheelMeshTransforms[i].transform.rotation = rotation;
		}
	}
	
	private void Steer()
	{
		//steer proportionally to steering stick
		//float turn = player.GetAxis("Steer");
		//Only steer with front two wheels
		for (int i = 0; i < 2; i++) 
		{
			if(inOil) return;
			wheelColliders[i].steerAngle = (turn * maxSteerAngle);
		}
		
	}
	
	private void Drive()
	{
		//float acceleration = player.GetAxis("Accelerate"); //read input
		
		//All Wheel Drive
		for (int i = 0; i < 4; i++)
		{
			//Accelerate according to the throttle trigger
			wheelColliders[i].motorTorque = (acceleration * torque);
			
			if (wheelColliders[i].rpm <= maxRPM)
			{
				//do something if at max rpm
			}
		}
		
		
	}
	
	private void Brake()
	{
		//Brake with the brake trigger
		//float brakePressure = player.GetAxis("Brake");
		//all wheel brake
		for (int i = 0; i < 4; i++) 
		{
			if(inOil) return;
			wheelColliders[i].brakeTorque = (brakePressure * brakeForce);
		}
		
		
	}
	
	private void Reverse()
	{
		if (player.GetButton("Reverse"))
		{
			for (int i = 0; i < 4; i++)
			{
				//Accelerate according to the throttle trigger
				wheelColliders[i].motorTorque = (-reverseTorque);
			}
		}
	}
	
	void ToggleDrivingControls(bool enabled)
	{
		if(enabled)
		{
			player.controllers.maps.SetMapsEnabled(true, 1);//enable driving controls
		}
		else
		{
			player.controllers.maps.SetMapsEnabled(false, 1);//disable driving controls
		}
	}
	
	#region Durability
	
	public void TakeDamage(float damage, GameObject source)
	{
		if(isInvulnerable || hasExploded)
		{
			return;
		}
		else
		{
			if(source)
			{
				if(source != playerControlsScript.gameObject)//the player being hit can't be considered the player who last damaged themselves
				{
					playerLastDamagedBy = source;
				}
			}
			durability -= damage;
			if(durability <= 0)
			{
				Explode();
			}
		}
	}
	public void TakeKnockback(float magnitude, GameObject knocker)
	{
		float massCounteraction = 2500f;
		//Vector3 dir = knocker.transform.position - transform.GetChild(0).transform.position;
		Vector3 dir = transform.GetChild(0).transform.position - knocker.transform.position;
		dir *= magnitude*massCounteraction;
		buggyRigidbody.AddForce(dir, ForceMode.Impulse);
	}
	
	void DebugForceExplode()//for testing purposes!
	{
		TakeDamage(durability, null);
	}
	
	void Explode()
	{
		if(hasExploded)
		{
			return;
		}
		else
		{
			if(playerLastDamagedBy)//if someone else hurt them before they exploded
			{
				playerLastDamagedBy.GetComponent<PlayerControls>().destructionPoints++;
			}
			hasExploded = true;
			ToggleDrivingControls(false);
			if(playerControlsScript.heldScrap > 0)
			{
				StartCoroutine(LaunchScrapCoroutine());
			}
			
			StartCoroutine(RespawnDelayCoroutine());
			
		}
	}
	
	IEnumerator InvulnDurationCoroutine(float duration)
	{
		isInvulnerable = true;
		yield return new WaitForSeconds(duration);
		isInvulnerable = false;
	}
	
	IEnumerator RespawnDelayCoroutine()
	{
		GameObject explosion = null;
		if(tempExplosion)//this is all just temporary stuff to show an explosion sprite
		{
			explosion = Instantiate(explosionA, transform.GetChild(0).position, Quaternion.identity);
			explosion.transform.parent = gameObject.transform;
		}
		yield return new WaitForSeconds(respawnDelay);
		if(tempExplosion)
		{
			Destroy(explosion);
		}
		Respawn();
	}
	
	public void Respawn()
	{
		hasExploded = false;
		ToggleDrivingControls(true);
		durability = maxDurability;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		StartCoroutine(InvulnDurationCoroutine(respawnInvulnDuration));
		if(lastValidPosition == new Vector3(0,0,0))
		{
			//return them to their latest checkpoint instead
			GameObject latestCheckpoint = LatestCheckpoint();
			//transform.position = latestCheckpoint.transform.position + new Vector3(0,0,latestCheckpoint.transform.position.z + 2 - playerId*2.1f);
			transform.position = latestCheckpoint.transform.GetChild(playerId).position;
			transform.rotation = latestCheckpoint.transform.rotation;
			
			
			//TODO: zero out buggy velocity
			//temporary adjustments to make buggy upright, forward facing, and above ground
			transform.Rotate(0,90,0);
			//move the buggy up a little to prevent falling under the track, and to the side to correct for position differential between buggy and chassis
			//transform.position = new Vector3(transform.position.x+7.27f, transform.position.y+1, transform.position.z+1.4f);
		}
		else
		{
			transform.position = lastValidPosition;
		}
	}
	
	GameObject LatestCheckpoint()//find the checkpoint this buggy has hit that's furthest along in the race track
	{
		if(playerControlsScript.checkpointsHit.Count == 0)
		{
			//if they haven't hit any checkpoints at all, use the starting line
			foreach(GameObject g in GameManager.Instance.checkpoints)
			{
				if(g.GetComponent<RaceCheckpoints>().checkpointNumber == 0)
				{
					return g;
				}
			}
			Debug.LogWarning("Could not find the starting checkpoint! Make sure there is a checkpoint 0.");
			return null;
		}
		else
		{
			GameObject highestCheckpoint = null;
			int highestCheckpointNumber = -1;//below 0 so that the starting line can be chosen
			foreach(GameObject g in playerControlsScript.checkpointsHit)
			{
				if(g.GetComponent<RaceCheckpoints>().checkpointNumber > highestCheckpointNumber)
				{
					highestCheckpointNumber = g.GetComponent<RaceCheckpoints>().checkpointNumber;
					highestCheckpoint = g;
				}
			}
			return highestCheckpoint;
		}
	}
	
	
	#endregion
	
	#region Scrap
	
	public float timeBetweenScrapLaunches = 0.5f;
	public float scrapLaunchMagnitude = 8f;
	
	IEnumerator LaunchScrapCoroutine()
	{
		while(playerControlsScript.heldScrap > 0)
		{
			playerControlsScript.heldScrap--;
			
			GameObject s = Instantiate(GameManager.Instance.scrap, transform.position, transform.rotation);
			s.GetComponent<Scrap>().buggyToIgnore = this.gameObject;
			StartCoroutine(EnableColliderCoroutine(s));
			float massCounteraction = 2000f;
			//Vector3 dir = new Vector3(Random.Range(0, 360), Random.Range(0,360), Random.Range(0, 360));
			//Vector3 dir = Random.insideUnitCircle.normalized;
			//s.transform.Rotate(dir);
			s.GetComponent<Rigidbody>().AddForce(s.transform.transform.up*scrapLaunchMagnitude*massCounteraction + new Vector3(Random.Range(0,40), Random.Range(0,40), Random.Range(0,40)), ForceMode.Impulse);
			GameManager.Instance.excessScrap.Add(s);
			
			yield return new WaitForSeconds(timeBetweenScrapLaunches);
		}
	}
	
	IEnumerator EnableColliderCoroutine(GameObject scrap)
	{
		yield return new WaitForSeconds(.5f);
		scrap.GetComponent<Scrap>().buggyToIgnore = null;
	}
	
	public void PickupScrap(int amount)
	{
		playerControlsScript.ChangeHeldScrap(amount);
	}
	
	#endregion
}
