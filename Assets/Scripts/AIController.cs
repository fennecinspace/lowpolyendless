using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    private Rigidbody ai;
    
    [HideInInspector]
    public GroundPlayerController playerController;
    public float speed = 800;
    
    void Start() {
        ai = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        SpeedManager();
        AiMover();
    }

    void AiMover() {
        Vector3 mover = new Vector3(0,0,1);
        mover.z *= speed;
        ai.velocity = mover * Time.deltaTime;
    }

    void SpeedManager() {   
        // first batch of cars will start slow and speed up .. and the other batchs will just spawned speeding 
        // i need to find a way to make a predifined path for each lane so that the ai goes to it when changing lanes .
    }
}
