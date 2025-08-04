using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

public class GlobalLeaderboard : MonoBehaviour
{
    int maxResults = 5;
    public LeaderboardPopUp mostKillsPopUp;

    public void SubmitScore(int playerScore)
    {
        UpdatePlayerStatisticsRequest request = new UpdatePlayerStatisticsRequest()
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "Most Kills",
                    Value = playerScore,
                },
                new StatisticUpdate
                {
                    StatisticName = "Most Wins",
                    Value = 1,
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, PlayFabUpdateStatsResult, PlayFabUpdateStatsError);
    }

    void PlayFabUpdateStatsResult(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfully submitted score."); // Simplified message
    }

    void PlayFabUpdateStatsError(PlayFabError error)
    {
        Debug.LogError("Error submitting score: " + error.GenerateErrorReport()); // Detailed error log
    }

    public void GetLeaderboard()
    {
        GetLeaderboardRequest request = new GetLeaderboardRequest()
        {
            StatisticName = "Most Kills",

            MaxResultsCount = maxResults
        };
        
        PlayFabClientAPI.GetLeaderboard(
            request, PlayFabGetLeaderboardResult, PlayFabGetLeaderboardError);
    }

    void PlayFabGetLeaderboardResult(GetLeaderboardResult result)
    {
        Debug.Log($"Retrieved {result.Leaderboard.Count} leaderboard entries.");

        mostKillsPopUp.UpdateUI(result.Leaderboard);
        Debug.Log($"Successfully retrieved leaderboard with {result.Leaderboard.Count} entries.");
    }

    void PlayFabGetLeaderboardError(PlayFabError error)
    {
        Debug.LogError("Error retrieving leaderboard: " + error.GenerateErrorReport());
    }
}
