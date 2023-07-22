using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject roleTextPrefab;
    public GameObject powerUpTextPrefab;
    public GameObject boostedTextPrefab;

    private GameObject roleTextObject;
    private GameObject powerUpTextObject;
    private GameObject boostedTextObject;

    void Awake()
    {
        playerController = GetComponent<PlayerController>();

        if (playerController.IsLocalPlayer())
        {
            roleTextObject = Instantiate(roleTextPrefab);
            roleTextObject.transform.SetParent(FindObjectOfType<Canvas>().transform, false);

            powerUpTextObject = Instantiate(powerUpTextPrefab);
            powerUpTextObject.transform.SetParent(FindObjectOfType<Canvas>().transform, false);

            boostedTextObject = Instantiate(boostedTextPrefab);
            boostedTextObject.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        }
    }

    void Update()
    {
        if (playerController.IsLocalPlayer())
        {
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        TMP_Text roleText = roleTextObject.GetComponent<TMP_Text>();
        roleText.text = (playerController.roleManager.isSeeker ? "Seeking" : "Hiding");

        TMP_Text powerUpText = powerUpTextObject.GetComponent<TMP_Text>();
        if (playerController.storedPowerUp == PlayerController.PowerUp.None || playerController.storedPowerUp == null)
        {
            powerUpText.text = "Powerup: None";
        }
        else
        {
            powerUpText.text = "Powerup: " + GetCurrentPowerUp(playerController.storedPowerUp.Value);
        }

        TMP_Text boostedText = boostedTextObject.GetComponent<TMP_Text>();
        if (playerController.IsPowerUpActive)
        {
            boostedText.text = "Boosted";
        }
        else
        {
            boostedText.text = "";
        }
    }


    string GetCurrentPowerUp(PlayerController.PowerUp powerUp)
    {
        switch (powerUp)
        {
            case PlayerController.PowerUp.None:
                return "None";
            case PlayerController.PowerUp.SpeedBoost:
                return "Speed Boost";
            case PlayerController.PowerUp.JumpBoost:
                return "Jump Boost";
            default:
                return "None";
        }
    }
}
