using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Interactable
{
    public int health = 50;

    [SerializeField] PhotonView PV;

    void Awake()
    {
        text.gameObject.SetActive(false);

        Physics.IgnoreLayerCollision(0, 9);
    }

    public override string GetName()
    {
        return "Health Pack";
    }

    public override void Use(PlayerController player)
    {
        if (player.currentHealth + health >= 100)
        {
            player.SetHealth(100);
        } else
        {
            player.SetHealth(player.currentHealth + health);
        }
        DropManager.Instance.droppedItems.Remove(this);
        PV.RPC("RPC_RemoveHealthPack", RpcTarget.All);
    }

    [PunRPC]
    void RPC_RemoveHealthPack()
    {
        Destroy(this.gameObject);
    }



}
