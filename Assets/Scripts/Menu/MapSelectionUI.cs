using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MapSelectionUI : MonoBehaviourPunCallbacks
{
    public Button previousButton;
    public Button nextButton;
    public TMP_Text mapNameText;

    private MapManager mapSelector;

    void Start()
    {
        mapSelector = MapManager.Instance;
        if (mapSelector == null)
        {
            Debug.LogError("MapSelector instance is null!");
            return;
        }

        UpdateButtonInteractivity();

        if (PhotonNetwork.IsMasterClient)
        {
            int defaultMapIndex = 0;
            mapSelector.SelectMap(defaultMapIndex);
            mapNameText.text = mapSelector.mapNames[defaultMapIndex];
        }
        else
        {
            int existingMapIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties["selectedMapIndex"];
            mapNameText.text = mapSelector.mapNames[existingMapIndex];
        }

        previousButton.onClick.AddListener(OnPreviousButtonClicked);
        nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    private void UpdateButtonInteractivity()
    {
        bool isMaster = PhotonNetwork.IsMasterClient;
        previousButton.interactable = isMaster;
        nextButton.interactable = isMaster;
    }
    private void OnPreviousButtonClicked()
    {
        int newIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties["selectedMapIndex"] - 1;
        if (newIndex < 0) newIndex = mapSelector.mapNames.Count - 1;

        mapSelector.SelectMap(newIndex);
    }

    private void OnNextButtonClicked()
    {
        int newIndex = (int)PhotonNetwork.CurrentRoom.CustomProperties["selectedMapIndex"] + 1;
        if (newIndex >= mapSelector.mapNames.Count) newIndex = 0;

        mapSelector.SelectMap(newIndex);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("selectedMapIndex"))
        {
            int index = (int)propertiesThatChanged["selectedMapIndex"];
            mapNameText.text = mapSelector.mapNames[index];
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        UpdateButtonInteractivity();
    }
}
