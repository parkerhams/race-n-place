using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using TMPro;
using DG.DemiLib;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    #region Singleton

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }

            return _instance;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    #endregion
    

    #region Variables

	public enum Mode { Race, Place, Title, Selection, End, Tutorial};
	public Mode mode = Mode.Title;

    public Cursor[] cursors;
	Player player;
    
	public GameObject announceText;
	public GameObject subAnnounceText;
	public GameObject buggy;
	public int highestPointsThisRound = 0;
	int activeAnnouncements = 0;
	int maxAnnouncements = 12;
	public bool showingRaceResults = false;
	public bool raceCountingDown = false;//true when the 3, 2, 1, countdown is running right before race starts
	
	public int currentRound = 0;
	public int maxRounds = 12;
	
	public int lapsPerRound = 1;
	int playersFinishedRace = 0;
	public bool skipRaceResults = true;
	public int standingPointsValue = 5;//standingPointsValue - raceStanding = standingPoints. so if this value is 5, first place gets 4 points, 2nd place gets 3, etc
	
	public int numOfPlayers = 1;
	
	public float raceEndDuration = 3f;
	bool raceStatsOver = false;//true when the game has finished displaying post race stats and is ready to move onto place mode
	
	int checkpointLast = 0;//the highest number checkpoint before the finish line
	public List<GameObject> checkpoints = new List<GameObject>();
	public List<GameObject> allActivePlayers = new List<GameObject>();
	public List<SelectionPortrait> selectionPortraits = new List<SelectionPortrait>();
	[HideInInspector]
	public List<Portrait> ingamePortraits = new List<Portrait>();
	public int confirmedCharacters = 0;
	public GameObject[] genericPlayers;
	public GameObject[] tracks;
	public GameObject currentTrack;
	public List<Buggy> buggyScripts = new List<Buggy>();
	public List<GameObject> excessScrap = new List<GameObject>();
	
	public Camera camera;
	public Timer timer;
	public GameObject scrap;
	
	public List<GameObject> placedTraps = new List<GameObject>();

    #endregion


    // Start is called before the first frame update
    void Start()
	{
		camera.transform.DOMove(GameObject.Find("cameraStartPos").transform.position, 0f);
		camera.transform.DORotate(GameObject.Find("cameraStartPos").transform.rotation.eulerAngles, 0f);
		
		//clear any announcement text that exists
		announceText.GetComponent<TextMeshProUGUI>().text = "";
		subAnnounceText.GetComponent<TextMeshProUGUI>().text = "";
		
		//animate to the a zoom-in of the jumbotron for the charater select
		StartTitleScreen();
    }

    // Update is called once per frame
    void Update()
	{
		
	}
	
	void EndRace()
	{
		if(currentRound == 0)
		{
			return;
		}
		showingRaceResults = true;
		//lock controls?
		//pause timer?
		//zoom in on jumbotron
		//show how many points each person got and who wins a match point
		highestPointsThisRound = 0;
		
		camera.transform.DOMove(currentTrack.GetComponent<Track>().billboardCameraPosition.transform.position, .5f);
		camera.transform.DORotate(currentTrack.GetComponent<Track>().billboardCameraPosition.transform.rotation.eulerAngles, .5f);
		
		foreach(GameObject g in allActivePlayers)
		{
			PlayerControls pScript = g.GetComponent<PlayerControls>();
			pScript.portrait.GetComponent<Portrait>().UpdateStanding(false);
			highestPointsThisRound = Mathf.Max(highestPointsThisRound, (pScript.standingPoints + pScript.destructionPoints));
		}
		//after a second, announce winner
		StartCoroutine(WinnerAnnounceCoroutine());
		//after a couple seconds, zoom out in preparation for switching to place mode
		//StartCoroutine(RaceEndDelay());
	}
	IEnumerator WinnerAnnounceCoroutine()
	{
		bool endingMatch = false;
		yield return new WaitForSeconds(1.5f);
		foreach(GameObject g in allActivePlayers)
		{
			PlayerControls pScript = g.GetComponent<PlayerControls>();
			pScript.portrait.GetComponent<Portrait>().UpdateStanding(true);
		}
		yield return new WaitForSeconds(1.5f);
		raceStatsOver = true;
		showingRaceResults = false;
		foreach(GameObject g in allActivePlayers)
		{
			PlayerControls pScript = g.GetComponent<PlayerControls>();
			pScript.destructionPoints = 0;
			pScript.standingPoints = 0;
			if(pScript.roundWins == 3)
			{
				Announcement("Player " + pScript.playerId + " wins!", announceText);
				Announcement("Supreme champion " + pScript.character.ToString().ToLower() + "!", subAnnounceText);
				yield return new WaitForSeconds(3f);
				endingMatch = true;
			}
		}
		if(endingMatch)
		{
			EndMatch();
		}
		else
		{
			camera.transform.DOMove(currentTrack.GetComponent<Track>().defaultCameraPosition.transform.position, .5f);
			camera.transform.DORotate(currentTrack.GetComponent<Track>().defaultCameraPosition.transform.rotation.eulerAngles, .5f);
			SwapMode();
		}
		
	}
    
	void EndMatch()
	{
		camera.transform.DOMove(currentTrack.GetComponent<Track>().defaultCameraPosition.transform.position, .5f);
		camera.transform.DORotate(currentTrack.GetComponent<Track>().defaultCameraPosition.transform.rotation.eulerAngles, .5f);
		Debug.Log("End of match");
		//remove all traps
		foreach(GameObject t in placedTraps)
		{
			Destroy(t);
		}
		placedTraps.Clear();
		//announce supreme champion winner, throw confetti everywhere for a few seconds
		currentRound = 0;
		foreach(GameObject g in allActivePlayers)
		{
			PlayerControls pControls = g.GetComponent<PlayerControls>();
			pControls.bankedScrap = pControls.defaultBankedScrap;
			pControls.destructionPoints = 0;
			pControls.standingPoints = 0;
			pControls.roundWins = 0;
			pControls.lapsCompleted = 0;
			pControls.finishedRace = false;
		}
		playersFinishedRace = 0;
		//after a delay or button prompts, go back to selection screen
		//for now, the game just immediately loops back to round 1 on current track
		mode = Mode.Place;
		SwapMode();
	}

	public delegate void ModeSwapping();
	public static event ModeSwapping ModeSwapped;
	public static event ModeSwapping RaceEnded;

    /// <summary>
    /// Switches game mode from race to place, or vice versa
    /// </summary>
    public void SwapMode()
	{
		if(showingRaceResults)
		{
			return;
		}
		foreach(GameObject s in excessScrap)
		{
			Destroy(s);//for now, just get rid of all excess scrap when mode swapped
		}
	    //increment round only whenever race mode starts
	    if(mode != Mode.Race)
	    {
		    currentRound++;
	    }
	    if(mode == Mode.Race && currentRound >= maxRounds)
	    {
	    	if(!skipRaceResults) EndRace();
	    	mode = Mode.Place;//so that when the next SwapMode occurs, it starts in race mode
	    	EndMatch();
	    	return;
	    }
        
        
        if(mode == Mode.Race)
        {
        	if(!raceStatsOver && !skipRaceResults)
        	{
        		EndRace();
        		return;
        	}
            //switch to place
	        mode = Mode.Place;
	        //clear out old data about race mode stats
	        foreach(GameObject g in allActivePlayers)
	        {
	        	PlayerControls pControls = g.GetComponent<PlayerControls>();
	        	pControls.destructionPoints = 0;
	        	pControls.standingPoints = 0;
	        	pControls.finishedRace = false;
	        	pControls.lapsCompleted = 0;
	        }
	        playersFinishedRace = 0;
	        
	        RoundStartAnnouncement();
            foreach (Cursor c in cursors)
            {
	            c.ToggleActive(true);
	            c.player.controllers.maps.SetMapsEnabled(false, 1);//disable driving controls
	            c.player.controllers.maps.SetMapsEnabled(true, 2);//enable tower controls
            }
	        foreach(Buggy b in buggyScripts)
	        {
	        	b.gameObject.SetActive(false);
	        }
        }
        else if(mode == Mode.Place)
        {
            //switch to race
	        mode = Mode.Race;
	        StartCoroutine(RaceStartCountdownCoroutine());
	        raceStatsOver = false;
	        RoundStartAnnouncement();
            foreach(Cursor c in cursors)
            {
	            c.ToggleActive(false);
	            c.player.controllers.maps.SetMapsEnabled(true, 1);//enable driving controls
	            c.player.controllers.maps.SetMapsEnabled(false, 2);//disable tower controls
            }
	        foreach(Buggy b in buggyScripts)
	        {
	        	b.gameObject.SetActive(true);
	        	b.playerControlsScript.checkpointsHit.Clear();
	        	b.Respawn();
	        }
        }
	    if(ModeSwapped != null)
	    {
		    Debug.Log("Swapping mode to " + mode);
		    ModeSwapped();
	    }
    }

	public void GoToSelectionMode()
	{
		//go to the selection screen. do this every time after a race ends?
		//should each player's selected character remain locked in?
		mode = Mode.Selection;
		
	}
	
	IEnumerator RaceEndDelay()
	{
		//StartCoroutine(ReportStandingsCoroutine());
		yield return new WaitForSeconds(raceEndDuration);
		if(mode == Mode.Race)
		{
			SwapMode();
		}
	}
    
	public void LapComplete(PlayerControls playerControlsScript)//any time any player completed a lap
	{
		Debug.Log("Player " + playerControlsScript.playerId + " completed a lap!");
		playerControlsScript.checkpointsHit.Clear();
		
		playerControlsScript.lapsCompleted++;
		Debug.Log("laps completed: " + playerControlsScript.lapsCompleted + " / " + lapsPerRound);
		if(playerControlsScript.lapsCompleted >= lapsPerRound)
		{
			if(playerControlsScript.finishedRace)
			{
				return;//they already finished the race and are stunting on everyone, don't do anything
			}
			if(timer.timeRemaining > timer.raceTimeLimitAfterSomeoneFinishes)
			{
				timer.timeRemaining = timer.raceTimeLimitAfterSomeoneFinishes;
			}
			playerControlsScript.finishedRace = true;
			playersFinishedRace++;
			playerControlsScript.raceStanding = playersFinishedRace;
			playerControlsScript.standingPoints = 5 - playerControlsScript.raceStanding;
			playerControlsScript.portrait.GetComponent<Portrait>().UpdateScrap();
			Debug.Log("Player " + playerControlsScript.playerId + " finish the race in place:" + playerControlsScript.raceStanding);
			Announcement(playerControlsScript.character.ToString() + " finished in " + playerControlsScript.raceStanding + ToOrdinal(playerControlsScript.raceStanding) + " place!", subAnnounceText);
			if(isRaceComplete())
			{
				SwapMode();
			}
			
			
		}
	}
	
	bool isRaceComplete()
	{
		//check if any player hasn't finished the race
		foreach(GameObject a in allActivePlayers)
		{
			if(!a.GetComponent<PlayerControls>().finishedRace)
			{
				return false;
			}
		}
		return true;
	}
	
	IEnumerator RaceStartCountdownCoroutine()
	{
		Debug.Log("starting countdown");
		raceCountingDown = true;
		timer.PauseTimer();
		yield return new WaitForSeconds(1f);//wait a moment, the camera needs to pan over probably
		//3
		Portrait p1 = FindPortrait(0);
		StartCoroutine(p1.ShowCountdownCoroutine("3"));
		yield return new WaitForSeconds(1f);
		//2
		Portrait p2 = FindPortrait(1);
		StartCoroutine(p2.ShowCountdownCoroutine("2"));
		yield return new WaitForSeconds(1f);
		//1
		Portrait p3 = FindPortrait(2);
		StartCoroutine(p3.ShowCountdownCoroutine("1"));
		yield return new WaitForSeconds(1f);
		//go
		Portrait p4 = FindPortrait(3);
		StartCoroutine(p4.ShowCountdownCoroutine("GO"));
		raceCountingDown = false;
		timer.ResumeTimer();
	}
	
	Portrait FindPortrait(int playerID)
	{
		foreach(Portrait p in ingamePortraits)
		{
			if(p.playerId == playerID)
			{
				Debug.Log("found " + p + " ID " + p.playerId);
				return p;
			}
		}
		Debug.Log("Couldn't find portrait " + playerID);
		return null;
	}
    
	public void UpdateNumOfPlayers(GameObject genericPlayer, bool adding)//adding or removing a player from consideration as being a participant in the game (when they plug in a controller and press a button, or disconnect a controller)
	{
		if(adding)
		{
			allActivePlayers.Add(genericPlayer);
		}
		else{
			allActivePlayers.Remove(genericPlayer);
		}
		numOfPlayers = allActivePlayers.Count;
		Debug.Log("Current number of active players: " + numOfPlayers);
	}
    
	public void LoadMapCheckpoint(GameObject checkpoint)
	{
		RaceCheckpoints checkScript = checkpoint.GetComponent<RaceCheckpoints>();
		if(checkScript.checkpointNumber > checkpointLast)
		{
			checkpointLast = checkScript.checkpointNumber;
		}
		checkpoints.Add(checkpoint);
		
	}

	public void StartGame()
	{
		DOTween.KillAll();
		Debug.Log("start game");
		camera.transform.DOMove(currentTrack.GetComponent<Track>().defaultCameraPosition.transform.position, 1f);
		camera.transform.DORotate(currentTrack.GetComponent<Track>().defaultCameraPosition.transform.rotation.eulerAngles, 1f);
        //go from title screen to race
        mode = Mode.Place;
        SwapMode();
    }
    
	private void StartTitleScreen()
	{
		mode = Mode.Title;
		
		//move camera to title screen to display logo
		camera.transform.DOMove(GameObject.Find("TitleScreen").transform.Find("cameraTitleScreenPos").gameObject.transform.position, 1f);
		camera.transform.DORotate(GameObject.Find("TitleScreen").transform.Find("cameraTitleScreenPos").gameObject.transform.rotation.eulerAngles, 1f);
	
		//play particle effects on either side of the jumbotron
	}
	
	public void StartJoinScreen()
	{
		DOTween.KillAll();
		mode = Mode.Selection;
		camera.transform.DOMove(GameObject.Find("JoinScreen").transform.Find("cameraJoinScreenPos").gameObject.transform.position, .5f);
		camera.transform.DORotate(GameObject.Find("JoinScreen").transform.Find("cameraJoinScreenPos").gameObject.transform.rotation.eulerAngles, .4f);
		//join screen behaviors not implemented yet
		//StartGame();
	}
	
	public void StartTutorialScreen()
	{
		DOTween.KillAll();
		mode = Mode.Tutorial;
		camera.transform.DOMove(GameObject.Find("TutorialScreen").transform.Find("cameraTutorialScreenPos").gameObject.transform.position, .5f);
		camera.transform.DORotate(GameObject.Find("TutorialScreen").transform.Find("cameraTutorialScreenPos").gameObject.transform.rotation.eulerAngles, .4f);
		//join screen behaviors not implemented yet
		//StartGame();
	}
    
	void RoundStartAnnouncement()
	{
		string aText = "";
		string sText = "";
		if(mode == Mode.Place)
		{
			aText = "Place!";
		}
		else if(mode == Mode.Race)
		{
			aText = "Race!";
		}
		
		if(currentRound == maxRounds)
		{
			sText = "Final round!";
		}
		else
		{
			sText = "Round " + currentRound;
		}
		Announcement(aText, announceText);
		Announcement(sText, subAnnounceText);
	}
    
	void Announcement(string textToAnnounce, GameObject announcer)
	{
		if(textToAnnounce == null)
		{
			Debug.Log("Announcement made without any text");
			return;
		}
		if(activeAnnouncements >= maxAnnouncements)
		{
			//prevents the announcements from bugging out by trying to play too many at one time
			//this isn't expected to happen except when pressing the debug swap mode button
			print("too many announcements on screen");
			return;
		}
		
		activeAnnouncements++;
		
		RectTransform clone = Instantiate(announcer, announcer.transform.position, announcer.transform.rotation).GetComponent<RectTransform>();
		clone.transform.SetParent(announcer.transform.parent);
		clone.transform.position = announcer.transform.position;
		
		//move offscreen
		clone.transform.DOLocalMoveX(-2000f, 0f);

		clone.GetComponent<TextMeshProUGUI>().text = textToAnnounce;
		
		//tween the announcement rect transforms on/off screen
		Sequence seq = DOTween.Sequence();
		seq.Append(clone.transform.DOLocalMoveX(30f, .6f));
		seq.Append(clone.transform.DOLocalMoveX(0f, .3f));
		seq.AppendInterval(1.5f);
		seq.Append(clone.transform.DOLocalMoveX(-30f, .3f));
		seq.Append(clone.transform.DOLocalMoveX(2000f, .6f)).OnComplete(()=>{
			activeAnnouncements--;
			Destroy(clone.gameObject);
		});
	}
	
	public string ToOrdinal(int num)
	{
		string extension = "th";

		int last_digits = num % 100;

		if (last_digits < 11 || last_digits > 13)
		{
			switch (last_digits % 10)
			{
			case 1:
				extension = "st";
				break;
			case 2:
				extension = "nd";
				break;
			case 3:
				extension = "rd";
				break;
			}
		}

		return extension;
	}
}
