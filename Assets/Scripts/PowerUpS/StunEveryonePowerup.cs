using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class StunEveryonePowerup : MonoBehaviourPunCallbacks
{
    public float stunDuration = 5f;
    public PowerUpSpawner Spawner { get; set; }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Stun PowerUp Trigger entered by: " + other.name);
        PlayerController playerController = other.gameObject.GetComponentInParent<PlayerController>();
        if (playerController != null && playerController.IsLocalPlayer())
        {
            playerController.StorePowerUp(PlayerController.PowerUp.Stun, 1f, stunDuration);
            Spawner.PowerUpTaken();
            Debug.Log("Attempting to destroy power-up object");
            Photon.Pun.PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
