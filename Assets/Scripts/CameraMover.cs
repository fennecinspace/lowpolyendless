﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
    private Transform Subject;
    private Vector3 myPosition;
    public float distanceBehind = 5; // distance from camera to subject on z axis
    public float distanceAbove; // distance from camera to subject on y axis

    void Start () {
        Subject = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        distanceAbove = transform.position.y;
    }
	
	void Update () {
        myPosition = Subject.position; // takes subject position
        myPosition.y = distanceAbove; // modify position to be above subject
        myPosition.z = myPosition.z - distanceBehind;  // modify position to be behind subject and keep following it
        transform.position = myPosition; // assign position to this
	}

}