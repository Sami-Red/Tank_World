using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public string uid, username, bestScoreDate, roomName;
    public int bestScore, totalPlayersInGame;

    public PlayerData()
    {
        uid = Guid.NewGuid().ToString();
    }
}
