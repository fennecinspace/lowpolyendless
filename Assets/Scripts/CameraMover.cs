using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour {
    private Transform subject;
    private Vector3 myPosition;
    [Header("Camera Position")]
    public float distanceBehind = 5f; // distance from camera to subject on z axis
    public float distanceAbove = 2.268f; // distance from camera to subject on y axis
    public bool alowLeftRight = true;
    [Header("Camera Rotation")]
    public Quaternion theRotation;

    void Start () {
        theRotation = this.gameObject.transform.rotation; //(13.544f, 0f, 0f)
    }
	
	void Update () {
        if (subject == null) // will find player
            subject = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        
        if (alowLeftRight == false)
            myPosition.x = 0f;
        else
            myPosition.x = subject.position.x; // takes subject position

        myPosition.y = distanceAbove; // modify position to be above subject
        myPosition.z = subject.position.z - distanceBehind;  // modify position to be behind subject and keep following it
        transform.position = myPosition; // assign position to this
        transform.localRotation = theRotation;
    }
}
