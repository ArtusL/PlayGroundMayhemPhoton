using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += onSceneloaded;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= onSceneloaded;
    }
    void onSceneloaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1)
        {
            StartCoroutine(SetupMapAndSpawnPlayer());
        }
    }

    IEnumerator SetupMapAndSpawnPlayer()
    {
        int mapIndex = GetSelectedMapIndex();
        Debug.Log("Selected Map Index: " + mapIndex);

        GameObject[] maps = GameObject.FindGameObjectsWithTag("Map");

        foreach (var map in maps)
        {
            Debug.Log("Disabling Map: " + map.name);
            map.SetActive(false);
        }

        if (mapIndex >= 0 && mapIndex < maps.Length)
        {
            Debug.Log("Enabling Map: " + maps[mapIndex].name);
            maps[mapIndex].SetActive(true);
            yield return new WaitForSeconds(0.1f);
            SpawnManager.Instance.UpdateSpawnPoints(maps[mapIndex]);
        }
        else
        {
            Debug.LogError("Invalid map index!");
        }

        yield return new WaitForSeconds(0.1f);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
    }
    //void onSceneloaded(Scene scene, LoadSceneMode loadSceneMode)
    //{
    //    if (scene.buildIndex == 1) // Assuming the game scene's build index is 1
    //    {
    //        int mapIndex = GetSelectedMapIndex();

    //        GameObject[] maps = GameObject.FindGameObjectsWithTag("Map");

    //        foreach (var map in maps)
    //        {
    //            map.SetActive(false);
    //        }

    //        if (mapIndex >= 0 && mapIndex < maps.Length)
    //        {
    //            maps[mapIndex].SetActive(true);
    //        }
    //        else
    //        {
    //            Debug.LogError("Invalid map index!");
    //        }

    //        // Other code for instantiating players etc.
    //    }
    //}

    int GetSelectedMapIndex()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("map"))
        {
            return (int)PhotonNetwork.CurrentRoom.CustomProperties["map"];
        }
        else
        {
            Debug.LogError("Map index not found in custom properties!");
            return -1;
        }
    }
}