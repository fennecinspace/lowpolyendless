using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour {
    [SerializeField]
    private GameObject[] PlayerMeshes = new GameObject[1];
    [SerializeField] private int meshToInstantiate = 0;
    [SerializeField] private GameObject instantiatedPlayer;

	// Use this for initialization
	void Start () {
        instantiatedPlayer = Instantiate(PlayerMeshes[meshToInstantiate]);
        //Debug.Log(" width is " + instantiatedPlayer.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds);
    }


    public GameObject GetPlayer() {
        return instantiatedPlayer;
    }
}
