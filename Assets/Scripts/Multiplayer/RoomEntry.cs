using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomEntry : MonoBehaviour
{
    public Text roomText;
    public string roomName;

    public void JoinRoom()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomName);
    }
}
