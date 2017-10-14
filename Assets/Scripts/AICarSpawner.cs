using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarSpawner : MonoBehaviour {
    public GameObject player;
    public GameObject[] aiMesh = new GameObject[1]; // the model.fbx for the ai 
    public int aiOnScreen = 0; // number of spawned ai detected by camera (i need to write a function to use camera to detect)
    public int aiOnScreenLimit = 20;
    public GameObject[] existantAis;
    public Vector3 boxcastSize = new Vector3(1.2f, 0.9f, 2.65f);
    private GroundPlayerController playerController; // the player script will be used to get info like speed
    private Transform playerTransform; // this will be used to get the pos for the car to instantiate the ai enemies
    public float[] possiblePosesX = new float[4] { -4.45f, 1.50f, 4.45f, -1.50f };
    private Vector3 furthestCarPos;

    void Start() {
        existantAis = new GameObject[aiOnScreenLimit];
        player = GetComponent<PlayerSpawner>().GetPlayer();
        playerController = player.GetComponent<GroundPlayerController>();
        playerTransform = player.GetComponent<Transform>();
        furthestCarPos = playerTransform.position;
    }

    void Update() {
        GetFurthestCarPos();
        if (playerController.speed > 900 && aiOnScreen < aiOnScreenLimit)
            SpawnAiMesh();
        DestroyAiMesh();
        //Debug.Log(aiMesh[0].transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds.size * 0.28f);
    }
    
    void SpawnAiMesh() {
        Vector3 aiPos = playerTransform.position;
        while (true) {
            aiPos.x = GetRandomPossesX();
            if(aiOnScreen == 0)  
                aiPos.z = furthestCarPos.z + Random.Range(90f, 100f); // will spawn the first ai really far for higer go 160/200
            else
                aiPos.z = furthestCarPos.z + Random.Range(4f, 7f); // will assign the spawing Z-Pos using the pos of the furthest car away and add to it between 10 and 15 units
            if (!VerifyEmptyBoxSpace(aiPos)) {
                existantAis[aiOnScreen] = Instantiate(aiMesh[0], aiPos, aiMesh[0].transform.rotation); // will instantiate the AI
                existantAis[aiOnScreen].GetComponent<AIController>().playerController = playerController;
                //Debug.Log(furthestCarPos.z);
                aiOnScreen++;
                break;
            }
        }
    }

    bool VerifyEmptyBoxSpace(Vector3 pos) { // takes pos for instantiation 
        Collider[] hitResults = Physics.OverlapBox(pos, boxcastSize); // will spawn an overlap box and return all the colliders that box might have touched or contained 
        for (int i = 0; i < hitResults.Length; i++)
            if (hitResults[i].transform.tag == "AI" || hitResults[i].transform.tag == "Player") // this will check if the bex overlaped the Player or an AI 
                return true;
        return false;
    }

    void DestroyAiMesh() {
        for (int i = 0; i < aiOnScreen; i++) { // check every ai position in the ais on screen array
            if (existantAis[i].transform.position.z < playerTransform.position.z - 10) { // verify if ai is behind player
                Destroy(existantAis[i]); // destory AI
                existantAis[i] = null; // fill ai pos in array with null 
                for (int j = i; j < existantAis.Length - 1;) { // loop to move all ais after destoyed one one case behind to fill the first cases of the array
                    existantAis[j] = existantAis[++j];
                }
                aiOnScreen--; // decrement the aiOnScreen Variabale after destying the ai
            }
        }
    }

    void GetFurthestCarPos() { // this will find the furthest car away and update the furthestCarPos variabale so i can spawn another one 10 to 15 units after it later
        for (int i = 0; i < aiOnScreen; i++)
            if (existantAis[i].transform.position.z > furthestCarPos.z)
                furthestCarPos.z = existantAis[i].transform.position.z;
    }

    float GetRandomPossesX() { // this will return a random pos from the random pos table and try to not chose previous random ones
        float randomPosX; // will later store a random pos on x to check if the 2 ai spawned before this one will have that position in that case we will generate another and check again
        while (true) {
            randomPosX = possiblePosesX[Random.Range(0, 4)];
            if (aiOnScreen > 3) { // added thius ciondition because in the beginning of the game we might have 1 ai on screen or less and that would give us an Index Out Of Bounds Exception
                if (randomPosX != existantAis[aiOnScreen - 1].transform.position.x && randomPosX != existantAis[aiOnScreen - 3].transform.position.x)
                    return randomPosX;
            }
            else return randomPosX;
        }
    }
}

// this is for verefying empty space (box = half box) : (2.4, 1.8, 5.3) = (1.2, 0.9, 2.65) * 2

/*
void OnDrawGizmos() {
Gizmos.color = new Color(1, 0, 0, 0.5F);
Gizmos.DrawCube(new Vector3(1.55f, 2, 0), player.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size * 0.28f);
Debug.Log(player.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds.size * 0.28f);
}

// if you add this change condition for spawnAiMesh Call in update from < to <= and add this inside verefy empty in an else with a condition for aiOnScreen < aiOnScreenLimit
void RepossisionAiMesh(Vector3 aiPos) {
for (int i = 0; i < aiOnScreen; i++) // check every ai position in the ais on screen array
    if (existantAis[i].transform.position.z < playerTransform.position.z - 20) { // verify if ai is behind player
        existantAis[i].transform.position = aiPos;
    }
}
*/
