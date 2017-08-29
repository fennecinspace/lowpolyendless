using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Rigidbody Player;
    private Transform Wheels;
    public float limit = 4.5f;
    public float speed = 800f;
    public float wheelRotationDegree = 25f;
    public float meshRotationSpeed = 10f;
    public float changingLaneSpeed = 700f;
    public float moveForward = 1f, leftAndRight = 1f;
    public float upAndDown = 0.72f;


    void Start () {
        Player = GetComponent<Rigidbody>();
        Wheels = this.gameObject.transform.GetChild(1);

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
        // will limit pos to a certain interval [-limit,limit]
        Player.position = new Vector3(Mathf.Clamp(Player.position.x, -limit, limit), upAndDown, Player.position.z);
    }

    void MeshUpdater() {
        //Rotating the Wheels in relation wiht speed
        for (int i = 0 ; i < 4 ; i++)
            Wheels.GetChild(i).Rotate(Time.deltaTime * speed * 3, 0, 0);

        // Updating the Body to move left and right or stay idle
        if (Input.GetButton("Horizontal") && gameObject.GetComponent<CollisionManager>().isColliding == false ) {
            if (Input.GetAxis("Horizontal") > 0) {
                // this will rotate the body to the right (previous value is 2.759f)
                this.gameObject.transform.GetChild(0).localRotation = 
                    Quaternion.Lerp(this.gameObject.transform.GetChild(0).localRotation, Quaternion.Euler(-1.449f, 0.122f, 1.5f), Time.deltaTime * meshRotationSpeed);
                Player.transform.rotation = 
                    Quaternion.Lerp(Player.transform.rotation, Quaternion.Euler(0f, 3f, 0f), Time.deltaTime * meshRotationSpeed);
                // this will rotate the wheels to the right
                for (int i = 0; i < 2; i++)
                    Wheels.GetChild(i).localRotation =
                        Quaternion.Lerp(Wheels.GetChild(i).localRotation, Quaternion.Euler(0f, wheelRotationDegree, 0f), Time.deltaTime * meshRotationSpeed);
            }
            else if (Input.GetAxis("Horizontal") < 0) {
                // this will rotate the body to the left (previous value is -2.759f)
                this.gameObject.transform.GetChild(0).localRotation = 
                    Quaternion.Lerp(this.gameObject.transform.GetChild(0).localRotation, Quaternion.Euler(-1.449f, 0.122f, -1.5f), Time.deltaTime * meshRotationSpeed);
                Player.transform.rotation = 
                    Quaternion.Lerp(Player.transform.rotation, Quaternion.Euler(0f, -3f, 0f), Time.deltaTime * meshRotationSpeed);
                // this will rotate the wheels to the left
                for (int i = 0; i < 2; i++)
                    Wheels.GetChild(i).localRotation =
                        Quaternion.Lerp(Wheels.GetChild(i).localRotation, Quaternion.Euler(0f, -wheelRotationDegree, 0f), Time.deltaTime * meshRotationSpeed);
            }

        }
        else { // when the body is idle
            this.gameObject.transform.GetChild(0).localRotation = 
                Quaternion.Lerp(this.gameObject.transform.GetChild(0).localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * meshRotationSpeed);
            Player.transform.rotation = 
                Quaternion.Lerp(Player.transform.rotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * meshRotationSpeed);
            Wheels.GetChild(0).localRotation =
                Quaternion.Lerp(Wheels.GetChild(0).rotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * meshRotationSpeed);
            Wheels.GetChild(1).localRotation =
                Quaternion.Lerp(Wheels.GetChild(1).rotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * meshRotationSpeed);
        }
        /* Player.transform will rotate the car body left and right 
         and gameObject.transform.GetChild(0).localRotation 
         will rotate the body without the wheels */
    }

    void SpeedManager() {
        if (gameObject.GetComponent<CollisionManager>().isColliding == true
            && gameObject.GetComponent<CollisionManager>().colliderType == false && speed > 800)
            speed -= Time.deltaTime * 200;
        else if (speed < 2100)  // will increase speed by time 
            speed += Time.deltaTime * 50;
    }

}