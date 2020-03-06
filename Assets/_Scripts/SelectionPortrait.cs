using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectionPortrait : MonoBehaviour
{
	PlayerControls pScript;
	
	float selectionValue = 0f;
	bool isActive = false;
	public bool hasSelectedCharacter = false;
	bool cycledRecently = false;
	float cycleDelay = .5f;
	bool disabledConfirmButton = true;
	
	public int playerId = 0;
	public Image portraitImage;
	
	public Image leftArrow;
	public Image rightArrow;
	public Image confirmButtonPrompt;
	
	
	public Sprite foxImage;
	public Sprite snakeImage;
	public Sprite goatImage;
	public Sprite toadImage;
	
	List<PlayerControls.Characters> unselectedCharacters = new List<PlayerControls.Characters>();
	PlayerControls.Characters hoveredCharacter = PlayerControls.Characters.Fox;
	
    // Start is called before the first frame update
    void Start()
    {
	    unselectedCharacters.Add(PlayerControls.Characters.Fox);
	    unselectedCharacters.Add(PlayerControls.Characters.Goat);
	    unselectedCharacters.Add(PlayerControls.Characters.Snake);
	    unselectedCharacters.Add(PlayerControls.Characters.Toad);
	    
	    isActive = false;
	    gameObject.SetActive(false);
	    
	    foreach(GameObject p in GameManager.Instance.allActivePlayers)
	    {
		    PlayerControls playerControlsScript = p.GetComponent<PlayerControls>();
		    if(playerControlsScript.playerId == playerId)
		    {
		    	Debug.Log("activated" + p);
		    	pScript = playerControlsScript;
		    	isActive = true;
		    	hoveredCharacter = PlayerControls.Characters.Snake;
		    	gameObject.SetActive(true);
		    }
		    
	    }
	    
	    
	    if(isActive)
	    {
	    	Debug.Log("gave image to " + this.gameObject);
	    	portraitImage.sprite = foxImage;
	    	ToggleSelectionUI(true);
	    }
	    else
	    {
	    	Debug.Log("nulled " + this.gameObject);
	    	portraitImage.sprite = null;
	    }
    }

    // Update is called once per frame
    void Update()
	{
		if(isActive)
		{
			if(GameManager.Instance.mode == GameManager.Mode.Selection)
		    {
			    if(pScript.player.GetButtonDown("TitleScreenInput"))
			    {
				    ConfirmCharacterSelect();
				    if(GameManager.Instance.confirmedCharacters == GameManager.Instance.allActivePlayers.Count && !disabledConfirmButton)
				    {
				    	GameManager.Instance.StartTutorialScreen();
				    }
				    else
				    {
				    	Debug.Log("confirmed characters: " + GameManager.Instance.confirmedCharacters + "active players: " + GameManager.Instance.allActivePlayers.Count);
				    }
			    }
				if(pScript.player.GetButtonDown("Back"))
				{
					UndoCharacterSelect();
				}
			    if(pScript.player.GetAxis("Horizontal") < -.5f)
			    {
				    CyclePortraitOptions(-1, true);
			    }
			    else if(pScript.player.GetAxis("Horizontal") > .5f)
			    {
				    CyclePortraitOptions(1, true);
			    }
				
		    }
		}
    }
    
	void UpdateShownCharacter()
	{
		if(hoveredCharacter != null)
		{
			switch(hoveredCharacter)
			{
			case PlayerControls.Characters.Fox:
				portraitImage.sprite = foxImage;
				break;
			case PlayerControls.Characters.Snake:
				portraitImage.sprite = snakeImage;
				break;
			case PlayerControls.Characters.Goat:
				portraitImage.sprite = goatImage;
				break;
			case PlayerControls.Characters.Toad:
				portraitImage.sprite = toadImage;
				break;
			}
		}
	}
    
	public void ConfirmCharacterSelect()
	{
		if(disabledConfirmButton)
		{
			return;
		}
		if(hasSelectedCharacter)
		{
			return;
		}
		if(hoveredCharacter != null)
		{
			if(unselectedCharacters.Contains(hoveredCharacter))
			{
				pScript.character = hoveredCharacter;
				hasSelectedCharacter = true;
				ToggleSelectionUI(false);
				GameManager.Instance.confirmedCharacters++;
				StartCoroutine(PreventInstantInputCoroutine());
				
				foreach(SelectionPortrait s in GameManager.Instance.selectionPortraits)
				{
					s.unselectedCharacters.Remove(hoveredCharacter);
					if(s != this)
					{
						s.MoveOffSelectedCharacter();
					}
				}
			}
		}
	}
	
	void UndoCharacterSelect()
	{
		if(!hasSelectedCharacter)
		{
			return;
		}
		hasSelectedCharacter = false;
		ToggleSelectionUI(true);
		GameManager.Instance.confirmedCharacters--;
		
		foreach(SelectionPortrait s in GameManager.Instance.selectionPortraits)
		{
			s.unselectedCharacters.Add(pScript.character);
		}
	}
	
	void ToggleSelectionUI(bool on)
	{
		if(on)
		{
			leftArrow.gameObject.SetActive(true);
			rightArrow.gameObject.SetActive(true);
			confirmButtonPrompt.gameObject.SetActive(true);
		}
		else
		{
			leftArrow.gameObject.SetActive(false);
			rightArrow.gameObject.SetActive(false);
			confirmButtonPrompt.gameObject.SetActive(false);
		}
	}
	
	void MoveOffSelectedCharacter()
	{
		int tempInfiniteLoopPrevention = 0;
		while(!unselectedCharacters.Contains(hoveredCharacter))
		{
			CyclePortraitOptions(1, false);
			tempInfiniteLoopPrevention++;
			if(tempInfiniteLoopPrevention > 10)
			{
				Debug.Log("infinite loop in portrait cycling!");
				break;
			}
		}
	}
	
	public IEnumerator CycleDelayCoroutine()
	{
		cycledRecently = true;
		yield return new WaitForSeconds(cycleDelay);
		cycledRecently = false;
	}
	public IEnumerator PreventInstantInputCoroutine()
	{
		disabledConfirmButton = true;
		yield return new WaitForSeconds(.5f);
		disabledConfirmButton = false;
	}
    
	public void CyclePortraitOptions (int direction, bool manual)//-1 to left, 1 to right. "manual" means the player initiated it so give it a cycle delay
	{
		if(hasSelectedCharacter)
		{
			return;
		}
		if(cycledRecently && manual)
		{
			return;
		}
		if(manual)
		{
			StartCoroutine(CycleDelayCoroutine());
		}
		hoveredCharacter += direction;
		if((int)hoveredCharacter > 3)
		{
			hoveredCharacter = (PlayerControls.Characters)0;
		}
		if((int)hoveredCharacter < 0)
		{
			hoveredCharacter = (PlayerControls.Characters)3;
		}
		MoveOffSelectedCharacter();
		UpdateShownCharacter();
	}
}
