using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Utilities;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text roomNameText;

    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;

    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;

    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject masterOnly;

    void Awake()
    {
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    /*void OnLevelWasLoaded(int level)
    {
        if (level == 1)
        {
            MenuManager.instance.CloseAll();
        }
    }*/

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        MenuManager.instance.OpenMenu("title");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameInputField.text))
        {
            return;
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.BroadcastPropsChangeToAll = true;
        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
        MenuManager.instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.instance.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        PlayerListManager.Instance.UpdatePlayerList();

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        masterOnly.SetActive(PhotonNetwork.IsMasterClient);

        MapController.Instance.UpdateMapDisplay(MapController.Instance.GetSelectedMap());
        GamemodeController.Instance.UpdateGamemodeDisplay(GamemodeController.Instance.GetSelectedGamemode());

        if ((string)PhotonNetwork.CurrentRoom.CustomProperties["gamemode"] == "tdm")
        {
            PlayerListManager.Instance.ChangeGamemodeScreen(GamemodeController.Instance.FindGamemode("tdm"));
            RoomManager.Instance.ClickedJoinTeamButton(Random.Range(0, 1));
        }


    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
         startGameButton.SetActive(PhotonNetwork.IsMasterClient);
         masterOnly.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MenuManager.instance.ThrowError("error", "Room Creation Failed: " + message);
    }

    /*
     * These methods are called when a player clicks a button one of our menus.
     */
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(GamemodeController.Instance.GetSelectedGamemode().gamemodeBaseSceneIndex);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("title");
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform t in roomListContent)
        {
            Destroy(t.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().Setup(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().Setup(newPlayer);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
