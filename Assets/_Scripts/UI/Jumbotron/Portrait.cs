using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Portrait : MonoBehaviour
{
	
	PlayerControls pScript;
	
	public int playerId = 0;
	public bool isActive = false;
	
	public Image portraitImage;
	
	public GameObject standingText;
	public GameObject scrapText;
	public GameObject scrapImage;
	
	[Header("Post-race stats")]
	public GameObject postRaceStats;//the parent object of all the post race stats
	public TextMeshProUGUI standingPointsText;
	public TextMeshProUGUI destructionPointsText;
	public TextMeshProUGUI winnerText;
	public TextMeshProUGUI countdownText;
	public Image win1;
	public Image win2;
	public Image win3;
	
	public Image winEmpty;
	public Image winFilled;
	
	
	public Sprite foxImage;
	public Sprite snakeImage;
	public Sprite goatImage;
	public Sprite toadImage;
	
	Color purple = new Color(170,96,178);//fox
	Color red = new Color(222,105,148);//snek
	Color blue = new Color(78,172,186);//goat
	Color green = new Color(90,131,49);//toad
	
	
	void OnEnable()
	{
		GameManager.ModeSwapped += EstablishPortrait;
	}
	void OnDisable()
	{
		GameManager.ModeSwapped -= EstablishPortrait;
	}
	
	
    // Start is called before the first frame update
    void Start()
	{
		GameManager.Instance.ingamePortraits.Add(this);
		EstablishPortrait();
    }
    
	void EstablishPortrait()
	{
		foreach(GameObject p in GameManager.Instance.allActivePlayers)
		{
			PlayerControls playerControlsScript = p.GetComponent<PlayerControls>();
			if(playerControlsScript.playerId == playerId)
			{
				pScript = playerControlsScript;
				isActive = true;
				UpdatePortrait();
				UpdateScrap();
				UpdateStanding(false);
				return;
			}
		}
		//if no active player existed for this portrait, then blank out the portrait
		portraitImage.sprite = null;
		portraitImage.color = Color.black;
		UpdateScrap();
		UpdateStanding(false);
	}
	
	public IEnumerator ShowCountdownCoroutine(string textToShow)
	{
		Debug.Log(textToShow);
		portraitImage.gameObject.SetActive(false);
		countdownText.gameObject.SetActive(true);
		countdownText.text = textToShow;
		scrapText.SetActive(false);
		scrapImage.SetActive(false);
		//number
		yield return new WaitForSeconds(1f);
		//normal
		if(isActive) UpdateScrap();
		countdownText.gameObject.SetActive(false);
		portraitImage.gameObject.SetActive(true);
	}
    
	void UpdatePortrait()
	{
		if(!pScript)
		{
			return;
		}
		//if(pScript.character = PlayerControls.Characters.Fox)
		switch (pScript.character)
		{
		case PlayerControls.Characters.Fox:
			portraitImage.color = purple;
			portraitImage.sprite = foxImage;
			break;
		case PlayerControls.Characters.Goat:
			portraitImage.color = blue;
			portraitImage.sprite = goatImage;
			break;
		case PlayerControls.Characters.Snake:
			portraitImage.color = red;
			portraitImage.sprite = snakeImage;
			break;
		case PlayerControls.Characters.Toad:
			portraitImage.color = green;
			portraitImage.sprite = toadImage;
			break;
		default:
			Debug.Log("defaulted");
			break;
		}
	}
	
	public void UpdateScrap()
	{
		if(!isActive)
		{
			scrapText.SetActive(false);
			scrapImage.SetActive(false);
		}
		else
		{
			scrapText.SetActive(true);
			scrapImage.SetActive(true);
			scrapText.GetComponent<TextMeshProUGUI>().text = pScript.bankedScrap.ToString();
		}
	}
	
	public void UpdateStanding(bool announceWinner)
	{
		if(!pScript)
		{
			postRaceStats.SetActive(false);
			return;
		}
		
		if(!GameManager.Instance.showingRaceResults)
		{
			postRaceStats.SetActive(false);
			return;
		}
		
		postRaceStats.SetActive(true);
		UpdateWinImages();
		if(announceWinner)
		{
			//announce the winner
			if((pScript.standingPoints + pScript.destructionPoints) == GameManager.Instance.highestPointsThisRound)
			{
				//this is the winner
				winnerText.text = "Winner!";
				winnerText.gameObject.SetActive(true);
				pScript.roundWins++;
				//give them a point
				//pScript.roundWins++;
				//check if they're overall winner? or do that in game manager
				//update their point images
				UpdateWinImages();
			}
			else
			{
				winnerText.gameObject.SetActive(false);
			}
		}
		else
		{
			winnerText.gameObject.SetActive(false);
		}
		//standingText.GetComponent<TextMeshProUGUI>().text = pScript.raceStanding.ToString() + GameManager.Instance.ToOrdinal(pScript.raceStanding);
		standingPointsText.text = pScript.standingPoints.ToString();
		destructionPointsText.text = pScript.destructionPoints.ToString();

	}
	
	void UpdateWinImages()
	{
		switch(pScript.roundWins)
		{
		case 0:
			win1.sprite = winEmpty.sprite; win1.color = winEmpty.color;
			win2.sprite = winEmpty.sprite; win2.color = winEmpty.color;
			win3.sprite = winEmpty.sprite; win3.color = winEmpty.color;
			break;
		case 1:
			win1.sprite = winFilled.sprite; win1.color = winFilled.color;
			win2.sprite = winEmpty.sprite; win2.color = winEmpty.color;
			win3.sprite = winEmpty.sprite; win3.color = winEmpty.color;
			break;
		case 2:
			win1.sprite = winFilled.sprite; win1.color = winFilled.color;
			win2.sprite = winFilled.sprite; win2.color = winFilled.color;
			win3.sprite = winEmpty.sprite; win3.color = winEmpty.color;
			break;
		case 3:
			win1.sprite = winFilled.sprite; win1.color = winFilled.color;
			win2.sprite = winFilled.sprite; win2.color = winFilled.color;
			win3.sprite = winFilled.sprite; win3.color = winFilled.color;
			break;
		default:
			break;
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
