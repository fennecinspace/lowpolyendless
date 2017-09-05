using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
    private Transform subject;
    private Vector3 myPosition;
    public float distanceBehind = 5; // distance from camera to subject on z axis
    public float distanceAbove; // distance from camera to subject on y axis
    public bool leftAndRight = true;

    void Start () {
    }
	
	void Update () {
        if (subject == null) // will find player
            subject = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        distanceAbove = transform.position.y;

        if (leftAndRight == false)
            myPosition.x = 0f;
        else
            myPosition.x = subject.position.x; // takes subject position
        myPosition.y = distanceAbove; // modify position to be above subject
        myPosition.z = subject.position.z - distanceBehind;  // modify position to be behind subject and keep following it
        transform.position = myPosition; // assign position to this
	}

}
