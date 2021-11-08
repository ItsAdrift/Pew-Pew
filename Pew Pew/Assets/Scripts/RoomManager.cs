using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.IO;
using Utilities;
using ExitGames.Client.Photon;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    private PhotonView PV;

    void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;

        PV = GetComponent<PhotonView>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
        GeneralEvents.GamemodeChange += (Gamemode newMode) => { OnGamemodeChange(newMode); };
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GeneralEvents.GamemodeChange -= OnGamemodeChange;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == LevelIndex.FFA_BASE || scene.buildIndex == LevelIndex.TDM_BASE)
        {
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("map", out object mapID);

            Map map = MapController.Instance.FindMap((string)mapID);

            GameObject mapObject = map.mapObject;
            Vector3 mapPosition = new Vector3(0f, map.mapHeight, 0f);
            Quaternion mapRotation = Quaternion.identity;

            Instantiate(mapObject, mapPosition, mapRotation);

            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        foreach (PhotonView pv in FindObjectsOfType<PhotonView>())
        {
            if (pv.Owner.UserId == otherPlayer.UserId)
            {
                Destroy(pv.transform.root.gameObject);
            }
        }
    }

    // 0 = Red, 1 = Blue
    public void ClickedJoinTeamButton(int team)
    {
        PhotonNetwork.LocalPlayer.SetTeam(team);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer)
        {
            if (changedProps.ContainsKey("teamIdx"))
            {
                PV.RPC("RPC_UpdatePlayerList", RpcTarget.Others);
                PlayerListManager.Instance.UpdatePlayerList();
            }
            
        }
    }

    [PunRPC]
    void RPC_UpdatePlayerList()
    {
        PlayerListManager.Instance.UpdatePlayerList();
    }

    public void OnGamemodeChange(Gamemode newMode)
    {
        PV.RPC("RPC_SwitchGamemode", RpcTarget.Others, newMode.gamemodeID);
    }

    [PunRPC]
    void RPC_SwitchGamemode(string gamemodeID)
    {
        PlayerListManager.Instance.OnGamemodeChange(GamemodeController.Instance.FindGamemode(gamemodeID));
    }
}
