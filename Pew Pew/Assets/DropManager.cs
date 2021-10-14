using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;

public class DropManager : MonoBehaviour
{
    public static DropManager Instance;

    public PlayerController playerController;

    void Awake()
    {
        Instance = this;
    }

    public void DropHealthpack(Vector3 position, Quaternion rotation)
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "HealthPack"), position, rotation, 0, new object[] { playerController });
    }
}
