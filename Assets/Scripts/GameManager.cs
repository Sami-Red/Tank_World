using UnityEngine;
using System.IO;
using Leguar.TotalJSON;
using PlayFab;
using PlayFab.ClientModels;
using System.Text;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerData playerData;
    public string filePath;
    public GlobalLeaderboard globalLeaderboard;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        LoadPlayerData();
        LoginToPlayFab();

    }
    public void SavePlayerData()
    {
        string serialisedDataString = JSON.Serialize(playerData).CreateString();

        string b64String = eBase64(serialisedDataString);
        File.WriteAllText(filePath, b64String);
        
        
    }

    public void LoadPlayerData()
    {
        if (!File.Exists(filePath))
        {
            playerData = new PlayerData();
            SavePlayerData();
            return;
        }

        string fContents = File.ReadAllText(filePath);
        string jString = Encoding.UTF8.GetString(Convert.FromBase64String(fContents));
        playerData = JSON.ParseString(jString).Deserialize<PlayerData>();
    }

    void LoginToPlayFab()
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
            CustomId = playerData.uid
        };
        PlayFabClientAPI.LoginWithCustomID(request, PlayFabLoginResult, PlayFabLoginError);
    }
    void PlayFabLoginResult(LoginResult loginResult)
    {
        Debug.Log("PlayFab - Login Succeeded: " + loginResult.ToJson());
    }
    void PlayFabLoginError(PlayFabError loginError)
    {
        Debug.Log("PlayFab - Login Failed: " + loginError.ErrorMessage);
    }

    // json b64 section
    public string eBase64(string jString)
    {
        byte[] jByte = Encoding.UTF8.GetBytes(jString);
        return Convert.ToBase64String(jByte);
    }
    public string dBase64(string b64String)
    {
        byte[] jByte = Convert.FromBase64String(b64String);
        return Convert.ToBase64String(jByte);
    }
}
