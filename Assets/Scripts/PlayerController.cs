using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody Player;
    public float limit = 5.5f;
    public float speed = 800;
    public float changingLaneSpeed = 700;
    public float moveForward = 1, leftAndRight = 1;
    public float upAndDown = 0.72f;

    void Start () {
        Player = GetComponent<Rigidbody>();
	}

    void FixedUpdate() {
        CarMover(); // will move the car l&r and forward
        MeshUpdater(); // will update car Wheels and Car body when changing lanes
        SpeedManager(); // Manages Speed
        gameObject.GetComponent<CollisionManager>().CollisionDetection(); // is Managing COLLISIONS
    }

    void CarMover() {
        float moveHorizontal = Input.GetAxis("Horizontal") * leftAndRight;
        Vector3 movement = new Vector3(moveHorizontal, upAndDown, moveForward); // get axis will smpoth left and right
        movement.x *= changingLaneSpeed;
        movement.z *= speed;
        Player.velocity = movement * Time.deltaTime; // moving the player and using dt to smooth transition 

        Player.position = new Vector3(Mathf.Clamp(Player.position.x, -limit, limit), upAndDown, Player.position.z);
        Player.transform.rotation = Quaternion.identity;
    }

    void MeshUpdater() {

    }

    void SpeedManager() {
        if (gameObject.GetComponent<CollisionManager>().isColliding == true 
            && gameObject.GetComponent<CollisionManager>().colliderType == false && speed > 800 )
                speed -= Time.deltaTime * 200;
        else if (speed < 2100)  // will increase speed by time 
                speed += Time.deltaTime * 50;
    } 
}
