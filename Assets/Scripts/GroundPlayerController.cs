using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlayerController : MonoBehaviour {
    private Rigidbody Player;
    private Transform Wheels;
    [Header("Data")]
    public float speed = 800f;
    [Header("Limitations")]
    public float limit = 4.5f;
    public float wheelRotationDegree = 25f;
    public float meshRotationSpeed = 10f;
    public float changingLaneSpeed = 700f;
    public float moveForward = 1f, leftAndRight = 1f;
    public float upAndDown = 0.72f;

    private CollisionManager CollisionChecker;

    void Start () {
        Player = GetComponent<Rigidbody>();
        Wheels = transform.GetChild(1);
        CollisionChecker = GetComponent<CollisionManager>();
    }

    void FixedUpdate() {
        CarMover(); // will move the car l&r and forward
        SpeedManager(); // Manages Speed
        MeshUpdater(); // will update car Wheels and Car body when changing lanes
        //gameObject.GetComponent<CollisionManager>().CollisionDetection(); // is Managing COLLISIONS
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
        //Rotating the Wheels in relation with speed
        for (int i = 0 ; i < 4 ; i++)
            Wheels.GetChild(i).Rotate(Time.deltaTime * speed * 3, 0, 0);
        // Updating the Body to move left and right or stay idle
        if (Input.GetButton("Horizontal") && CollisionChecker.colliderType != false ) {
            if (Input.GetAxis("Horizontal") > 0) {
                // this will rotate the body to the right (previous value was  x : -1.449f and z : 2.759f)
                this.gameObject.transform.GetChild(0).localRotation = 
                    Quaternion.Lerp(this.gameObject.transform.GetChild(0).localRotation, Quaternion.Euler(0f, 0.122f, 1.5f), Time.deltaTime * meshRotationSpeed);
                Player.transform.rotation = 
                    Quaternion.Lerp(Player.transform.rotation, Quaternion.Euler(0f, 3f, 0f), Time.deltaTime * meshRotationSpeed);
                // this will rotate the wheels to the right
                for (int i = 0; i < 2; i++)
                    Wheels.GetChild(i).localRotation =
                        Quaternion.Lerp(Wheels.GetChild(i).localRotation, Quaternion.Euler(0f, wheelRotationDegree, 0f), Time.deltaTime * meshRotationSpeed);
            }
            else if (Input.GetAxis("Horizontal") < 0) {
                // this will rotate the body to the left (previous value was  x : -1.449f and z : -2.759f)
                this.gameObject.transform.GetChild(0).localRotation = 
                    Quaternion.Lerp(this.gameObject.transform.GetChild(0).localRotation, Quaternion.Euler(0f, 0.122f, -1.5f), Time.deltaTime * meshRotationSpeed);
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
                Quaternion.Lerp(this.gameObject.transform.GetChild(0).localRotation, Quaternion.Euler(- 0.001f * speed, 0f, 0f), Time.deltaTime * meshRotationSpeed);
            Player.transform.rotation = 
                Quaternion.Lerp(Player.transform.rotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * meshRotationSpeed);
            for (int i = 0; i < 2; i++)
                Wheels.GetChild(i).localRotation =
                    Quaternion.Lerp(Wheels.GetChild(i).localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * meshRotationSpeed);
        }
        /* Player.transform.rotation will rotate the entire car left and right 
         and gameObject.transform.GetChild(0).localRotation 
         will rotate the body without the wheels */
    }

    void SpeedManager() {
        if (CollisionChecker.isColliding == false) { // when no collision is happening 
            if (speed < 1500 && speed >= 800)  // will increase speed by time 
                speed += Time.deltaTime * 50;
            else if (speed < 800) // will increase speed eapidly when starting under 800
                speed += Time.deltaTime * 200;
        }
        else { // when colliding
            if (CollisionChecker.colliderType == false && speed > 500) // will decrease speed when coliding with boundries
                speed -= Time.deltaTime * 200;
        }
    }


    /*
    add breaks and acceleration inputs.. with mesh update for brake
    .. and maybe for acceleration not sure .. 
    maybe just maybe add gear mesh update in certain speeds 
    remember you're already on second gear so add 3 mesh updates for gear if you decide to add them
    add mobile inputs
    ... 
     */
}