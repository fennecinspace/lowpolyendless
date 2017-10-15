using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {
    public bool isColliding = false;
    public bool? colliderType = null;
    public GameObject objectCollidedWith = null;

    void OnCollisionEnter(Collision col) {
        isColliding = true;
        if (col.gameObject.tag == "LoadedTerrain") //false for terrain
            colliderType = false;
        else {// true for AI or other objects
            objectCollidedWith = col.transform.gameObject;
            colliderType = true;
        }

    }

    void OnCollisionExit(Collision col) {
        isColliding = false;
        colliderType = null;
        objectCollidedWith = null;
    }

    public void CollisionDetection() {
        if (isColliding == true) {
            Debug.Log("Object is Colliding");
            if (colliderType == false)
                Debug.Log("Terrain");
            else
                Debug.Log("notTerrain");
        }
        else
            Debug.Log("Obj isn't colliding");
    }
}
