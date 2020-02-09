using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// Players will each have a "generic player" game object that they always own and control, for accessing controls like options menu, title screen navigation, etc
/// </summary>
public class PlayerControls : MonoBehaviour
{
	public int playerId = 0;

	private Player player;

    private void Awake()
    {
        // Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
        player = ReInput.players.GetPlayer(playerId);
    }
    // Start is called before the first frame update
    void Start()
    {
	    //player.controllers.maps.SetAllMapsEnabled(true);
    }

    void GetInput()//all input is fetched in Update so that no inputs are lost
    {
	    if (player.GetButtonDown("DebugSwapMode"))
	    {
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

    void ProcessInput()//runs in Update, for actions that don't need to use physics
    {

    }

    void ProcessFixedInput()//runs in FixedUpdate, for actions that use physics
	{
    	
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
