using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameDurationUI : MonoBehaviour
{
    public Button addButton;
    public Button subtractButton;
    public TextMeshProUGUI durationText;
    private int selectedDurationIndex = 0;

    private readonly float[] durations = { 60f, 120f, 180f, 240f, 300f }; 

    private void Start()
    {
        UpdateDurationDisplay();
        addButton.onClick.AddListener(AddTime);
        subtractButton.onClick.AddListener(SubtractTime);
    }

    void UpdateDurationDisplay()
    {
        durationText.text = (durations[selectedDurationIndex] / 60f) + " minutes";
    }

    public void AddTime()
    {
        if (selectedDurationIndex < durations.Length - 1)
        {
            selectedDurationIndex++;
            UpdateDurationDisplay();
        }
    }

    public void SubtractTime()
    {
        if (selectedDurationIndex > 0)
        {
            selectedDurationIndex--;
            UpdateDurationDisplay();
        }
    }

    public void SaveSelection()
    {
        ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable
        {
            { "gameDuration", durations[selectedDurationIndex] }
        };
        Photon.Pun.PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
    }
}
