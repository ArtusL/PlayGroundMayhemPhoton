using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Text endGameText;

    public void ShowEndGameText(string playerName)
    {
        endGameText.text = $"Game ended. The loser is {playerName}.";
        endGameText.gameObject.SetActive(true);
    }
}


