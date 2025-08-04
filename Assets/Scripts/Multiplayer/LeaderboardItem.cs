using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardItem : MonoBehaviour
{
    public Text order, username, score;

    public void SetScores(int _order, string _username, int _score)
    {
        order.text = _order.ToString() + ")";
        username.text = _username;
        score.text = _score.ToString();
    }
}
