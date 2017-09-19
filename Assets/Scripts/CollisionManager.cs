using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {
    public bool isColliding = false;
    public bool? colliderType = null;

    void OnCollisionEnter(Collision col) {
        isColliding = true;
        if (col.gameObject.tag == "LoadedTerrain")
            colliderType = false;
        else
            colliderType = true;
    }

    void OnCollisionExit(Collision col) {
        isColliding = false;
        colliderType = null;
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
