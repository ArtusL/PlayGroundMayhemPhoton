using System.Collections;
using UnityEngine;

public interface IPowerUp
{
    void UsePowerUp(PlayerController playerController);
    void SetSpawner(PowerUpSpawner spawner);
    string GetPowerUpName();
}

