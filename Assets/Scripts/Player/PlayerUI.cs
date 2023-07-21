//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class PlayerUI : MonoBehaviour
//{
//    public PlayerController playerController;
//    public Image staminaBar;
//    public TextMeshProUGUI staminaText;
//    public TextMeshProUGUI powerupText;
//    public TextMeshProUGUI roleText;

//    void Update()
//    {
//        if (playerController != null && playerController.photonView.IsMine)
//        {
//            UpdateStaminaBar();
//            UpdatePowerupText();
//            UpdateRoleText();
//        }
//    }

//    private void UpdateStaminaBar()
//    {
//        float maxStamina = playerController.GetMaxStamina();
//        float staminaPercentage = playerController.currentStamina / maxStamina;
//        staminaBar.fillAmount = staminaPercentage;
//        staminaText.text = Mathf.RoundToInt(staminaPercentage * 100) + "%";

//        if (staminaPercentage > 0.75f)
//        {
//            staminaBar.color = Color.green;
//        }
//        else if (staminaPercentage > 0.35f)
//        {
//            staminaBar.color = Color.yellow;
//        }
//        else
//        {
//            staminaBar.color = Color.red;
//        }
//    }

//    void UpdatePowerupText()
//    {
//        if (playerController.currentPowerUp != null)
//        {
//            playerController.hasPowerup = true;
//            powerupText.text = "Current Power-Up: " + playerController.currentPowerUp.GetPowerUpName();
//            Debug.Log("In UpdatePowerupText: currentPowerUp = " + playerController.currentPowerUp.GetPowerUpName());
//        }
//        else
//        {
//            playerController.hasPowerup = false;
//            powerupText.text = "No Power-Up";
//            Debug.Log("Player has no power-up");
//        }
//        Debug.Log("In UpdatePowerupText: hasPowerup = " + playerController.hasPowerup);
//    }

//    private void UpdateRoleText()
//    {
//        roleText.text = playerController.isSeeker ? "Role: Seeker" : "Role: Hider";
//    }
//}

