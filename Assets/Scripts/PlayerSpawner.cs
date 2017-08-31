using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
    public GameObject[] players = new GameObject[1];
    public int playerToInstantiate = 0;

	// Use this for initialization
	void Start () {
        Instantiate(players[playerToInstantiate]);
    }
}
