using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Utilities;

public class PlayerManager : MonoBehaviour
{
    private int kills = 0;
    private int deaths = 0;

    PhotonView PV;

    GameObject controller;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        // Create our player controller
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { PV.ViewID });

        /*Hashtable hash = new Hashtable();
        hash.Add("deaths", 0);
        hash.Add("kills", 0);

        PV.Owner.SetCustomProperties(hash);*/
    }

    public void Die(string damager)
    {
        ScoreboardManager scoreboard = ScoreboardManager.Instance;

        PhotonNetwork.Destroy(controller);
        CreateController();

        

        // Display the death message to the new controller & set their settings again
        PlayerController newController = controller.GetComponent<PlayerController>();
        //newController.ShowDeathMessage(damager.Replace("void_", "")); Removed this due to suggestions, death messages will now just show up in the top right hand corner
        newController.GetComponent<GameSettingsLink>()?.UpdateSliders();
        if (damager.StartsWith("void_"))
        {
            NotificationManager.Instance.SendPlayerDiedMessage("<KILLED> was killed by <KILLER>", PV.Owner.NickName, damager.Replace("void_", ""));
        } else if (damager.Equals("The Void"))
        {
            NotificationManager.Instance.SendPlayerDiedMessage("<KILLED> fell into the void", PV.Owner.NickName, damager);
        } else
        {
            NotificationManager.Instance.SendPlayerDiedMessage("<KILLED> was killed by <KILLER>", PV.Owner.NickName, damager);
        }

        PhotonNetwork.LocalPlayer.AddDeaths(1);
    }

    public void AddKill()
    {
        PhotonNetwork.LocalPlayer.AddKills(1);
    }
}
