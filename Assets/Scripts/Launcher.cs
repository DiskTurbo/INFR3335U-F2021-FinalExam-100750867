using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using UnityEngine.Audio;
using JSAM;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    public static int mapSelectInput;

    [SerializeField] AudioMixerGroup AAAAAAAAAAAAA;

    void Awake()
    {
        Instance = this;
    }

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField userNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject roomManager;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        if(GameObject.Find("RoomManager") == null)
        {
            Instantiate(roomManager);
        }
        Debug.Log("Connected to master.");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        PhotonNetwork.NickName = ("Guest #" + (Random.Range(0, 9999).ToString("0000")));
    }
    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined lobby.");
    }
    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        Debug.Log("Creating room...");
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        if (!string.IsNullOrEmpty(userNameInputField.text))
        {
            PhotonNetwork.NickName = userNameInputField.text;
        }
        MenuManager.Instance.OpenMenu("room");
        Debug.Log("Joined room (" + PhotonNetwork.CurrentRoom.Name + ")");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed! (" + message + ")";
        Debug.Log("Error");
        MenuManager.Instance.OpenMenu("error");
    }

    public void StartGame()
    {
        JSAM.AudioManager.PlaySound(Sounds.gamestart);
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        Debug.Log("Joining room...");
        MenuManager.Instance.OpenMenu("loading");

    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room.");
        MenuManager.Instance.OpenMenu("title");
    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
       foreach(Transform trans in roomListContent)
       {
            Destroy(trans.gameObject);
       }
       for(int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                continue;
            }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
        JSAM.AudioManager.PlaySound(Sounds.playerjoin);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        JSAM.AudioManager.PlaySound(Sounds.playerleave);
    }


}
