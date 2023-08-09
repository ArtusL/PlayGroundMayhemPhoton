using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;

public class EndGamePanel : MonoBehaviourPunCallbacks
{
    //public Button restartGameButton;
    public Button quitButton;

    public static EndGamePanel Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        LocalAudioManager localAudioManager = FindObjectOfType<LocalAudioManager>();
        if (localAudioManager != null)
        {
            localAudioManager.GetComponent<AudioSource>().Stop();
        }
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Leaving room...");
            PhotonNetwork.LeaveRoom(); 
        }
        else
        {
            Debug.Log("Not in a room. Returning to main menu...");
            PhotonNetwork.LoadLevel("MainMenu");
        }
         if (RoomManager.Instance != null)
         {
             Destroy(RoomManager.Instance.gameObject);
         }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
