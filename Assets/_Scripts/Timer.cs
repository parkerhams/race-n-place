using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
	//TODO: add a few seconds of delay when a round starts before the timer starts (equivalent to how much time controls are locked before the mode begins)
	//TODO: add timer blinking red when time runs low (and sound effect?)
	public float timeRemaining = 10f;
	float timeLimit = 10f;
	bool timerRunning = false;
	
	public float raceTimeLimit = 30f;
	public float placeTimeLimit = 15f;
	public float raceTimeLimitAfterSomeoneFinishes = 15f;
	bool isPaused = false;
	
	public GameObject timerObject;
	
	public bool isEnabled = true;//so it can be toggled to allow testing without interference
	
	void OnEnable()
	{
		GameManager.ModeSwapped += CancelTimer;
		GameManager.ModeSwapped += ForceStartTimer;
		GameManager.RaceEnded += PauseTimer;
	}
	void OnDisable()
	{
		GameManager.ModeSwapped -= CancelTimer;
		GameManager.ModeSwapped -= ForceStartTimer;
		GameManager.RaceEnded -= PauseTimer;
	}
	
    // Start is called before the first frame update
    void Start()
    {
	    timerObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
	    if(timerRunning && isEnabled && !isPaused)
	    {
	    	timeRemaining -= Time.deltaTime;
	    	timerObject.GetComponent<TextMeshProUGUI>().text = timeRemaining.ToString("n1");
	    	if(timeRemaining <= 0)
	    	{
	    		timeRemaining = 0;
	    		EndTimer();
	    	}
	    	else if(timeRemaining < 5f)
	    	{
	    		timerObject.GetComponent<TextMeshProUGUI>().color = Color.red;
	    	}
	    }
    }
    
	void ToggleInterface(bool enabled)
	{
		if(enabled)
		{
			timerObject.SetActive(true);
		}
		else
		{
			timerObject.SetActive(false);
		}
	}
	
	public void ForceStartTimer()//when a mode swap occurs, always start the timer
	{
		if(GameManager.Instance.mode == GameManager.Mode.Race)//it has switched to race mode, use race mode timer
		{
			StartTimer(raceTimeLimit);
		}
		else if(GameManager.Instance.mode == GameManager.Mode.Place)
		{
			StartTimer(placeTimeLimit);
		}
	}
    
	void StartTimer(float duration)
	{
		timerRunning = true;
		ToggleInterface(true);
		timeLimit = duration;
		timeRemaining = duration;
		timerObject.GetComponent<TextMeshProUGUI>().color = Color.white;
	}
	
	void EndTimer()
	{
		timerRunning = false;
		ToggleInterface(false);
		GameManager.Instance.SwapMode();
	}
    
	void CancelTimer()
	{
		if(!timerRunning)
		{
			//don't need to do anything, timer was already stopped
			return;
		}
		timerRunning = false;
		ToggleInterface(false);
	}
	
	public void PauseTimer()
	{
		timerRunning = false;
		isPaused = true;
	}
	public void ResumeTimer()
	{
		timerRunning = true;
		isPaused = false;
	}
}
