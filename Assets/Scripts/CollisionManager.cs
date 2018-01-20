using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {
    public bool isColliding = false;
    public int colliderType = 0;
    public GameObject objectCollidedWith = null;

    void OnCollisionEnter(Collision col) {
        isColliding = true;
        if (col.gameObject.tag == "LoadedTerrain") //false for terrain
            colliderType = 1;
        else {// true for AI or other objects
            objectCollidedWith = col.transform.gameObject;
            colliderType = 2;
        }

    }

    void OnCollisionExit(Collision col) {
        isColliding = false;
        colliderType = 0;
        objectCollidedWith = null;
    }

    public void CollisionDetection() {
        if (isColliding == true) {
            Debug.Log("Object is Colliding");
            if (colliderType == 1)
                Debug.Log("Terrain");
            else
                Debug.Log("notTerrain");
        }
        else
            Debug.Log("Obj isn't colliding");
    }
}
