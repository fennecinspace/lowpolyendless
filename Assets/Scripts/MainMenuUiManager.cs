using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUiManager : MonoBehaviour {
	public void LevelLoader() {
		SceneManager.LoadScene("Forest");
	}
}
