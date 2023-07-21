//using Photon.Pun;
//using UnityEngine;

//public class SpeedPowerUp : MonoBehaviourPunCallbacks, IPowerUp
//{
//    public float speedIncrease = 2f;
//    public float duration = 5f;
//    public PowerUpSpawner spawner;

//    public void SetSpawner(PowerUpSpawner spawner)
//    {
//        this.spawner = spawner;
//    }

//    public void UsePowerUp(PlayerController playerController)
//    {
//        playerController.hasPowerup = true;
//        playerController.currentPowerUp = this;
//        playerController.photonView.RPC("RpcSpeedBoost", RpcTarget.All, speedIncrease, duration);

//        if (spawner != null)
//        {
//            spawner.PowerUpTaken();
//        }

//        PhotonNetwork.Destroy(gameObject);

//        playerController.currentPowerUp = this;
//        Debug.Log("Current power-up set to: " + this);
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.CompareTag("Player"))
//        {
//            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

//            if (playerController != null && playerController.currentPowerUp == null)
//            {
//                playerController.currentPowerUp = this;
//                gameObject.SetActive(false);
//            }
//        }
//    }

//    public string GetPowerUpName()
//    {
//        return "Speed Boost";
//    }
//}
