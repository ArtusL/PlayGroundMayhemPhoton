using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEveryonePowerUp : MonoBehaviour
{
    public PowerUpSpawner Spawner { get; set; }
    public float StunDuration = 5.0f;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("PowerUp Trigger entered by: " + other.name);
        PlayerController playerController = other.gameObject.GetComponentInParent<PlayerController>();
        if (playerController != null && !playerController.HasPowerUp)
        {
            playerController.PickupPowerUp();
            playerController.StorePowerUp(PlayerController.PowerUp.StaminaRefill, 0f, 0f);
            Debug.Log("Attempting to destroy power-up object");
            photonView.RPC("DestroyObjectAndRespawn", RpcTarget.MasterClient, photonView.ViewID);
        }
    }
    //void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("PowerUp Trigger entered by: " + other.name);
    //    PlayerController playerController = other.gameObject.GetComponentInParent<PlayerController>();
    //    if (playerController != null && !playerController.HasPowerUp)
    //    {
    //        playerController.PickupPowerUp(); 
    //        playerController.StorePowerUp(PlayerController.PowerUp.Stun, 0f, 0f);
    //        Debug.Log("Attempting to destroy power-up object");
    //        photonView.RPC("DestroyObjectAndRespawn", RpcTarget.MasterClient, photonView.ViewID);
    //    }
    //}


    [PunRPC]
    public void Pickup(int playerID)
    {
        Debug.Log("Powerup Pickup called");
        GameObject playerGO = PhotonView.Find(playerID).gameObject;
        PlayerController player = playerGO.GetComponent<PlayerController>();

        Debug.Log("Stun PowerUp picked up by: " + playerGO.name);

        player.ApplyStunToOthers(StunDuration);

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
                PowerUpSpawner spawner = photonView.GetComponent<StunEveryonePowerUp>().Spawner;
                spawner?.PowerUpTaken();
                PhotonNetwork.Destroy(photonView.gameObject);
            }
        }
    }
}
