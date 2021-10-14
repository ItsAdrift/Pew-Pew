using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Interactable
{
    public int health = 50;

    private PhotonView PV;
    private PlayerController playerController;

    void Awake()
    {
        playerController = (PlayerController) PV.InstantiationData[0];
    }

    public override string GetName()
    {
        return "Health Pack";
    }

    public override void Use()
    {
        if (playerController.currentHealth + health >= 100)
        {
            playerController.currentHealth = 100;
        } else
        {
            playerController.currentHealth = playerController.currentHealth + health;
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
