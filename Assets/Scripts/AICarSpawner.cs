using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarSpawner : MonoBehaviour {
    public GameObject player;
    public GameObject[] aiMesh = new GameObject[1]; // the model.fbx for the ai 
    public int aiOnScreen = 0; // number of spawned ai detected by camera (i need to write a function to use camera to detect)
    public int aiOnScreenLimit = 4;
    public GameObject[] existantAis = new GameObject[4];

    GroundPlayerController playerController; // the player script will be used to get info like speed
    Transform playerTransform; // this will be used to get the pos for the car to instantiate the ai enemies

    float[] possiblePosesX = new float[4] { -4.45f, -1.55f, 1.55f, 4.45f };

    void Update () {
        if (player == null) {
            player = this.gameObject.GetComponent<PlayerSpawner>().GetPlayer();
            playerController = player.GetComponent<GroundPlayerController>();
            playerTransform = player.GetComponent<Transform>();
        }

        SpawnAiMesh();
    }

    
    void SpawnAiMesh () {
        if (playerController.speed > 800 && aiOnScreen < aiOnScreenLimit) {
            Vector3 aiPos = new Vector3(possiblePosesX[Random.Range(0, 4)],playerTransform.position.y, playerTransform.position.z + 40f);
            existantAis[aiOnScreen] = Instantiate(aiMesh[0]);
            existantAis[aiOnScreen++].transform.position = aiPos;
        }
    }
}

