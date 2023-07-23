using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaPowerUp : MonoBehaviour
{
    public PowerUpSpawner Spawner { get; set; }

    private PhotonView photonView;


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("PowerUp Trigger entered by: " + other.name);
        PlayerController playerController = other.gameObject.GetComponentInParent<PlayerController>();
        if (playerController != null)
        {
            playerController.StorePowerUp(PlayerController.PowerUp.StaminaRefill, 0f, 0f);
            Debug.Log("Attempting to destroy power-up object");
            photonView.RPC("DestroyObjectAndRespawn", RpcTarget.MasterClient, photonView.ViewID);
        }
    }

    [PunRPC]
    public void Pickup(int playerID)
    {
        Debug.Log("Powerup Pickup called");
        GameObject playerGO = PhotonView.Find(playerID).gameObject;
        PlayerController player = playerGO.GetComponent<PlayerController>();

        Debug.Log("Speed Boost PowerUp picked up by: " + playerGO.name);

        player.RefillStamina();

        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void DestroyObjectAndRespawn(int photonViewID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = PhotonView.Find(photonViewID);
            if (photonView)
            {
                PowerUpSpawner spawner = photonView.GetComponent<StaminaPowerUp>().Spawner;
                spawner?.PowerUpTaken();
                PhotonNetwork.Destroy(photonView.gameObject);
            }
        }
    }
}
