using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Utilities;

public class ScoreboardEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text username;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private TMP_Text deathsText;

    public Player Player;

    public int Kills => Player.GetKills();
    public int Deaths => Player.GetDeaths();

    public void Set(Player _player)
    {
        Player = _player;
        UpdateScore();

    }

    public void UpdateScore()
    {
        username.text = Player.NickName;
        killsText.text = "" + Player.GetKills();
        deathsText.text = "" + Player.GetDeaths();
    }
}
