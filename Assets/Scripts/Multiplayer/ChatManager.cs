using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using TMPro;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    public static ChatManager instance;

    public GameObject panel;
    public TMPro.TMP_InputField inputField; 
    public TMPro.TextMeshProUGUI chatContent; 

    private bool visableChat = false; 
    private ChatClient chatClient; 
    private string currentRoomName; 
    public bool playerChatting = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new Photon.Chat.AuthenticationValues(PhotonNetwork.NickName));

        panel.SetActive(false);
    }

    void Update()
    {
        chatClient.Service();
       
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OpenChatPanel();
        }
    }

    private void OpenChatPanel()
    {
        if (panel == null || inputField == null)
        {
            return;
        }

        visableChat = !visableChat;
        panel.SetActive(visableChat);

        if (visableChat)
        {
            playerChatting = true; 
            inputField.Select();
            inputField.ActivateInputField();
        }
        else
        {
            playerChatting = false; 
            inputField.text = "";
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }


    public void SendMessage()
    {
        if (string.IsNullOrEmpty(inputField.text))
        {
            return;
        }

        chatClient.PublishMessage(currentRoomName, inputField.text);
        inputField.text = "";
    }

    public void OnConnected()
    {
        currentRoomName = PhotonNetwork.CurrentRoom.Name;

        chatClient.Subscribe(new string[] { currentRoomName });
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected from Photon Chat");
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("Chat state changed: " + state);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("Photon Chat Debug: " + message);
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName == currentRoomName)
        {
            for (int i = 0; i < senders.Length; i++)
            {
                chatContent.text += $"{senders[i]}: {messages[i]}\n";
            }
        }
    }
    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Subscribed to channel: " + channels[0]);
    }

    // not needed methods
    public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void OnUnsubscribed(string[] channels) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnUserSubscribed(string channel, string user) { }
    public void OnUserUnsubscribed(string channel, string user) { }
}
