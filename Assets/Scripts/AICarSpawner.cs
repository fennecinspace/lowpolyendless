using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarSpawner : MonoBehaviour {
    public GameObject player;
    public GameObject[] aiMesh = new GameObject[1]; // the model.fbx for the ai 
    public int aiOnScreen = 0; // number of spawned ai detected by camera (i need to write a function to use camera to detect)
    public int aiOnScreenLimit = 4;

    GroundPlayerController playerController; // the player script will be used to get info like speed
    Transform playerTransform; // this will be used to get the pos for the car to instantiate the ai enemies
    GameObject[] existantAis = new GameObject[4];

    void Update () {
        if (player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
            playerController = player.GetComponent<GroundPlayerController>();
            playerTransform = player.GetComponent<Transform>();
        }

        //SpawnEnemyCar();
        //DestroyEnemyCar();
    }

    /*
    void SpawnEnemyCar () {
        if (playerController.speed > 900 && aiOnScreen < aiOnScreenLimit) {
            playerTransform.position.x = // add a table that has 1.55 -1.55 4.45 -4.45 and randomize each one 
            Instantiate(aiMesh,playerTransform);

        }
    }/*

    /*void DestroyEnemyCar () {
        
    }*/

    // this will be put on hold for sometime
    // write a function to control the ai random right and left rare movement
}

