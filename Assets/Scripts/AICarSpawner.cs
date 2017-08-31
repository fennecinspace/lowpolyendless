using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarSpawner : MonoBehaviour {
    GroundPlayerController playerController; // the player script will be used to get info like speed
    Transform playerTransform; // this will be used to get the pos for the car to instantiate the ai enemies
    public GameObject aiMesh; // the model.fbx for the ai 
    public int aiOnScreen = 4; // number of spawned ai detected by camera (i need to write a function to use camera to detect)
    
    void Start() {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<GroundPlayerController>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    } 

    /*
    void SpawnEnemyCar() {
         if (playerController.speed > 900 && aiOnScreen < 4) {

        }
    }    */ // this will be put on hold for sometime
    // write a function to use camera to detect number of ai spawned 
    // write a function to control the ai random right and left rare movement
}

