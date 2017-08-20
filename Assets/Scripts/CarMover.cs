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
        steerCar(); // going to steer the car
    }


    void steerCar() {
        float steering = Input.GetAxis("Horizontal") * 45f;
        float accelerate = Input.GetAxis("Vertical") * maxTorque;

        wheelColliders[0].steerAngle = steering;
        wheelColliders[1].steerAngle = steering;

        for (int i = 0; i < 4; i++) {
            wheelColliders[i].motorTorque = accelerate;
        }
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
}