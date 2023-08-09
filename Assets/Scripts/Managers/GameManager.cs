using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public LocalAudioManager localAudioManager;
    public EndGamePanel endGamePanel;
    private bool countdownInProgress = false;

    private float gameDuration;  
    private float timer;
    private bool gameIsOver = false;
    public TextMeshProUGUI endGameText; 
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI countdownText;
    public static GameManager Instance { get; private set; }

    public bool GameStarted { get; private set; } = false;
    public bool GameOver { get; private set; } = false;

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
        localAudioManager.PlayAudienceSound();
        StartCoroutine(StartCountdown());
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gameDuration"))
        {
            gameDuration = (int)PhotonNetwork.CurrentRoom.CustomProperties["gameDuration"];
            Debug.Log("Game Duration Loaded: " + gameDuration);
        }
        else
        {
            Debug.LogError("gameDuration key not found in custom properties.");
        }

        if (PhotonNetwork.IsMasterClient)
        {
            timer = gameDuration;
        }

        if (endGameText != null)
            endGameText.gameObject.SetActive(false);


        //StartGame();
    }

    private IEnumerator StartCountdown()
    {
        countdownInProgress = true; 
        yield return new WaitForSeconds(1f);

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            localAudioManager.PlayCountdownSound();

            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "";
        localAudioManager.PlayBuzzerSound();
        //localAudioManager.PlayAudienceSound();
        StartGame();
        countdownInProgress = false; 
    }

    void Update()
    {
        if (!GameStarted)
        {
            return;
        }

        if (PhotonNetwork.IsMasterClient && !gameIsOver)
        {
            timer -= Time.deltaTime;
            if (timer <= 3f && timer > 0f && !countdownInProgress)
            {
                StartCoroutine(EndCountdown());
            }

            if (timer <= 0f)
            {
                timer = 0f;
                gameIsOver = true;
                photonView.RPC("EndGame", RpcTarget.All);
            }
        }

        if (!countdownInProgress && timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            timerText.gameObject.SetActive(false); 
        }
    }

    private IEnumerator EndCountdown()
    {
        countdownInProgress = true;
        timerText.gameObject.SetActive(false); 

        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            localAudioManager.PlayCountdownSound();

            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "";
        localAudioManager.PlayBuzzerSound();

        countdownInProgress = false;
    }

    [PunRPC]
    void EndGame()
    {
        GameStarted = false;
        GameOver = true;

        PlayerController[] playerControllers = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in playerControllers)
        {
            player.StartCoroutine(player.Stun(float.MaxValue));
        }
        PlayerRoleManager[] roleManagers = FindObjectsOfType<PlayerRoleManager>();
        foreach (PlayerRoleManager player in roleManagers)
        {
            if (player.isSeeker)
            {
                if (endGameText != null)
                {
                    endGameText.text = player.photonView.Owner.NickName + " has lost the game";
                    endGameText.gameObject.SetActive(true);
                }
                StartCoroutine(LoadMenuAfterDelay(5f));
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
}
