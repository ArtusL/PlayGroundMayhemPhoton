using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameDurationUI : MonoBehaviourPunCallbacks
{
    public Button addButton;
    public Button subtractButton;
    public TextMeshProUGUI durationText;
    private int selectedDuration;

    private readonly int maxDuration = 300;
    private readonly int minDuration = 60;//remember to change back to 60

    private void Start()
    {
        selectedDuration = minDuration;
        UpdateDurationDisplay();
        UpdateButtonInteractivity();
        addButton.onClick.AddListener(AddTime);
        subtractButton.onClick.AddListener(SubtractTime);
    }

    private void UpdateButtonInteractivity()
    {
        bool isMaster = PhotonNetwork.IsMasterClient;
        addButton.interactable = isMaster;
        subtractButton.interactable = isMaster;
    }

    void UpdateDurationDisplay()
    {
        durationText.text = (selectedDuration / 60f) + " minutes";
    }

    public void AddTime()
    {
        if (selectedDuration < maxDuration)
        {
            selectedDuration += 60;
            UpdateDurationDisplay();
            SaveGameDuration();
        }
    }

    public void SubtractTime()
    {
        if (selectedDuration > minDuration)
        {
            selectedDuration -= 60;//same here remember
            UpdateDurationDisplay();
            SaveGameDuration();
        }
    }

    public void SaveGameDuration()
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.LogError("CurrentRoom is null. Cannot save selection.");
            return;
        }
        ExitGames.Client.Photon.Hashtable roomProps = new ExitGames.Client.Photon.Hashtable
        {
            { "gameDuration", selectedDuration }
        };
        Photon.Pun.PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
        Debug.Log("Saved game duration: " + selectedDuration);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("gameDuration"))
        {
            selectedDuration = (int)propertiesThatChanged["gameDuration"];
            UpdateDurationDisplay();
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        UpdateButtonInteractivity();
    }
}
