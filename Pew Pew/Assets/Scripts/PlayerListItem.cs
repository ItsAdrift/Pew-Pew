using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerListItem : MonoBehaviourPunCallbacks
{

    [SerializeField] TMP_Text text;

    Player player;

    public void Setup(Player _player)
    {
        player = _player;
        text.text = player.NickName;
    }

    // This callback is sent to every client when a player leaves
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }

}
