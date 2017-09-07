using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
    public GameObject[] PlayerMeshes = new GameObject[1];
    public int meshToInstantiate = 0;
    public GameObject instantiatedPlayer;

	// Use this for initialization
	void Start () {
        instantiatedPlayer = Instantiate(PlayerMeshes[meshToInstantiate]);
    }

    public GameObject GetPlayer() {
        return instantiatedPlayer;
    }
}
