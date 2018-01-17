using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

	[SerializeField]
	private int score;
	private int scoreAddition = 100;
	void Start () {
		initScore();
		
	}
	void OnTriggerExit(Collider AI){
		if (AI.transform.position.z < gameObject.transform.position.z && AI.gameObject.tag == "AI")
			 addScore(scoreAddition);
    }

	private void addScore(int value){
		setScore(getScore() + value);
	}

	private void initScore(){
		score = 0;
	}

	private int getScore(){
		return score;
	}

	private void setScore(int value){
		score = value;
	}
}
