using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {
    [HideInInspector]
    public GroundPlayerController playerController;
    private Rigidbody ai;
    private Transform Wheels;
    private float lanePos; // this will be used to clamp me into my lane .. and i will unclamp when i try to change lanes
    //Mesh Properties
    private float meshRotationSpeed = 5f;
    private float wheelRotationDegree = 25f;

    [Header("Speed Settings")]
    public float initSpeed; // the ai initiated speed
    public float speed;
    public float brakingSpeedLimit;
    
    [Header("Car XZ Dimensions")] // for raycasting
    public float halfCarXsize; //0.68f
    public float halfCarZsize; //1.545f
    
    [Header("Proximity Checker Info")] // to check for close objects
    public bool forwardProxEnabled;
    public bool backProxEnabled, leftProxEnabled, rightProxEnabled ,forwardRightEnabled,forwardLeftEnabled,backRightEnabled, backLeftEnabled;
    public int forwardProxType, backProxType, leftProxType, rightProxType ,forwardRightProxType,forwardLeftProxType,backRightProxType,backLeftProxType;
    private float otherAiSpeed; // this will contain the speed of what ever ai is in front of me 


    [Header("Lane Changing Info")]
    public int currentLane; // lanes are .0.3.1.2.
    public int nextLane;
    public bool laneChangingEnabled = false;
    public bool goLaneRight = false, goLaneLeft = false;
    private float[] lanes = new float[4] { -4.45f, -1.50f, 1.50f, 4.45f };
    private bool frontIsChangingLane = false;
    
    void Start() {
        ai = gameObject.GetComponent<Rigidbody>();
        lanePos = ai.transform.position.x;
        Wheels = transform.GetChild(1); // getting the wheels game object to 
        InitNextLaneSetter(); // initial next lane value is the same as the lane i'm spawned in
        InitSpeedSetter(); // will set the initial speed
        GetHalfSize(); // will get half of my mesh size in X and Z axis for the raycasting later
        currentLane = nextLane; // setting the initial next lane as my current lane
        otherAiSpeed = initSpeed; // setting the otherAiSpeed as my speed for the initial value
        brakingSpeedLimit = (initSpeed == 500)? 500 : Random.Range(600,700); // seting the speed the ai stops breaking at when the player is breaking
    }

    void Update() {
        CurrentLaneSetter(); // will set the currentLane variable to which ever lane i'm in 
        ProximityChecker(); // will set the proximity and proxType variables to these values prox(True/False) and proxType(0/1/2)
        LaneChangingManager(); // will decide to change lane or not using a set of conditions
    }

    void FixedUpdate() {
        LaneChanger(); // will move the ai to another lane
        SpeedManager(); // will manage the speed
        AiMover(); // will move the ai to the front
        MeshUpdater(); // will update the mesh according to the AiMover and LaneChanger
    }
   
    void AiMover() {
        Vector3 mover = new Vector3(0, 0, 1);
        mover.z *= speed;
        ai.velocity = mover * Time.deltaTime;
        //clamp to lane by +/- 0.01 margin which means 0.02 space to move  
        if (!laneChangingEnabled) // if i'm changing lanes i will not be clamped
            ai.transform.position = new Vector3(Mathf.Clamp(ai.transform.position.x, lanePos - 0.01f, lanePos + 0.01f), ai.transform.position.y, ai.transform.position.z);
    }

    void LaneChanger() {
        if (laneChangingEnabled) {
            if (goLaneRight) // when going right add 0.025f everyframe
                ai.transform.position = new Vector3(ai.transform.position.x + 0.025f, ai.transform.position.y, ai.transform.position.z);
            if (goLaneLeft) // when going left deducte 0.025f everyframe
                ai.transform.position = new Vector3(ai.transform.position.x - 0.025f, ai.transform.position.y, ai.transform.position.z);
        }
    }

    void LaneChangingManager() {
        if (nextLane == currentLane) // if the current lane is the same as the next lane var then no changing in lanse is needed
            laneChangingEnabled = false;
        else
            laneChangingEnabled = true;

        if (forwardProxEnabled && !laneChangingEnabled) { // if no changing lane is being done check if we need to enable one by checking fornt raycast
            if (rightProxType != 1 && leftProxType != 1 && currentLane > 0 && currentLane < 3 && !frontIsChangingLane) {
                if (Random.Range(0, 2) == 0) { // 50 % chance going right 
                    if (!forwardRightEnabled && !backRightEnabled) {
                        nextLane = currentLane + 1; // the next lane becomes diffrent which meand the changing in lane will be enbaled and the lane changer functin will check for right or left
                        goLaneRight = true;
                        goLaneLeft = false;
                    }
                }
                else {
                    if (!forwardLeftEnabled && !backLeftEnabled) { // 50 % chance going left 
                        nextLane = currentLane - 1;
                        goLaneRight = false;
                        goLaneLeft = true;
                    }
                }
                return;
            }

            if (rightProxType != 1 && currentLane >= 0 && currentLane < 3 && !forwardRightEnabled && !backRightEnabled && !frontIsChangingLane) {
                nextLane = currentLane + 1;
                goLaneRight = true;
                goLaneLeft = false;
                return;
            }

            if (leftProxType != 1 && currentLane > 0 && currentLane <= 3 && !forwardLeftEnabled && !backLeftEnabled && !frontIsChangingLane) {
                nextLane = currentLane - 1;
                goLaneRight = false;
                goLaneLeft = true;
                return;
            }
        }

        if (!laneChangingEnabled) { // when the changing in lane is not enabled the goLaneRight && goLaneLeft will be false so that the mesh updater does the idle animation
            goLaneRight = false;
            goLaneLeft = false;
            return;
        }
    }

    void SpeedManager() {
        if (forwardProxEnabled && forwardProxType == 1) // if ai is in front of me i'll take its speed
            speed = otherAiSpeed;
        else if (laneChangingEnabled) // if car is changing lanes
            speed = otherAiSpeed;
        else if (speed < initSpeed) // if player is done braking and nothing is in front of me i'll increase my speed
            speed += 10;
        else
            speed = initSpeed; // if my speed is superior than my default speed, then i will set my speed to my default speed

        if (playerController.IsBraking() && speed > brakingSpeedLimit)  // if player is braking i will brake too
            speed -= 10;
        //speed = 5f;
    }

    int DoubleRaycaster(Vector3 pos, float maxDistance, float halfCarXsize, float halfCarZsize, string axis) { // will return 1 for AI and 2 for anything else and 0 for no collision
        Ray ray1, ray2;
        RaycastHit hit1, hit2;
        bool ray1hit,ray2hit;

        if (axis == "z") {
            ray1 = new Ray(new Vector3(pos.x + halfCarXsize , pos.y, pos.z + halfCarZsize), Vector3.forward);
            ray2 = new Ray(new Vector3(pos.x - halfCarXsize , pos.y, pos.z + halfCarZsize), Vector3.forward);
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
            ray1 = new Ray(new Vector3(pos.x + halfCarXsize , pos.y, pos.z - halfCarZsize), Vector3.back);
            ray2 = new Ray(new Vector3(pos.x - halfCarXsize , pos.y, pos.z - halfCarZsize), Vector3.back);
        }

        ray1hit = Physics.Raycast(ray1, out hit1, maxDistance);
        ray2hit = Physics.Raycast(ray2, out hit2, maxDistance);

        Debug.DrawRay(ray1.origin, ray1.direction, Color.green);
        Debug.DrawRay(ray2.origin, ray2.direction, Color.green);

        if (ray1hit == true && ray2hit == true) { // added verification for both for in case ray1 hits player and ray2 hits ai in that case hittype is ai to avoid ai collision problem when changing lanes
            if (hit1.transform.tag == "AI" && hit2.transform.tag == "AI") {  // this will solve the issue hitting 2 ais and one of them has higher speed .. so i match it and end up crashing to the second one
                if(axis == "z") // checking for axis so that the speed doesn't take another value when prox is other than forward
                    if (hit1.transform.gameObject.GetComponent<AIController>().speed <= hit2.transform.gameObject.GetComponent<AIController>().speed){
                        otherAiSpeed = hit1.transform.gameObject.GetComponent<AIController>().speed;
                        frontIsChangingLane = hit1.transform.gameObject.GetComponent<AIController>().laneChangingEnabled;
                    }
                    else {
                        otherAiSpeed = hit2.transform.gameObject.GetComponent<AIController>().speed;
                        frontIsChangingLane = hit2.transform.gameObject.GetComponent<AIController>().laneChangingEnabled;
                    }
                return 1;
            }
            if (hit1.transform.tag == "AI") {
                if (axis == "z"){
                    otherAiSpeed = hit1.transform.gameObject.GetComponent<AIController>().speed; // i will get the speed of the ai in fornt of me so that i can match it and not crash into him
                    frontIsChangingLane = hit1.transform.gameObject.GetComponent<AIController>().laneChangingEnabled;
                } 
                return 1;
            }
            if (hit2.transform.tag == "AI") {
                if (axis == "z"){
                    otherAiSpeed = hit2.transform.gameObject.GetComponent<AIController>().speed;
                    frontIsChangingLane = hit2.transform.gameObject.GetComponent<AIController>().laneChangingEnabled;
                }
                return 1;
            }
            return 2;
        }
        else if (ray1hit == true) {
            if (hit1.transform.tag == "AI") {
                if (axis == "z"){
                    otherAiSpeed = hit1.transform.gameObject.GetComponent<AIController>().speed; // i will get the speed of the ai in fornt of me so that i can match it and not crash into him
                    frontIsChangingLane = hit1.transform.gameObject.GetComponent<AIController>().laneChangingEnabled;
                }
                return 1;
            }
            return 2; // for player
        }
        else if (ray2hit == true) {
            if (hit2.transform.tag == "AI") {
                if (axis == "z"){
                    otherAiSpeed = hit2.transform.gameObject.GetComponent<AIController>().speed;
                    frontIsChangingLane = hit2.transform.gameObject.GetComponent<AIController>().laneChangingEnabled;
                }
                return 1;
            }
            return 2; // for player
        }
        else
            return 0; // for nothing
        //will check ray 1 first if it hits it will check the RaycastHit var for the tag .. else it will do the same for ray2 .. if not it will return 0 for no RaycastHit 
    }

    int SideRaycaster(Vector3 pos, float maxDistance, float halfCarXsize, float halfCarZsize, string axis) { // return same asDoubleRaycaster
        Ray ray;
        RaycastHit hit;
        bool rayhit;

        if (axis == "fr")
            ray = new Ray(new Vector3(pos.x + halfCarXsize + 2f , pos.y, pos.z ), Vector3.forward);
        else if (axis == "fl")
            ray = new Ray(new Vector3(pos.x - halfCarXsize - 2f , pos.y, pos.z ), Vector3.forward);
        else if (axis == "br")
            ray = new Ray(new Vector3(pos.x + halfCarXsize + 2f , pos.y, pos.z ), Vector3.back);
        else  // axis = bl
            ray = new Ray(new Vector3(pos.x - halfCarXsize - 2f , pos.y, pos.z ), Vector3.back);

        rayhit = Physics.Raycast(ray, out hit,maxDistance);
        Debug.DrawRay(ray.origin, ray.direction, Color.green);

        if (rayhit){ // when the ray hits a collider
            if (hit.transform.tag == "AI")
                return 1;  // check if it hit an ai then return 1
            else // if it hit player then return 2 
                return 2;
        }
        else return 0; // 0 for nothing
    }

    void ProximityChecker() { // this will call raycasting functions and check for close by objects // this will be called in Update
        // 0 for Nothing && 1 for Ai && 2 for Player
        rightProxType = DoubleRaycaster(ai.transform.position, 3.4f, halfCarXsize, halfCarZsize, "x");
        leftProxType = DoubleRaycaster(ai.transform.position, 3.4f, halfCarXsize, halfCarZsize, "-x");
        forwardProxType = DoubleRaycaster(ai.transform.position, 2.0f, halfCarXsize, halfCarZsize, "z");
        backProxType = DoubleRaycaster(ai.transform.position, 2.0f, halfCarXsize, halfCarZsize, "-z");
        forwardRightProxType = SideRaycaster(ai.transform.position, halfCarZsize * 4f, halfCarXsize, halfCarZsize, "fr"); // i used halfCarZsize * 5f for the max distance
        backRightProxType = SideRaycaster(ai.transform.position, halfCarZsize * 4f, halfCarXsize, halfCarZsize, "br");    // because i need to check if there is a car
        forwardLeftProxType = SideRaycaster(ai.transform.position, halfCarZsize * 4f, halfCarXsize, halfCarZsize, "fl"); // by more than 1 car unit + the space between them
        backLeftProxType = SideRaycaster(ai.transform.position, halfCarZsize * 4f, halfCarXsize, halfCarZsize, "bl");    // so that i garanty that i don't change the lane and get stuck
        // true for in proximity and false otherwise
        rightProxEnabled = (rightProxType == 0) ? false : true;
        leftProxEnabled = (leftProxType == 0) ? false : true;
        forwardProxEnabled = (forwardProxType == 0) ? false : true;
        backProxEnabled = (backProxType == 0) ? false : true;
        forwardRightEnabled = (forwardRightProxType == 0) ? false : true;
        backRightEnabled = (backRightProxType == 0) ? false : true;
        forwardLeftEnabled = (forwardLeftProxType == 0) ? false : true;
        backLeftEnabled = (backLeftProxType == 0) ? false : true;
    }
    
    void CurrentLaneSetter() { // this function will be called in update and it will set the current lane variable to the next lane variable whe the lane changing is done and that will disable laneChangingEnabled
        for (int i = 0; i < 4; i++)
            if (Mathf.Floor (ai.transform.position.x * 10) == Mathf.Floor(lanes[i]* 10)) { // this will change for example ... x.yz == xyj to xy.z == xy.j then to xy == xy .. so that we avoid float point precision problems
                currentLane = i;
                break;
            }
        lanePos = lanes[currentLane];
    }
    void MeshUpdater() {
        //Rotating the Wheels in relation with speed
        for (int i = 0; i < 4; i++)
            Wheels.GetChild(i).Rotate(Time.deltaTime * speed * 3, 0, 0);
        if (laneChangingEnabled) {
            if (goLaneRight) {
                // this will rotate the body to the right (previous value was  x : -1.449f and z : 2.759f)
                if (playerController.IsBraking() && speed > brakingSpeedLimit) // when breaking lean forward while turning right
                    this.gameObject.transform.GetChild(0).localRotation =
                        Quaternion.Slerp(this.gameObject.transform.GetChild(0).localRotation, Quaternion.Euler(2.214f, 0.122f, 1.5f), Time.deltaTime * meshRotationSpeed);
                else // this will just turn right
                    this.gameObject.transform.GetChild(0).localRotation =
                        Quaternion.Slerp(this.gameObject.transform.GetChild(0).localRotation, Quaternion.Euler(0f, 0.122f, 1.5f), Time.deltaTime * meshRotationSpeed);
                // this will rotate the entire car to the right
                ai.transform.rotation =
                    Quaternion.Slerp(ai.transform.rotation, Quaternion.Euler(0f, 3f, 0f), Time.deltaTime * meshRotationSpeed);
                // this will rotate the wheels to the right
                for (int i = 0; i < 2; i++)
                    Wheels.GetChild(i).localRotation =
                        Quaternion.Slerp(Wheels.GetChild(i).localRotation, Quaternion.Euler(0f, wheelRotationDegree, 0f), Time.deltaTime * meshRotationSpeed);
            }
            else if (goLaneLeft) {
                // this will rotate the body to the left (previous value was  x : -1.449f and z : -2.759f)
                if (playerController.IsBraking() && speed > brakingSpeedLimit) // when breaking lean forward while turning left
                    this.gameObject.transform.GetChild(0).localRotation =
                        Quaternion.Slerp(this.gameObject.transform.GetChild(0).localRotation, Quaternion.Euler(2.214f, 0.122f, -1.5f), Time.deltaTime * meshRotationSpeed);
                else // this will just turn left
                    this.gameObject.transform.GetChild(0).localRotation =
                        Quaternion.Slerp(this.gameObject.transform.GetChild(0).localRotation, Quaternion.Euler(0f, 0.122f, -1.5f), Time.deltaTime * meshRotationSpeed);
                // this will rotate the entire car to the left
                ai.transform.rotation =
                    Quaternion.Slerp(ai.transform.rotation, Quaternion.Euler(0f, -3f, 0f), Time.deltaTime * meshRotationSpeed);
                // this will rotate the wheels to the left
                for (int i = 0; i < 2; i++)
                    Wheels.GetChild(i).localRotation =
                        Quaternion.Slerp(Wheels.GetChild(i).localRotation, Quaternion.Euler(0f, -wheelRotationDegree, 0f), Time.deltaTime * meshRotationSpeed);
            }
        }
        else { // when the body is idle
            if (playerController.IsBraking() && speed > brakingSpeedLimit) // will get car to lean forward when breaking
                this.gameObject.transform.GetChild(0).localRotation =
                    Quaternion.Slerp(this.gameObject.transform.GetChild(0).localRotation, Quaternion.Euler(2.214f, 0f, 0f), Time.deltaTime * meshRotationSpeed);
            else // will get car body to go slightly up as speed increases
                this.gameObject.transform.GetChild(0).localRotation =
                    Quaternion.Slerp(this.gameObject.transform.GetChild(0).localRotation, Quaternion.Euler(-0.001f * speed, 0f, 0f), Time.deltaTime * meshRotationSpeed);
            //this will set the entire car to idle 
            ai.transform.rotation =
                Quaternion.Slerp(ai.transform.rotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * meshRotationSpeed);
            // this will set the wheels to idle
            for (int i = 0; i < 2; i++)
                Wheels.GetChild(i).localRotation =
                    Quaternion.Slerp(Wheels.GetChild(i).localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * meshRotationSpeed);
        }
    }
    
    // initial attributes setters
    void InitSpeedSetter() { // this will set the initSpeed variable
        int probability = Random.Range(1, 100);
        if (probability > 80)
            initSpeed = 900;
        if (probability > 50)
            initSpeed = 700;
        else
            initSpeed = 500; 
        speed = initSpeed; // so that the moment the ai spawns its speed is its initial Chosen Speed 
    }
    void GetHalfSize(){ // this will set the half car size when spawning the ai mesh .. for the raycasting 
        halfCarXsize = ai.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size.x / 2f - 0.055f;
        halfCarZsize = ai.transform.GetChild(0).GetComponent<MeshRenderer>().bounds.size.z / 2f - 0.055f;
    }
    void InitNextLaneSetter() { // this will set the initial next lane which is the same as the initial current lane 
        for (int i = 0; i < 4; i++)
            if (Mathf.Floor(ai.transform.position.x) == Mathf.Floor(lanes[i])) {
                nextLane = i;
                break;
            }
    }
}

/*
 * ai type 1 ... will increase speed normally and decreases it as player decreases his speed too 
 * ai type 2 ... will change lane as player tries to change his lane too 
 * ai type 3 ... will decrease speed gradually when player is behind it and try to crash with him
 */

    /* bool VerifyHitInfo(ref RaycastHit hit,string axis) { // will return true if he can pass
        if (HitIsNull(ref hit))
            return true;
        if (axis == "fr" || axis == "fl"){
            if (hit.collider.gameObject.GetComponent<AIController>().speed > speed)
                return true;
        }

        else { // if (axis == "br" || axis == "bl")
            if (hit.collider.gameObject.GetComponent<AIController>().initSpeed == hit.collider.gameObject.GetComponent<AIController>().speed && hit.collider.gameObject.GetComponent<AIController>().speed <= speed)
                return true;
        }
        return false;

    } */

    /*
        private RaycastHit hitfr,hitbr,hitfl,hitbl;
        bool EmptyLane(string axis){ // will check for frant's and back's lefts and rights to make the lanechanging more realistic 
        if (axis == "r"){
            if (hitfr.transform != null){
                if(hitfr.transform.tag == "AI"){
                    if (hitfr.transform.gameObject.GetComponent<AIController>().speed > speed)
                        return true;
                    else return false;
                }
                else return false;
            }
            else if (hitbr.transform != null){
                if(hitbr.transform.tag == "AI"){
                    if (hitbr.transform.gameObject.GetComponent<AIController>().speed <= speed)
                        return true;
                    else return false;
                }
                else return false;
            }
            else
                return true;
        }
        else { //if (axis == "l")
            if (hitfl.transform != null){
                if(hitfl.transform.tag == "AI"){
                    if (hitfl.transform.gameObject.GetComponent<AIController>().speed > speed)
                        return true;
                    else return false;
                }
            else return false;
            }
            else if (hitbl.transform != null){
                if(hitbl.transform.tag == "AI"){
                    if (hitbl.transform.gameObject.GetComponent<AIController>().speed <= speed)
                        return true;
                    else return false;
                }
            else return false;
            }
            else
                return true;
        }
    }
     */

