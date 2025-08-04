using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;


public class MultiplayerLevelManager : MonoBehaviourPunCallbacks
{
    public int maxKills = 4;
    public GameObject gameOverPopUp;
    public TMPro.TextMeshProUGUI winnerText;
    public Transform[] spawnPoints;
    // Timer related
    [SerializeField] private float gameDuration = 300f;
    [SerializeField] private TMPro.TextMeshProUGUI timerText;
    // Sound related 
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;
    // Timer related
    private float currentTime;
    private const string TimerKey = "GameTimer";
    // Win related
    private bool isGameOver;
    private Photon.Realtime.Player winnerPlayer = null;
    public Button playAgainButton;

    public string mainMenuScene;



    private void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            SpawnPlayer();

            if (PhotonNetwork.IsMasterClient)
            {
                StartGameTimer(); // Only Master Client sets the timer
            }

            StartCoroutine(InitializeTimerAfterDelay()); // Add delay before checking timer
        }
    }
    private IEnumerator InitializeTimerAfterDelay()
    {
        yield return new WaitForSeconds(1f); // Wait for room properties to sync

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TimerKey, out object timerEndValue))
        {
            Debug.Log($"TimerKey found after delay: {timerEndValue}");
        }
        else
        {
            Debug.LogError("TimerKey still not found after delay!");
        }
    }
    private void Update()
    {
        if (!isGameOver)
        {
            UpdateGameTimer();
        }
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("Only one player left in the room. Showing end game panel...");
            ShowEndGamePanel();
        }
    }
    private void ShowEndGamePanel()
    {
        if (gameOverPopUp != null)
        {
            gameOverPopUp.SetActive(true);
        }
    }
    private void SpawnPlayer()
    {
        Transform selectedSpawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        PhotonNetwork.Instantiate("Multiplayer Player", selectedSpawnPoint.position, selectedSpawnPoint.rotation);

    }
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        int kills = targetPlayer.GetScore();
        if (kills == maxKills)
        {
            EndGame(targetPlayer);
            StorePersonalBest();
        }
    }
    void StorePersonalBest()
    {
        int currentScore = PhotonNetwork.LocalPlayer.GetScore();
        PlayerData playerData = GameManager.instance.playerData;

        if (currentScore > playerData.bestScore)
        {
            playerData.username = PhotonNetwork.LocalPlayer.NickName;
            playerData.bestScore = currentScore;
            playerData.bestScoreDate = DateTime.UtcNow.ToString();
            playerData.totalPlayersInGame = PhotonNetwork.CurrentRoom.PlayerCount;
            playerData.roomName = PhotonNetwork.CurrentRoom.Name;

            GameManager.instance.globalLeaderboard.SubmitScore(currentScore);
            GameManager.instance.SavePlayerData();
        }
    }
    public void LeaveGame()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(mainMenuScene); // Load the main menu or lobby
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("Multiplayer");
    }
    private void StartGameTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { TimerKey, PhotonNetwork.Time + gameDuration }
        };

            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            Debug.Log($"TimerKey set in room properties: {roomProperties[TimerKey]}");
        }
        else
        {
            Debug.Log("Not Master Client; skipping StartGameTimer.");
        }
    }
    private void UpdateGameTimer()
    {
        if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TimerKey, out object timerEndValue))
        {
            if (!isGameOver && PhotonNetwork.IsMasterClient)
            {
                Debug.LogWarning("TimerKey not yet available. Waiting for synchronization.");
            }
            return;
        }

        double timerEnd = (double)timerEndValue;
        currentTime = (float)(timerEnd - PhotonNetwork.Time);

        if (currentTime <= 0 && !isGameOver)
        {
            Debug.Log("Timer finished. Determining winner and triggering EndGame.");
            currentTime = 0;

            // Find the player with the highest kills directly in this method
            Photon.Realtime.Player winnerPlayer = null;
            int highestKills = -1;

            foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.TryGetValue("Kills", out object kills))
                {
                    int playerKills = (int)kills;
                    Debug.Log($"Player {player.NickName} has {playerKills} kills.");

                    if (playerKills > highestKills)
                    {
                        highestKills = playerKills;
                        winnerPlayer = player;
                    }
                }
            }
            EndGame(winnerPlayer);

        }

        if (timerText != null)
        {
            timerText.text = $"Time: {Mathf.Max(0, currentTime):F0} seconds";
        }
    }

    private void EndGame(Photon.Realtime.Player targetPlayer)
    {
        isGameOver = true;

        if (targetPlayer == null)
        {
            winnerText.text = "No winner could be determined.";
            gameOverPopUp.SetActive(true);
            return;
        }

        if (targetPlayer == PhotonNetwork.LocalPlayer)
        {
            winnerText.text = $"Congratulations, {targetPlayer.NickName}! You Win!";

            GameManager.instance.globalLeaderboard.SubmitScore(PhotonNetwork.LocalPlayer.GetScore());

            if (audioSource != null && winSound != null)
            {
                audioSource.PlayOneShot(winSound);
            }
        }
        else
        {
            winnerText.text = "You lost! Again.";
            if (audioSource != null && loseSound != null)
            {
                audioSource.PlayOneShot(loseSound);
            }
        }

        gameOverPopUp.SetActive(true);
    }



    public void CheckPlayAgainAvailability()
    {
        if (playAgainButton == null)
        {
            Debug.LogError("Play Again button is not assigned!");
            return;
        }

        if (PhotonNetwork.PlayerList.Length >= 2)
        {
            playAgainButton.gameObject.SetActive(true);
            playAgainButton.onClick.RemoveAllListeners();
            playAgainButton.onClick.AddListener(() => PlayAgain());
        }
        else
        {
            playAgainButton.gameObject.SetActive(false);
        }
    }

    private void PlayAgain()
    {
        PhotonView photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView is missing!");
            return;
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Only the Master Client can start a new game.");
            return;
        }

        if (PhotonNetwork.PlayerList.Length >= 2)
        {
            photonView.RPC("RestartGame", RpcTarget.All);
        }
        else
        {
            playAgainButton.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    private void RestartGame()
    {
        PhotonNetwork.LoadLevel("PlayerBattle");
    }

}

