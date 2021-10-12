using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

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
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { PV.ViewID});
    }

    public void Die(string damager)
    {
        PhotonNetwork.Destroy(controller);
        CreateController();

        // Display the death message to the new controller & set their settings again
        PlayerController newController = controller.GetComponent<PlayerController>();
        newController.ShowDeathMessage(damager);
        newController.GetComponent<GameSettingsLink>()?.UpdateSliders();
    }
}
