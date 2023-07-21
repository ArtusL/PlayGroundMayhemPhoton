using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PowerUpSpawner : MonoBehaviourPunCallbacks
{
    public GameObject[] powerUpPrefabs;
    private bool powerUpIsSpawned = false;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnPowerUp());
        }
    }

    private IEnumerator SpawnPowerUp()
    {
        while (!powerUpIsSpawned)
        {
            Debug.Log("PowerUpSpawner: Powerup is not spawned, spawning a new powerup.");
            GameObject powerUpToSpawn = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
            GameObject powerUp = PhotonNetwork.Instantiate(powerUpToSpawn.name, transform.position, transform.rotation);

            IPowerUp powerUpComponent = powerUp.GetComponent<IPowerUp>();

            if (powerUpComponent != null)
            {
                powerUpComponent.SetSpawner(this);
                powerUpIsSpawned = true;
            }
            else
            {
                Debug.LogError("Spawned power-up does not implement IPowerUp interface.");
            }

            yield return null;
        }
    }

    public void PowerUpTaken()
    {
        Debug.Log("Power up taken.");
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Power up taken on the server.");
            powerUpIsSpawned = false;
            Debug.Log("PowerUpIsSpawned after being set to false: " + powerUpIsSpawned);

            StartCoroutine(DelayedSpawnPowerUp());
        }
    }

    private IEnumerator DelayedSpawnPowerUp()
    {
        yield return new WaitForSeconds(10f);
        StartCoroutine(SpawnPowerUp());
    }
}
