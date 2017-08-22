using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour {
    private Rigidbody Player;
    public float limit = 5.5f;
    public int speed = 1000;
    public float moveForward = 1;
    public float leftAndRight = 1;
    public float upAndDown = 0.72f;

    void Start () {
        Player = GetComponent<Rigidbody>();
        Player.centerOfMass = new Vector3(0.0f, 0.0f, 0.0f);
	}

    void FixedUpdate() {

        float moveHorizontal = Input.GetAxis("Horizontal") * leftAndRight;
        Vector3 movement = new Vector3(moveHorizontal , upAndDown, moveForward); // get axis will smpoth left and right
        movement.x *= speed;
        movement.z *= speed;
        Player.velocity = movement * Time.deltaTime; // moving the player and using dt to smooth transition 

        Player.position = new Vector3(Mathf.Clamp(Player.position.x, -limit, limit), upAndDown, Player.position.z);
        Player.transform.rotation = Quaternion.Euler(0.0f,0.0f,0.0f); 
    }
}
