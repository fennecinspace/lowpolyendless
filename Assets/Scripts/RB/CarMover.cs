using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMover : MonoBehaviour {
    public WheelCollider[] wheelColliders = new WheelCollider[4]; // Getting Wheel Colliders
    public Transform[] wheelMeshes = new Transform[4]; // Getting Wheel Meshes to move them

    public float maxTorque = 5000f;

    void Update() {
        UpdateMeshPos(); // Rotating the wheels
        
    }

    void FixedUpdate() {
        moveCar(); // going to steer the car and move it
    }


    void moveCar() {
        float steering = Input.GetAxis("Horizontal") * 45f;
        float accelerate = Input.GetAxis("Vertical") * maxTorque;

        wheelColliders[0].steerAngle = steering;
        wheelColliders[1].steerAngle = steering;

        wheelColliders[2].motorTorque = accelerate;
        wheelColliders[3].motorTorque = accelerate;

    }

    void UpdateMeshPos() {
        for (int i = 0; i < 4; i++) { 
            Quaternion quat;
            Vector3 pos;

            wheelColliders[i].GetWorldPose(out pos, out quat);
            wheelMeshes[i].position = pos;
            wheelMeshes[i].rotation = quat;
        }
    }
    /*
    void calcCenterOfMass() {
        Rigidbody Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        Debug.Log(Player.centerOfMass);
    }

    void setCenterOfMass() {
        Rigidbody Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        Player.centerOfMass = new Vector3(0f, 0f, 0f);
    }
    */
}