using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// Players will each have a "generic player" game object that they always own and control, for accessing controls like options menu, title screen navigation, etc
/// This script will also store persistent information about the player's status such as laps completed, score, race standing, scrap owned/banked
/// </summary>
public class PlayerControls : MonoBehaviour
{
	public int playerId = 0;

	public Player player;
	
	public enum Characters { Fox=0, Snake=1, Goat=2, Toad=3};
	public Characters character = Characters.Fox;
	
	public bool isPlaying = false;//whether or not there's someone playing in this slot (GenericPlayers 1, 2, 3, 4)
	
	public List<GameObject> checkpointsHit = new List<GameObject>();//all the checkpoints the player has touched during the current lap
	public int lapsCompleted = 0;
	public bool finishedRace = false;//whether or not they finished all their laps and are done racing
	public int raceStanding = 1;//1 is 1st place, 2 is 2nd, etc
	public int bankedScrap = 8;
	public int defaultBankedScrap = 8;
	public int destructionPoints = 0;
	public int standingPoints = 0;//1st place is worth 4 points, 2nd is 3, 3rd is 1, 4th is 0
	public int roundWins = 0;
	public int heldScrap = 0;//scrap the player has acquired during the race. added to bankedScrap when round ends
	public GameObject myBuggy;
	public GameObject portrait;
	
	public string characterName = "Player";
	//by default, this will be "Player 1", "Player 2", etc
	//as soon as they lock in a character, their name will change to that character's name - "Lizard", "Fox", etc
	
	void OnEnable()
	{
		GameManager.ModeSwapped += OnModeSwapped;
	}
	void OnDisable()
	{
		GameManager.ModeSwapped -= OnModeSwapped;
	}

    private void Awake()
    {
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
	    player = ReInput.players.GetPlayer(playerId);
	    ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
	    ReInput.ControllerConnectedEvent += OnControllerConnected;
	    characterName = "Player " + playerId;
    }

	void GetInput()//all input is fetched in Update so that no inputs are lost
	{
		
		if(GameManager.Instance.mode == GameManager.Mode.Title)
		{
			if(player.GetButtonDown("TitleScreenInput"))
			{
				foreach(SelectionPortrait s in GameManager.Instance.selectionPortraits)
				{
					StartCoroutine(s.PreventInstantInputCoroutine());
					GameManager.Instance.StartJoinScreen();
				}
			}
			
		}
		if(GameManager.Instance.mode == GameManager.Mode.Title)
		{
			if(player.GetButtonDown("TitleScreenInput"))
			{
				//confirm character
			}
			if(player.GetAxis("Horizontal") < -.5f)
			{
				
			}
			else if(player.GetAxis("Horizontal") > .5f)
			{
				//switch to next right portrait
			}
			
		}
		
		if(GameManager.Instance.mode == GameManager.Mode.Tutorial)
		{
			if(player.GetButtonDown("TitleScreenInput"))
			{
				GameManager.Instance.StartGame();
			}
		}
		//make sure the game realizes the first player is connected (the oncontrollerconnected function doesn't run for controllers that were plugged in before the game started)
		if(player.GetAnyButtonDown() && isPlaying == false)
		{
			isPlaying = true;
			GameManager.Instance.UpdateNumOfPlayers(this.gameObject, true);
		}
    	
	    if (player.GetButtonDown("DebugSwapMode"))
	    {
	    	Debug.Log("debug swap mode");
		    GameManager.Instance.SwapMode();
	    }
	    
	    if(GameManager.Instance.mode == GameManager.Mode.Title)
	    {
		    if(player.GetAnyButtonDown())
		    {
		    	GameManager.Instance.GoToSelectionMode();
		    }
	    }
	    
	    
	    //TODO: Set input actions for "Race" Mode. These actions will only be available if game is in "Race" mode
	    
	    //TODO: Set input actions for "Place" Mode These actions will only be available if game is in "Place" mode
	    
	    
	}
    
	void OnModeSwapped()
	{
		if(GameManager.Instance.mode == GameManager.Mode.Place)
		{
			ChangeBankedScrap(heldScrap);
			ChangeHeldScrap(-heldScrap);
		}
	}
    
	
	void OnControllerConnected(ControllerStatusChangedEventArgs args)
	{
		if(args.controllerId == playerId)
		{
			Debug.Log("A controller was connected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
			isPlaying = true;
			GameManager.Instance.UpdateNumOfPlayers(this.gameObject, true);
			MakeBuggy();
		}
	}
	
	void MakeBuggy()
	{
		if(myBuggy == null)
		{
			GameObject theBuggy = Instantiate(GameManager.Instance.buggy, new Vector3(-11, 2, -5), Quaternion.identity);
			theBuggy.SetActive(true);
			myBuggy = theBuggy;
			theBuggy.GetComponent<Buggy>().playerId = playerId;
			theBuggy.GetComponent<Buggy>().playerControlsScript = this;
			theBuggy.GetComponent<Buggy>().player = ReInput.players.GetPlayer(playerId);
			Debug.Log("Player " + playerId + " buggy created.");
		}
		else{
			myBuggy.SetActive(true);
			Debug.Log("Player " + playerId + " buggy re-enabled.");
		}
	}
	
	void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
	{
		if(args.controllerId == playerId)
		{
			Debug.Log("A controller was disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
			myBuggy.SetActive(false);
			Debug.Log("Player " + playerId + " buggy disabled.");
			GameManager.Instance.UpdateNumOfPlayers(this.gameObject, false);		}
	}

    void ProcessInput()//runs in Update, for actions that don't need to use physics
    {

    }

    void ProcessFixedInput()//runs in FixedUpdate, for actions that use physics
	{
    	
	}
	
    
	public void ChangeHeldScrap(int amount)
	{
		heldScrap += amount;
		if(heldScrap <= 0)
		{
			heldScrap = 0;
		}
	}
	
	public void ChangeBankedScrap(int amount)
	{
		bankedScrap += amount;
		if(bankedScrap <= 0)
		{
			bankedScrap = 0;
		}
		portrait.GetComponent<Portrait>().UpdateScrap();
	}

    // Update is called once per frame
    void Update()
	{
        GetInput();
        ProcessInput();
    }
    private void FixedUpdate()
    {
        ProcessFixedInput();
    }
}
