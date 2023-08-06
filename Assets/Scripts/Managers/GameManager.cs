using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public EndGamePanel endGamePanel;

    public float gameDuration = 15f;  
    private float timer;
    private bool gameIsOver = false;
    public TextMeshProUGUI endGameText; 
    public TextMeshProUGUI timerText;
    public static GameManager Instance { get; private set; }

    public bool GameStarted { get; private set; } = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gameDuration"))
        {
            gameDuration = (float)PhotonNetwork.CurrentRoom.CustomProperties["gameDuration"];
        }

        if (PhotonNetwork.IsMasterClient)
        {
            timer = gameDuration;
        }

        if (endGameText != null)
            endGameText.gameObject.SetActive(false);

        if (timerText != null)
            timerText.text = "Time left: " + Mathf.Ceil(timer).ToString();

        StartGame();
    }


    void Update()
    {
        if (PhotonNetwork.IsMasterClient && !gameIsOver)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                timer = 0f;
                gameIsOver = true;
                photonView.RPC("EndGame", RpcTarget.All);
            }
        }

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    [PunRPC]
    void EndGame()
    {
        GameStarted = false;
        PlayerRoleManager[] players = FindObjectsOfType<PlayerRoleManager>();
        foreach (PlayerRoleManager player in players)
        {
            if (player.isSeeker)
            {
                if (endGameText != null)
                {
                    endGameText.text = player.photonView.Owner.NickName + " has lost the game";
                    endGameText.gameObject.SetActive(true);
                }
                StartCoroutine(LoadMenuAfterDelay(10f));
                return;
            }
        }
        EndGamePanel.Instance.Show();
    }

    IEnumerator LoadMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        endGamePanel.Show();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(timer);
        }
        else
        {
            timer = (float)stream.ReceiveNext();
        }
    }

    public void StartGame()
    {
        timer = gameDuration;
        gameIsOver = false;
        GameStarted = true;

        EndGamePanel.Instance.HidePanel();
        endGameText.text = "";
        endGameText.gameObject.SetActive(false);
    }
    public void RestartGame()
    {
        gameIsOver = false;
        timer = gameDuration;
        endGameText.gameObject.SetActive(false); 

        EndGamePanel.Instance.Hide();
    }
}
