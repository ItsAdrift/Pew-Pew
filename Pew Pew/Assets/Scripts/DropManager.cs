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

    public List<Interactable> droppedItems = new List<Interactable>();

    public bool lookingAtDrop = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        droppedItems.RemoveAll(item => item == null);

        if (!lookingAtDrop)
        {
            foreach (Interactable i in droppedItems)
            {
                i.text.gameObject.SetActive(false);
            }
        }
        
    }

    public void DropHealthpack(Vector3 position, Quaternion rotation)
    {
        droppedItems.Add(PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "HealthPack"), position, rotation).GetComponent<Interactable>());
    }
}
