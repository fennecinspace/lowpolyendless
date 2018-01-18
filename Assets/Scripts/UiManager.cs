using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {
	public Text scoreText;
	public Text speedText;
	private GameObject uiData;

	void Start () {
		getUIData();
	}
	
	void Update() {
		UIScoreUpdater();
		UISpeedUpdater();
	}

	private void UIScoreUpdater(){
		scoreText.text = "Score\n" + uiData.GetComponent<ScoreManager>().getScore();
	}

	private void UISpeedUpdater(){
		speedText.text = "Speed\n" + uiData.GetComponent<GroundPlayerController>().speed;
	}

	private void getUIData(){
		uiData = GameObject.FindGameObjectWithTag("Player");
	}
}
