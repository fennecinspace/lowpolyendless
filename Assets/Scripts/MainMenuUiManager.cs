using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUiManager : MonoBehaviour {
	void Update() {
		LevelLoader();
	}

	void LevelLoader() {
		if(Input.GetKeyDown(KeyCode.F))
			SceneManager.LoadScene("Forest");
	}
}
