using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class JoinScreen : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> playerJoinPanels;
	
	private int maxPlayers = 4;
	
	[SerializeField]
	private GameObject joinPanelPrefab;
	
	private Player player;
	
	private List<Player> numOfPlayers = new List<Player>();
	
	private int NumberOfJoinedPlayers
	{
		get
		{
			if(joinedPlayers == null)
				return 0;
			else
			{
				return joinedPlayers.Count(c => c);
				//numOfPlayers.Add();
			}
				
		}
	}
	
	protected bool[] joinedPlayers; //this bool determines whether or not players 1-4 have joined or not
	
	private void Start()
	{
		InitializePlayerList();
	}
	
	/// <summary>
	/// use the bool joinedPlayers[] to change the state of the player index that joined
	/// </summary>
	private void CheckForJoinedPlayers()
	{
		for(int i = 0; i < maxPlayers; ++i)
		{
			if(joinedPlayers[i] == true)
				continue;
				
			print("number of joined players: " + GameManager.Instance.numOfPlayers);
			
			//access the "generic player" element attached to each player's Rewired component -- get their input
			if(player.GetButtonDown(""))
			{
				playerJoinPanels[i].transform.Find("playerText").GetComponent<Text>().text = "Player " + 
					(i + 1).ToString() + " Joined!";
				joinedPlayers[i] = true;
			}
		}
	}
	
	/// <summary>
	/// set number of players joined
	/// </summary>
	private void InitializePlayerList()
	{
		
	}
}
