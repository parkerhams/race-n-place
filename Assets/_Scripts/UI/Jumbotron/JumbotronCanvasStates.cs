using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumbotronCanvasStates : MonoBehaviour
{
	public enum PortraitStates
	{
		Idle,
		Celebration,
		HitTrap,
		Victory,
		Defeat
	}
	
	private List<GameObject> characterPortraits = new List<GameObject>(); //will always be 2-4 players
	
	private PortraitStates characterPortraitStates;
	
	private void Start()
	{
		characterPortraitStates = PortraitStates.Idle; //the default - each portrait is idle until an event
		//populate the portraits here based on how many players there are
    }
}
