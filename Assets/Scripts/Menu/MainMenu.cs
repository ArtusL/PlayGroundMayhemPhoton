using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviourPunCallbacks
{
    public string menuName;
    public bool open;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void Open()
    {
        Debug.Log("Opening main menu.");
        open = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        Debug.Log("Closing main menu.");
        open = false;
        gameObject.SetActive(false);
    }
}

