using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour {
    private Rigidbody Player;
    public float limit = 5.5f;
    public int speed = 1000;
    public float forwardMovement = 1;
    public float leftAndRight = 1, upAndDown = 0, enableForward = 1;

    void Start () {
        Player = GetComponent<Rigidbody>();
	}

    void FixedUpdate() {
        // multipliers will say which axis will have more effect than other .. and disable the axis that is multiplied by 0
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal") * leftAndRight, Input.GetAxisRaw("Vertical") * upAndDown, forwardMovement * enableForward); // get axis will smpoth left and right
        // normalizing to 1 and moving
        movement = movement.normalized; // normalizing movement vector
        Player.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // this will set collisionDetection on player as ContinuousDynamic which will stop ot from passing through boundary colliders when speeding up
        Player.velocity = movement * speed * Time.deltaTime; // moving the player and using dt to smooth transition 
    }
}
