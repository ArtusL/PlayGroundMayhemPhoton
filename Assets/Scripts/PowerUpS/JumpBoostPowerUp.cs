using UnityEngine;
using Photon.Pun;

public class JumpBoostPowerUp : MonoBehaviourPunCallbacks
{
    public float jumpMultiplier = 2f;
    public float duration = 5f;
    public PowerUpSpawner Spawner { get; set; }

     void OnTriggerEnter(Collider other)
    {
        Debug.Log("Jump Boost Trigger entered by: " + other.name);
        PlayerController playerController = other.gameObject.GetComponentInParent<PlayerController>();
        if (playerController != null && playerController.IsLocalPlayer())
        {
            playerController.StorePowerUp(PlayerController.PowerUp.JumpBoost, jumpMultiplier, duration);
            Spawner.PowerUpTaken();
            Debug.Log("Attempting to destroy power-up object");
            Photon.Pun.PhotonNetwork.Destroy(this.gameObject);
        }
    }


    [PunRPC]
    public void Pickup(int playerID)
    {
        Debug.Log("Powerup Pickup called");
        GameObject playerGO = PhotonView.Find(playerID).gameObject;
        PlayerController player = playerGO.GetComponent<PlayerController>();

        Debug.Log("Jump Boost PowerUp picked up by: " + playerGO.name);

        player.ApplyJumpBoost(jumpMultiplier, duration);

        PhotonNetwork.Destroy(gameObject);
    }

}
