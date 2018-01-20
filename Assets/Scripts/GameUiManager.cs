using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUiManager : MonoBehaviour {
	private GameObject uiData; // contains the player gameobject

	[Header("InGame UI")]
	public GameObject GameInfo;
	public Text scoreText;
	public Text speedText;
	
	[Header("PauseMenu UI")]
	public GameObject PauseMenu;

	void Start () {
		Time.timeScale = 1;
		getUIData();
		HideUIElement(PauseMenu);
	}
	
	void Update() {
		UIScoreUpdater();
		UISpeedUpdater();
		PauseMenuManager();
	}

	// Game INFO
	private void UIScoreUpdater(){
		scoreText.text = "SCORE\n" + uiData.GetComponent<ScoreManager>().getScore(); // updating the score UI Text
	}

	private void UISpeedUpdater(){
		speedText.text = "SPEED\n" + (int)uiData.GetComponent<GroundPlayerController>().speed/10; // updating the speed UI Text
	}

	private void getUIData(){
		uiData = GameObject.FindGameObjectWithTag("Player"); // will refrence the player gameobject to get the score and speed
	}

	// Pause Menu
	
	private void PauseMenuManager(){
		if (Input.GetKeyDown(KeyCode.P)) {
			if (Time.timeScale == 1) { // will pause game
				Time.timeScale = 0;
				HideUIElement(GameInfo);
				ShowUIElement(PauseMenu);
			}
			else {
				Time.timeScale = 1;
				HideUIElement(PauseMenu);
				ShowUIElement(GameInfo);
			}
		}
	}

	// Other


	public void HideUIElement(GameObject element){
		element.SetActive(false);
	}

	public void ShowUIElement(GameObject element){
		element.SetActive(true);
	}
}
