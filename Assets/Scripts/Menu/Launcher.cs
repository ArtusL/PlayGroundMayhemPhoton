using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections.Generic;
using Photon.Realtime;
using System.Linq;
using System.Collections;

public class Launcher : MonoBehaviourPunCallbacks
{
    public LocalAudioManager localAudioManager;
    public static Launcher Instance;
    public GameDurationUI gameDurationUI;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text RoomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform PlayerListContent;
    [SerializeField] GameObject RoomListPrefab;
    [SerializeField] GameObject PlayerListPrefab;
    [SerializeField] GameObject StartGameButton;

    private bool isConnectingToMaster;

    void Awake()
    {
        Instance = this;
        
    }
    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
        if (localAudioManager != null)
        {
            localAudioManager.PlayMainMenuMusic();
        }
        else
        {
            Debug.LogError("LocalAudioManager not assigned!");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("MainMenu");
        Debug.Log("Joined Lobby");
        //if (nonNetworkAudio != null)
        //{
        //    nonNetworkAudio.PlayMainMenuMusic();
        //}
        //else
        //{
        //    Debug.LogError("NonNetworkAudio not assigned!");
        //}
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("RoomMenu");
        RoomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in PlayerListContent)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(PlayerListPrefab, PlayerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        StartGameButton.SetActive(PhotonNetwork.IsMasterClient);

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        StartGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Joined Failed: " + message;
        MenuManager.Instance.OpenMenu("ErrorMenu");
    }

    public void StartGame()
    {
        StartCoroutine(SaveDurationAndStartGame());
    }

    private IEnumerator SaveDurationAndStartGame()
    {
        gameDurationUI.SaveGameDuration();
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.LoadLevel(1);

        if (localAudioManager != null)
        {
            localAudioManager.StopMusic();
        }
    } 
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");
    }
    public void LeaveRoom()
    {
        Debug.Log("Leaving room...");
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
        Destroy(RoomManager.Instance.gameObject);
    }
    public override void OnLeftRoom()
    {
        Debug.Log("Left room successfully.");
        MenuManager.Instance.OpenMenu("MainMenu");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (roomListContent == null)
        {
            Debug.LogError("roomListContent is null. Cannot update room list.");
            return;
        }

        foreach (Transform trans in roomListContent)
        {
            if (trans == null)
            {
                Debug.LogError("Child of roomListContent is null. Skipping...");
                continue;
            }
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(RoomListPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }



    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListPrefab, PlayerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}