using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PersonalBestPopup : MonoBehaviour
{
    public GameObject scoreHolder, noScoreText;
    public Text userName, bestScore, date, totalPlayers, roomName;
    public void UpdatePeronalBestUI()
    {
        PlayerData playerData = GameManager.instance.playerData;
        if (playerData.username != null)
            {
            userName.text = playerData.username;
            bestScore.text = playerData.bestScore.ToString();
            date.text = playerData.bestScoreDate;
            totalPlayers.text = playerData.totalPlayersInGame.ToString();
            roomName.text = playerData.roomName;

            scoreHolder.SetActive(true);
            noScoreText.SetActive(false);
        }
        else
        {
            scoreHolder.SetActive(false);
            noScoreText.SetActive(true);
        }
        
    }
    void OnEnable()
    {
        UpdatePeronalBestUI();
    }
}
