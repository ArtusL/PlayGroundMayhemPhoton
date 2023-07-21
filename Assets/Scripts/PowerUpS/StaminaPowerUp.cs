//using System.Collections;
//using UnityEngine;

//public class StaminaPowerUp : MonoBehaviour, IPowerUp
//{
//    public PowerUpSpawner spawner;
//    public float powerUpDuration = 5f;

//    public void SetSpawner(PowerUpSpawner spawner)
//    {
//        this.spawner = spawner;
//    }

//    public void UsePowerUp(PlayerController player)
//    {
//        Debug.Log("Applying Stamina Boost powerup.");
//        player.hasPowerup = true;
//        player.currentStamina = player.GetMaxStamina();
//        player.currentPowerUp = this;
//        Debug.Log("Current power-up set to: " + this);
//        player.StartCoroutine(ApplyPowerUp(player));
//    }

//    private IEnumerator ApplyPowerUp(PlayerController player)
//    {
//        yield return new WaitForSeconds(powerUpDuration);

//        player.hasPowerup = false;

//        if (spawner != null)
//        {
//            Debug.Log("Informing spawner that power-up has been taken.");
//            spawner.PowerUpTaken();
//        }

//        if (this != null)
//        {
//            Destroy(gameObject);
//        }
//    }


//    private void OnTriggerEnter(Collider other)
//    {
//        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();

//        if (playerController != null && !playerController.hasPowerup)
//        {
//            UsePowerUp(playerController);
//            gameObject.SetActive(false);
//        }
//    }

//    public string GetPowerUpName()
//    {
//        return "Stamina Boost";
//    }
//}
