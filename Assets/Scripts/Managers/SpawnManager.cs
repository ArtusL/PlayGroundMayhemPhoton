using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    List<SpawnPoint> spawnPoints;

    void Awake()
    {
        Instance = this;
        spawnPoints = new List<SpawnPoint>();
    }

    public void UpdateSpawnPoints(GameObject activeMap)
    {
        spawnPoints.Clear(); 
        spawnPoints.AddRange(activeMap.GetComponentsInChildren<SpawnPoint>(true)); 
    }

    public Transform GetSpawnPoint()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
        //Debug.Log("Chosen Spawn Point: " + spawnPoint.position);
        return spawnPoint;
    }
}

