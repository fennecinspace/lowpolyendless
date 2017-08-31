using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ForestLevelSpawner : MonoBehaviour {
    [Header("Game Objects")]
    public GameObject terrain;
    public GameObject Parent;
    private Transform Subject;

    [Header("Terrain Tile Spawning")]
    public int numberOfStartTiles = 3;
    public int numberOfTilesOnScreen = 4;
    private float terrainLength = 77.55f;
    private float beginSpawn = 0f;
    private List<GameObject> tilesOnScreen = new List<GameObject>();

    [Header("Tree Spawning")]
    public float numberOfTrees = 50;
    public float roadSideTreeNumber = 3;
    public GameObject[] Trees = new GameObject[4];
    public GameObject roadSideTree;

    [Header("Terrain Boundary Colliders")]
    public Vector3 colliderCenter = new Vector3(-5.5f,5.0f,0.0f);
    public Vector3 colliderSize = new Vector3(0.1f,10.0f,77.55f);

    
    void Start () {
        Subject = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        for (int i=0; i< numberOfStartTiles; i++)
            SpawnTerrain();
    }

    void Update() {
        DestroyTerrain();
        if (Subject.position.z > beginSpawn - numberOfStartTiles * terrainLength)
            SpawnTerrain();   
    }

    void SpawnTerrain() {
        tilesOnScreen.Add(Instantiate(terrain,new Vector3(0, 0, 1 * beginSpawn), Quaternion.identity, Parent.transform)); // spawning terrain after the last one in default rotation and setting parent 
        beginSpawn += terrainLength; // increase the point to spawn in for next terrain 
        SpawnTrees(tilesOnScreen[tilesOnScreen.Count - 1].transform); // will spawn Trees
        DestroyColliders("Tree"); // will get collider for all side objects and will destroy them to improve performance
        SpawnTerrainColliders(tilesOnScreen[tilesOnScreen.Count - 1]); // will send last spawned collider to the method that will spawn colldiers on the left and right side
        DestroyOutOfBoundery("Tree", -38.7f, 38.7f); // will remove trees that went out of boundery
    }

    /* beginspawn will start on 0 when game starts
     * after that it will increment by terrain size 
     * so that the next time spawning will be done 
     * right after the ending of the last spawned terrain
     * which will make the terrain into one long terrain
     */

    void DestroyTerrain() {
        if (tilesOnScreen.Count > numberOfTilesOnScreen) { // if number of spawned terrain exceeds allowed .. will destroy terrain
            Destroy(tilesOnScreen[0]); // remove terrain from scene
            tilesOnScreen.RemoveAt(0); // remove destroyed terrain from the list of existing terrain in scene
        }
    }

    void SpawnTrees(Transform terrainPos) {
        int x = Random.Range(1, 3); // to decide on left or right side for road trees
        
        for (int i = 0; i < roadSideTreeNumber; i++) {
            switch (x) {
                case 1: {// spawn right side
                        TreeSide(terrainPos, 7.0f, 8.5f, 150.0f, 210.0f, 0.0f, 0.0f);
                        x = 2;
                        break;
                    }
                case 2: {// spawn left side
                        TreeSide(terrainPos, -8.5f, -7.0f, -30.0f, 30.0f, 0.0f, 0.0f);
                        x = 1;
                        break;
                    }
            }
        }

        for (int i = 0; i < numberOfTrees; i++) {
            // spawn right side
            TreeSide(terrainPos, 7.7f, 37.7f);
            // spawn left side 
            TreeSide(terrainPos, -7.7f, -37.7f);
        }
    }

    void TreeSide(Transform terrainPos, float min, float max) {
        Vector3 temp = terrainPos.position; // getting terrain position

        while (true) {
            temp.z += Random.Range(-37.7f, 37.7f);  // getting random x and z
            temp.x += Random.Range(min, max);   // for initiating tree
            if (!VerifyEmptySpace(temp)) {
                Instantiate(Trees[Random.Range(0, Trees.Length)],temp,Quaternion.Euler(Random.Range(-5f, 5f), Random.Range(0f, 360f), Random.Range(-5f, 5f)), terrainPos); // initiating tree
                break;
            }
        }
    }

    //OverLoading Tree Side to give it how much a spawned object can turn on the y axis .. and turn on the x axis (tilt to the side)
    void TreeSide(Transform terrainPos, float min, float max, float turnDegreeMin, float turnDegreeMax, float tiltDegreeMin, float tiltDegreeMax) {
        Vector3 temp = terrainPos.position; // getting terrain position

        while (true) {
            temp.z += Random.Range(-37.7f, 37.7f);  // getting random x and z
            temp.x += Random.Range(min, max);   // for initiating tree
            if (!VerifyEmptySpace(temp)) {
                Instantiate(roadSideTree, temp, Quaternion.Euler(Random.Range(tiltDegreeMin, tiltDegreeMax), Random.Range(turnDegreeMin, turnDegreeMax), Random.Range(-5f, 5f)), terrainPos); // initiating tree
                break;
            }
        }
    }

    bool VerifyEmptySpace(Vector3 pos) { // takes pos for instantiation 
        pos.y += 50; // adds to y pos to raycast from above the pos
        Ray ray = new Ray(pos, Vector3.down); // raycasting
        //Debug.Log("Raycasted"); 
        return Physics.Raycast(ray); // returning -> true for "collider found" ## -> false for "no collider found"  
    }

    void SpawnTerrainColliders(GameObject terrain) {
        BoxCollider leftTerrainCollider = terrain.AddComponent<BoxCollider>();
        BoxCollider rightTerrainCollider = terrain.AddComponent<BoxCollider>();

        leftTerrainCollider.center = colliderCenter;
        leftTerrainCollider.size = colliderSize;

        rightTerrainCollider.center = colliderCenter = new Vector3(-colliderCenter.x, colliderCenter.y, colliderCenter.z);
        rightTerrainCollider.size = colliderSize;
    }

    void DestroyColliders(string tag) {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject anObject in objects) // loops on each element of the array ! 
            Destroy(anObject.GetComponent<Collider>()); // delete 1 collider per object for each object from static array
    }

    void DestroyOutOfBoundery(string tag, float min, float max) { // takes tag of objects to destory if surpassed min and max bounderies on x axis
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag); // gets all objects with that tag 

        foreach (GameObject anObject in objects)
            if (anObject.transform.position.x < min || anObject.transform.position.x > max) // verifies if object surpassed bounderies 
                Destroy(anObject); // destroys the object 
    }
}

