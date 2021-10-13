using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
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

        Hashtable hash = new Hashtable();
        hash.Add("deaths", 0);
        hash.Add("kills", 0);

        PV.Owner.SetCustomProperties(hash);
    }

    public void Die(string damager)
    {
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

        int deathScore = (int)PhotonNetwork.LocalPlayer.CustomProperties["deaths"];
        deathScore++;
        Hashtable hash = new Hashtable();
        hash.Add("deaths", deathScore);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

    }

    public void AddKill()
    {
        int killScore = (int)PhotonNetwork.LocalPlayer.CustomProperties["kills"];
        killScore++;
        Hashtable hash = new Hashtable();
        hash.Add("kills", killScore);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
}
