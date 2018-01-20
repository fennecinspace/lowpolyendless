using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

	[SerializeField]
	private int score; // score container
	private int scoreAddition = 100; // increase the score by this value

	void Start() {
		InitScore();
		InvokeRepeating("AddScore",0.0f,2.0f); // starting from 0 seconds (beginning of game) call AddScore every 2 seconds
	}

	void OnTriggerExit(Collider AI) { // gets called when other colliders exit collision with the player's SCORE COLLIDER 
		if (AI.transform.position.z < gameObject.transform.position.z && AI.gameObject.tag == "AI") // check if player really bypassed the AI
			ScoreAdderOnOvertake(scoreAddition); // increase score when player bypasses the AI
    }

	private void ScoreAdderOnOvertake(int value) {
		setScore(getScore() + value); // increase the score by "value" amount
	}

	private void AddScore(){
		if (gameObject.GetComponent<GroundPlayerController>().speed > 800)
			setScore(getScore() + 2); // add 2 to score
	}

	private void InitScore() {
		score = 0; // initialize score to 0 on game start
	}

	public int getScore() { 
		return score; // score getter
	}

	private void setScore(int value) {
		score = value;  // score setter
	}
}
