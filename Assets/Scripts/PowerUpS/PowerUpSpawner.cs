using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PowerUpSpawner : MonoBehaviourPun
{
    public GameObject[] powerUpPrefabs;
    public float respawnDelay = 10f;

    private void Start()
    {
        Debug.Log("PowerUpSpawner Start method called");

        if (photonView.IsMine)
        {
            Debug.Log("Is owner of the PhotonView. Spawning first power-up.");
            SpawnPowerUp();
        }
    }

    private void SpawnPowerUp()
    {
        int powerUpIndex = Random.Range(0, powerUpPrefabs.Length);
        GameObject powerUp = PhotonNetwork.Instantiate(powerUpPrefabs[powerUpIndex].name, transform.position, Quaternion.identity);

        SpeedBoostPowerUp speedBoost = powerUp.GetComponent<SpeedBoostPowerUp>();
        if (speedBoost != null)
        {
            speedBoost.Spawner = this;
        }
        else
        {
            JumpBoostPowerUp jumpBoost = powerUp.GetComponent<JumpBoostPowerUp>();
            if (jumpBoost != null)
            {
                jumpBoost.Spawner = this;
            }
        }
    }
    public void PowerUpTaken()
    {
        StartCoroutine(RespawnPowerUp());
    }

    private IEnumerator RespawnPowerUp()
    {
        yield return new WaitForSeconds(respawnDelay);

        SpawnPowerUp();
    }
}
