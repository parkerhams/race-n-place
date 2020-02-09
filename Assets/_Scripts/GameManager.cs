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

	public enum Mode { Race, Place, Title, Selection, End};
	public Mode mode = Mode.Title;

    public Cursor[] cursors;
	Player player;
    
	public GameObject announceText;
	public GameObject subAnnounceText;
	int activeAnnouncements = 0;
	int maxAnnouncements = 12;
	
	public int currentRound = 0;
	int maxRounds = 3;
	
	public int lapsPerRound = 2;
	int playersFinishedRace = 0;
	
	public int numOfPlayers = 1;
	
	int checkpointLast = 0;//the highest number checkpoint before the finish line
	public List<GameObject> checkpoints = new List<GameObject>();

    #endregion


    // Start is called before the first frame update
    void Start()
	{
		//clear any announcement text that exists
		announceText.GetComponent<TextMeshProUGUI>().text = "";
		subAnnounceText.GetComponent<TextMeshProUGUI>().text = "";
		
		//skip title screen mode since it's not implemented yet
        StartGame();
    }

    // Update is called once per frame
    void Update()
	{
		
    }

    /// <summary>
    /// Switches game mode from race to place, or vice versa
    /// </summary>
    public void SwapMode()
    {
	    Debug.Log("Swapping mode");
	    //increment round only whenever race mode starts
	    if(mode != Mode.Race)
	    {
		    currentRound++;
		    if(currentRound > maxRounds)
		    {
			    currentRound = 1;
		    }
	    }
        
        
        if(mode == Mode.Race)
        {
            //switch to place
	        mode = Mode.Place;
	        RoundStartAnnouncement();
            foreach (Cursor c in cursors)
            {
	            c.ToggleActive(true);
	            c.player.controllers.maps.SetMapsEnabled(false, 1);//disable driving controls
	            c.player.controllers.maps.SetMapsEnabled(true, 2);//enable tower controls
            }
        }
        else if(mode == Mode.Place)
        {
            //switch to race
	        mode = Mode.Race;
	        RoundStartAnnouncement();
            foreach(Cursor c in cursors)
            {
	            c.ToggleActive(false);
	            c.player.controllers.maps.SetMapsEnabled(true, 1);//enable driving controls
	            c.player.controllers.maps.SetMapsEnabled(false, 2);//disable tower controls
            }
        }
    }

	public void GoToSelectionMode()
	{
		//go to the selection screen. do this every time after a race ends?
		//should each player's selected character remain locked in?
		mode = Mode.Selection;
		
	}
    
	public void LapComplete(Buggy buggyScript)//any time any player completed a lap
	{
		Debug.Log("Player " + buggyScript.playerId + " completed a lap!");
		buggyScript.checkpointsHit = null;
		
		buggyScript.lapsCompleted++;
		if(buggyScript.lapsCompleted >= lapsPerRound)
		{
			buggyScript.finishedRace = true;
			playersFinishedRace++;
			buggyScript.raceStanding = playersFinishedRace;
			
		}
	}
    
	void SetNumOfPlayers()
	{
		//after switching from selection screen to race, set the number of players
		//for debug purposes right now the number is just 1
	}
    
	public void LoadMapCheckpoint(GameObject checkpoint)
	{
		RaceCheckpoints checkScript = checkpoint.GetComponent<RaceCheckpoints>();
		if(checkScript.checkpointNumber > checkpointLast)
		{
			checkpointLast = checkScript.checkpointNumber;
		}
		
	}

    void StartGame()
    {
        //go from title screen to race
        mode = Mode.Place;
        SwapMode();
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
		print("announcer position: " + announcer.transform.position.x + ", clone position: " + clone.transform.position.x);
		
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
}
