using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {
	public Text scoreText;
	public Text speedText;
	private GameObject uiData; // contains the player gameobject

	void Start () {
		getUIData();
	}
	
	void Update() {
		UIScoreUpdater();
		UISpeedUpdater();
	}

	private void UIScoreUpdater(){
		scoreText.text = "SCORE\n" + uiData.GetComponent<ScoreManager>().getScore(); // updating the score UI Text
	}

	private void UISpeedUpdater(){
		speedText.text = "SPEED\n" + (int)uiData.GetComponent<GroundPlayerController>().speed/10; // updating the speed UI Text
	}

	private void getUIData(){
		uiData = GameObject.FindGameObjectWithTag("Player"); // will refrence the player gameobject to get the score and speed
	}
}
