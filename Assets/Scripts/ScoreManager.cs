using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

	[SerializeField]
	private int score; // score container
	private int scoreAddition = 100; // increase the score by this value

	void Start() {
		initScore();
	}

	void OnTriggerExit(Collider AI) { // gets called when other colliders exit collision with the player's SCORE COLLIDER 
		if (AI.transform.position.z < gameObject.transform.position.z && AI.gameObject.tag == "AI") // check if player really bypassed the AI
			scoreAdderOnOvertake(scoreAddition); // increase score when player bypasses the AI
    }

	private void scoreAdderOnOvertake(int value) {
		setScore(getScore() + value); // increase the score by "value" amount
	}

	private void initScore() {
		score = 0; // initialize score to 0 on game start
	}

	public int getScore() { 
		return score; // score getter
	}

	private void setScore(int value) {
		score = value;  // score setter
	}
}
