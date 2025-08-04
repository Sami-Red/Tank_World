using UnityEngine;
using Photon.Pun;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private float gameDuration = 300f; // Game duration in seconds
    [SerializeField] private TMPro.TextMeshProUGUI timerText;

    private float currentTime;
    private bool isTimerRunning = false;
    private const string TimerKey = "GameTimer";

    public delegate void TimerExpiredHandler();
    public event TimerExpiredHandler OnTimerExpired;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartGameTimer();
        }
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            UpdateGameTimer();
        }
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
        }

        isTimerRunning = true;
    }

    private void UpdateGameTimer()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(TimerKey, out object timerEndValue))
        {
            double timerEnd = (double)timerEndValue;
            currentTime = (float)(timerEnd - PhotonNetwork.Time);

            if (currentTime <= 0)
            {
                currentTime = 0;
                isTimerRunning = false;
                OnTimerExpired?.Invoke(); // Notify listeners that the timer has expired
            }

            // Update UI
            if (timerText != null)
            {
                timerText.text = $"Time: {Mathf.Max(0, currentTime):F0} seconds";
            }
        }
    }
}
