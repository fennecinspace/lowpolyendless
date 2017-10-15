using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    [HideInInspector]
    public GroundPlayerController playerController;
    private Rigidbody ai;
    private float lanePos; // this will be used to clamp me into my lane .. and i will unclamp when i try to change lanes

    public float speed; // this is my realtime speed
    
    [Header("Initial Info")]
    public float initSpeed; // the ai initiated speed
    public int aiType; // 3 types
    
    [Header("Car XZ Dimensions")] // for raycasting
    public float halfCarXsize = 0.68f; 
    public float halfCarZsize = 1.545f;
    
    [Header("Proximity Checker Info")] // to check for close objects
    public bool forwardProxEnabled;
    public bool backProxEnabled, leftProxEnabled, rightProxEnabled;
    public int forwardProxType, backProxType, leftProxType, rightProxType;

    private float otherAiSpeed; // this will contain the speed of what ever ai is in front of me 
    
    public bool laneChangingEnabled = false;

    void Start() {
        ai = gameObject.GetComponent<Rigidbody>();
        lanePos = ai.transform.position.x;
        otherAiSpeed = initSpeed;
        AiType();
        InitSpeedSetter();
    }

    void Update() {
        ProximityChecker();
    }

    void FixedUpdate() {
        AiManager();
        AiMover();
    }

    void AiMover() {
        Vector3 mover = new Vector3(0,0,1);
        mover.z *= speed;
        ai.velocity = mover * Time.deltaTime;

        //clamp to lane by +/- 0.01 margin which means 0.02 space to move 
        if (!laneChangingEnabled) // if i'm changing lanes i will not be clamped
            ai.transform.position = new Vector3(Mathf.Clamp(ai.transform.position.x, lanePos - .01f, lanePos + .01f), ai.transform.position.y, ai.transform.position.z);
    }

    void AiManager() {
        if (laneChangingEnabled)
            speed = otherAiSpeed;
        else
            speed = initSpeed;

        if (forwardProxEnabled && forwardProxType == 1) {
            speed = otherAiSpeed;
            //LaneChanger();
        }
    }

    int DoubleRaycaster(Vector3 pos, float maxDistance, float halfCarXsize, float halfCarZsize, string axis) { // will return 1 for AI and 2 for anything else and 0 for no collision
        Ray ray1, ray2;
        RaycastHit hit1, hit2;
        bool ray1hit,ray2hit;

        if (axis == "z") {
            ray1 = new Ray(new Vector3(pos.x + halfCarXsize, pos.y, pos.z + halfCarZsize), Vector3.forward);
            ray2 = new Ray(new Vector3(pos.x - halfCarXsize, pos.y, pos.z + halfCarZsize), Vector3.forward);

        }
        else if (axis == "x") {
            //maxDistance *= 2f;
            ray1 = new Ray(new Vector3(pos.x + halfCarXsize, pos.y, pos.z + halfCarZsize), Vector3.right);
            ray2 = new Ray(new Vector3(pos.x + halfCarXsize, pos.y, pos.z - halfCarZsize), Vector3.right);
        }

        else if (axis == "-x") {
            //maxDistance *= 2f;
            ray1 = new Ray(new Vector3(pos.x - halfCarXsize, pos.y, pos.z + halfCarZsize), Vector3.left);
            ray2 = new Ray(new Vector3(pos.x - halfCarXsize, pos.y, pos.z - halfCarZsize), Vector3.left);
        }

        else /*if (axis == "-z")*/ {
            ray1 = new Ray(new Vector3(pos.x + halfCarXsize, pos.y, pos.z - halfCarZsize), Vector3.back);
            ray2 = new Ray(new Vector3(pos.x - halfCarXsize, pos.y, pos.z - halfCarZsize), Vector3.back);
        }

        ray1hit = Physics.Raycast(ray1, out hit1, maxDistance);
        ray2hit = Physics.Raycast(ray2, out hit2, maxDistance);

        Debug.DrawRay(ray1.origin, ray1.direction, Color.blue);
        Debug.DrawRay(ray2.origin, ray2.direction, Color.blue);
            if (ray1hit == true && ray2hit == true) { // added verification for both for in case ray1 hits player and ray2 hits ai in that case hittype is ai to avoid ai collision problem when changing lanes
                if (hit1.transform.tag == "AI" && hit2.transform.tag == "AI") {  // this will solve the issue hitting 2 ais and one of them has higher speed .. so i match it and end up crashing to the second one
                    if(axis == "z") // checking for axis so that the speed doesn't take another value when prox is other than forward
                        if (hit1.transform.gameObject.GetComponent<AIController>().speed <= hit2.transform.gameObject.GetComponent<AIController>().speed)
                            otherAiSpeed = hit1.transform.gameObject.GetComponent<AIController>().speed;
                        else
                            otherAiSpeed = hit2.transform.gameObject.GetComponent<AIController>().speed;
                    return 1;
                }
                if (hit1.transform.tag == "AI") {
                    if (axis == "z")
                        otherAiSpeed = hit1.transform.gameObject.GetComponent<AIController>().speed; // i will get the speed of the ai in fornt of me so that i can match it and not crash into him
                    return 1;
                }
                if (hit2.transform.tag == "AI") {
                    if (axis == "z")
                        otherAiSpeed = hit2.transform.gameObject.GetComponent<AIController>().speed;
                    return 1;
                }
                return 2;
            }
            else if (ray1hit == true) {
                if (hit1.transform.tag == "AI") {
                    if (axis == "z")
                        otherAiSpeed = hit1.transform.gameObject.GetComponent<AIController>().speed;
                    return 1;
                }
                return 2; // for player
            }
            else if (ray2hit == true) {
                if (hit2.transform.tag == "AI") {
                    if (axis == "z")
                        otherAiSpeed = hit2.transform.gameObject.GetComponent<AIController>().speed;
                    return 1;
                }
                return 2; // for player
            }
            else
                return 0; // for nothing
        //will check ray 1 first if it hits it will check the RaycastHit var for the tag .. else it will do the same for ray2 .. if not it will return 0 for no RaycastHit 
    }

    void ProximityChecker() {
        // 0 for Nothing
        // 1 for Ai 
        // 2 for Player
        rightProxType = DoubleRaycaster(ai.transform.position, 3.0f, halfCarXsize, halfCarZsize, "x");
        leftProxType = DoubleRaycaster(ai.transform.position, 3.0f, halfCarXsize, halfCarZsize, "-x");
        forwardProxType = DoubleRaycaster(ai.transform.position, 1.0f, halfCarXsize, halfCarZsize, "z");
        backProxType = DoubleRaycaster(ai.transform.position, 1.0f, halfCarXsize, halfCarZsize, "-z");
        // true for in proximity and false otherwise
        rightProxEnabled = (rightProxType == 0) ? false : true;
        leftProxEnabled = (leftProxType == 0) ? false : true;
        forwardProxEnabled = (forwardProxType == 0) ? false : true;
        backProxEnabled = (backProxType == 0) ? false : true;
    }

    void InitSpeedSetter() {
        int probability = Random.Range(1, 100);
        if (probability > 80)
            initSpeed = 900;
        if (probability > 50)
            initSpeed = 700;
        else
            initSpeed = 500;
    }

    void AiType() {
        int probability = Random.Range(1, 100);
        if (probability > 90) // 9% chance
            aiType = 3; // type 3
        if (probability > 60) // 29% chance
            aiType = 2; // type 2
        else // 61 % chance
            aiType = 1; // type 1
    }
}

/*
 * ai type 1 ... will increase speed normally and decreases it as player decreases his speed too 
 * ai type 2 ... will change lane as player tries to change his lane too 
 * ai type 3 ... will decrease speed gradually when player is behind it and try to crash with him
 */


