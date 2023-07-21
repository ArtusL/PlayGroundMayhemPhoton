//using UnityEngine;
//using Photon.Pun;

//public class NetworkGameManager : MonoBehaviourPunCallbacks
//{
//    public static NetworkGameManager instance;
//    public UIManager uiManager;

//    public GameManager gameManager;

//    private void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//        }
//        else
//        {
//            Debug.LogWarning("Multiple instances of NetworkGameManager!");
//        }
//    }

//    public void EndGame(string loserPlayerName)
//    {
//        if (!PhotonNetwork.IsMasterClient) return;

//        gameManager.SetGameEnded(true);
//        Debug.Log("Game ended. The last one tagged is the loser.");
//        photonView.RPC("RpcShowEndGameText", RpcTarget.All, loserPlayerName);

//        foreach (var player in gameManager.players)
//        {
//            player.canMove = false;
//        }
//    }

//    [PunRPC]
//    public void RpcShowEndGameText(string playerName)
//    {
//        uiManager.ShowEndGameText(playerName);
//    }
//}

