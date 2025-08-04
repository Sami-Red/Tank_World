using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;

public class Multiplayer : MonoBehaviourPunCallbacks
{
    // -=[Panels]=-
    public Transform LoginPanel;
    public Transform SelectionPanel;
    public Transform CreateRoomPanel;
    public Transform InsideRoomPanel;
    public Transform ListRoomPanel;
    public Transform chatPanel;

    public InputField roomNameInput;
    public InputField playerNameInput;

    string playerName;
    
    public GameObject textPrefab;
    public GameObject startGameButton;

    public Transform insideRoomPlayerList;
    public Transform listRoomPanel;
    public GameObject roomEntryPrefab;
    public Transform listRoomPanelContent;

    Dictionary<string, RoomInfo> cachedRoomList;

    public Chat chat;

    private void Start()
    {
        playerNameInput.text = playerName = string.Format("Player {0}", Random.Range(1, 1000000));
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Update()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            startGameButton.SetActive(PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2);
        } else{
            startGameButton.SetActive(false);
        }
    }

    public void LoginButtonClicked()
    {
        if (playerNameInput.text.Trim() != "")
        {
            PhotonNetwork.LocalPlayer.NickName = playerName = playerNameInput.text;
            PhotonNetwork.ConnectUsingSettings();
            UpdatePlayfabUsername(playerName);
        }
        else
        {
            Debug.Log("name is invalid.");
        }
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("We have been connect to master server");
        ActivePanel("Selection");
    }
    public void ActivePanel(string panelName)
    {
        LoginPanel.gameObject.SetActive(false);
        SelectionPanel.gameObject.SetActive(false);
        CreateRoomPanel.gameObject.SetActive(false);
        InsideRoomPanel.gameObject.SetActive(false);
        ListRoomPanel.gameObject.SetActive(false);
        chatPanel.gameObject.SetActive(false);

        if(panelName == LoginPanel.gameObject.name)
        {
            LoginPanel.gameObject.SetActive(true);
        }
        else if (panelName == SelectionPanel.gameObject.name)
        {
            SelectionPanel.gameObject.SetActive(true);
        }
        else if (panelName == CreateRoomPanel.gameObject.name)
        {
            CreateRoomPanel.gameObject.SetActive(true);
        }
        else if (panelName == InsideRoomPanel.gameObject.name)
        {
            InsideRoomPanel.gameObject.SetActive(true);
        }
        else if (panelName == ListRoomPanel.gameObject.name)
        {
            ListRoomPanel.gameObject.SetActive(true);
        }
        else if(panelName == chatPanel.gameObject.name)
        {
             chatPanel.gameObject.SetActive(true);
        }
    }
    void UpdatePlayfabUsername(string name)
    {
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, PlayFabUpdateUserTitleDisplayNameResult, PlayFabUpdateUserTitleDisplayNameError);
    }

    void PlayFabUpdateUserTitleDisplayNameResult(UpdateUserTitleDisplayNameResult updateUserTitleDisplayNameRequest)
    {
        Debug.Log("Playfab - UserTitleDisplay Updated");
    }

    void PlayFabUpdateUserTitleDisplayNameError(PlayFabError updateUserTitleDisplayNameError)
    {
        Debug.Log("Playfab - error occured while updating usertitledisplayvalue: " + updateUserTitleDisplayNameError.ErrorMessage);
    }
    public void DisconnectButtonClicked()
    {
        PhotonNetwork.Disconnect();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from master server");
        cachedRoomList = new Dictionary<string, RoomInfo>();
        ActivePanel("Login");
    }
    public void CreateARoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;

        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("Room has been created!");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create Room!");
    }
    public override void OnJoinedRoom()
    {
        var authenticationValues = new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);
        chat.userName = PhotonNetwork.LocalPlayer.NickName;
        chat.ChatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", authenticationValues);
  
        Debug.Log("You have joined Room");
        ActivePanel("InsideRoom");
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

        ActivePanel("InsideRoom");

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
            playerListEntry.GetComponent<Text>().text = player.NickName;
            playerListEntry.name = player.NickName;
        }
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MainMenu");
    }
    public override void OnLeftRoom()
    {
        chat.ChatClient.Disconnect();

        Debug.Log("Room has been created");
        ActivePanel("CreateRoom");
        Order66(insideRoomPlayerList);
    }

    public void Order66(Transform parent) //Destroys name object when a player leaves lobby
    {
        foreach(Transform child in parent)
        {
            Destroy(child.gameObject);
        }

    }
    public void ListRoomsClicked()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!");
        ActivePanel("ListRooms");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room Update: " + roomList.Count);

        foreach(var room in roomList)
        {
            var newRoomEntry = Instantiate(roomEntryPrefab, listRoomPanelContent);
            var newRoomEntryScript = newRoomEntry.GetComponent<RoomEntry>();
            newRoomEntryScript.roomName = room.Name;
            newRoomEntryScript.roomText.text = string.Format("[{0} - ({1}/{2})]", room.Name, room.PlayerCount, room.MaxPlayers);
        }
    }
    public void LeaveLobbyClicked()
    {
        PhotonNetwork.LeaveLobby();
    }   
    public override void OnLeftLobby()
    {
        Debug.Log("left Lobby");
        ActivePanel("Selection");
        Order66(insideRoomPlayerList);
        Order66(listRoomPanelContent);

    }
    public void OnJoinRandomRoomClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Player Joined Room");

        foreach(var player in PhotonNetwork.PlayerList)
        {
            var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
            playerListEntry.GetComponent<Text>().text = newPlayer.NickName;
            playerListEntry.name = player.NickName;
        }
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Player Left Room");
        
        foreach(Transform child in insideRoomPlayerList)
        {
            Destroy(child.gameObject);
            break;
        }
    }
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    public void StartGameClicked()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount < 2)
        {
            // Debug.Log("cannot play by urself"); // nvm made it so the button comes after 2 or more join
            return;
        }
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("PlayerBattle");
    }
      
}
