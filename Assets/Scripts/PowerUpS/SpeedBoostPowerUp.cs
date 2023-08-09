using UnityEngine;
using Photon.Pun;

public class SpeedBoostPowerUp : MonoBehaviourPunCallbacks
{
    public float speedMultiplier = 2f;
    public float duration = 5f;
    public PowerUpSpawner Spawner { get; set; }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("PowerUp Trigger entered by: " + other.name);
        PlayerController playerController = other.gameObject.GetComponentInParent<PlayerController>();
        if (playerController != null && !playerController.HasPowerUp)
        {
            playerController.PickupPowerUp();
            playerController.StorePowerUp(PlayerController.PowerUp.SpeedBoost, speedMultiplier, duration);
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

        player.ApplySpeedBoost(speedMultiplier, duration);

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
                PowerUpSpawner spawner = photonView.GetComponent<SpeedBoostPowerUp>().Spawner; 
                spawner?.PowerUpTaken();
                PhotonNetwork.Destroy(photonView.gameObject);
            }
        }
    }
}
