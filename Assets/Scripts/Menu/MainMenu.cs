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
    public void Open()
    {
        open = true;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        open = false;
        gameObject.SetActive(false);
    }

    //public override void OnLeftRoom()
    //{
    //    Debug.Log("Successfully left room, loading main menu scene...");

    //    try
    //    {
    //        SceneManager.LoadScene("MainMenu");
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("Error loading main menu scene: " + e);
    //    }
    //}


    //IEnumerator WaitAndOpenMenu()
    //{
    //    yield return null;
    //    MenuManager.Instance.OpenMenu("HomeScreenMenu");
    //}
}

