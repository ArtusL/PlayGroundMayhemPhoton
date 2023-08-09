using UnityEngine;
using Photon.Pun;

public class JumpBoostPowerUp : MonoBehaviourPunCallbacks
{
    public float jumpMultiplier = 2f;
    public float duration = 5f;
    public PowerUpSpawner Spawner { get; set; }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("PowerUp Trigger entered by: " + other.name);
        PlayerController playerController = other.gameObject.GetComponentInParent<PlayerController>();
        if (playerController != null && !playerController.HasPowerUp)
        {
            playerController.PickupPowerUp();
            playerController.StorePowerUp(PlayerController.PowerUp.JumpBoost, jumpMultiplier, duration);
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

        player.ApplySpeedBoost(jumpMultiplier, duration);

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
                PowerUpSpawner spawner = photonView.GetComponent<JumpBoostPowerUp>().Spawner; 
                spawner?.PowerUpTaken();
                PhotonNetwork.Destroy(photonView.gameObject);
            }
        }
    }

}
