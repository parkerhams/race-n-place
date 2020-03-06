using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackScrap : MonoBehaviour
{
	//This script should be applied to "TrackScrap". It looks through all its child game objects (which should only be scrap) and picks which ones to activate for reach race.
	
	public int spawnAmount = 5;//how many pieces of scrap to spawn when a race starts
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	void OnEnable()
	{
		GameManager.ModeSwapped += ChooseScrapToSpawn;
	}
	void OnDisable()
	{
		GameManager.ModeSwapped -= ChooseScrapToSpawn;
	}
    
	void ChooseScrapToSpawn()
	{
		//respawn all scrap in new places during place mode so players can plan around it
		if(GameManager.Instance.mode != GameManager.Mode.Place)
		{
			if(GameManager.Instance.mode == GameManager.Mode.Race && GameManager.Instance.currentRound == 1)
			{
				//match has started, first round of scrap needs to be spawned in
				//let the function run as normal
			}
			else
			{
				return;
			}
		}
		if(transform.childCount <= spawnAmount)
		{
			Debug.LogError(gameObject + " track needs more scrap objects, or spawn amount needs to be reduced!");
			spawnAmount = Mathf.Max(0, transform.childCount - 2);
		}
		
		List<GameObject> unchosenScrap = new List<GameObject>();
		List<GameObject> chosenScrap = new List<GameObject>();
		
		foreach(Transform child in transform)
		{
			if(!child.gameObject.GetComponent<Scrap>())
			{
				Debug.Log("Scrap object didn't have scrap script!");
				continue;
			}
			unchosenScrap.Add(child.gameObject);
		}
		for(var i = 1; i <= spawnAmount; i++)//loop through all children, choose some scrap to be activated
		{
			int randomScrapIdx = Random.Range(0, unchosenScrap.Count);
			chosenScrap.Add(unchosenScrap[randomScrapIdx]);
			unchosenScrap.Remove(unchosenScrap[randomScrapIdx]);
		}
		foreach(GameObject g in chosenScrap)
		{
			g.GetComponent<Scrap>().RestockScrap(true);//activate all chosen scrap
		}
		foreach(GameObject g in unchosenScrap)
		{
			g.GetComponent<Scrap>().RestockScrap(false);//deactivate all unchosen scrap
		}
	}
}
