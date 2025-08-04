using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UI;

public class Chat : MonoBehaviour, IChatClientListener
{
    public string userName;
    public ChatClient ChatClient;

    public InputField InputField;
    public Text chatContent;
    
    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("Chat - " + level + " - " + message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("Chat - OnChatStateChange" + state);
    }

    public void OnConnected()
    {
        Debug.Log("Chat - User: " + userName + " has Connected.");
        ChatClient.Subscribe(PhotonNetwork.CurrentRoom.Name, creationOptions: new ChannelCreationOptions() {PublishSubscribers = true});
    }

    public void OnDisconnected()
    {
        Debug.Log("Chat - User: " + userName + " has disconnected.");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        ChatChannel currentChat;
        if(ChatClient.TryGetChannel(PhotonNetwork.CurrentRoom.Name, out currentChat))
        {
            chatContent.text = currentChat.ToStringMessages();
        }
    }

    

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for(int i = 0 ; i < channels.Length; i++)
        {
            if (results[i])
            {
                Debug.Log("Chat - Subscribed to " + channels[i] + "channel.");
                ChatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, " has joined the chat.");
            }
        }
    }
    private void Start()
    {
        ChatClient = new ChatClient(this);
    }
    private void Update()
    {
        ChatClient.Service();
    }
    public void SetMessage()
    {
        if (InputField.text == "") { return; }
        ChatClient.PublishMessage(PhotonNetwork.CurrentRoom.Name, InputField.text);
        InputField.text = " ";
    }

    public void OnUnsubscribed(string[] channels){} // not needed.
    public void OnUserSubscribed(string channel, string user) { } // not needed.
    public void OnUserUnsubscribed(string channel, string user) { } // not needed.
    public void OnPrivateMessage(string sender, object message, string channelName) { }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { } // not needed.
    
}
