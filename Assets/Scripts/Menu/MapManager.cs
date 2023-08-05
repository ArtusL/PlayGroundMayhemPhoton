using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public List<string> mapNames;

    public int currentMapIndex = 0;

    public static MapManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SelectMap(int mapIndex) 
    {
        Hashtable customProperties = new Hashtable();
        customProperties.Add("map", mapIndex);
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "selectedMapIndex", mapIndex } }); 
        }
    }
}
