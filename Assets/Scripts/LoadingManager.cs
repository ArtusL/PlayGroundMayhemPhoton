using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadMap());
    }

    private IEnumerator LoadMap()
    {
        int mapIndex = GetSelectedMapIndex();
        ActivateMap(mapIndex);

        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("gameMaps");
    }

    private int GetSelectedMapIndex()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("map"))
        {
            return (int)PhotonNetwork.CurrentRoom.CustomProperties["map"];
        }
        else
        {
            Debug.LogError("Map index not found in custom properties!");
            return 0;
        }
    }

    private void ActivateMap(int mapIndex)
    {
    }
}
