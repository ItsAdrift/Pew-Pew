using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreboardManager : MonoBehaviourPunCallbacks
{
    [HideInInspector] public static ScoreboardManager Instance;

    [SerializeField] GameObject scoreboardPrefab;
    [SerializeField] Transform holder;

    Dictionary<string, Scoreboard> scoreboardItems = new Dictionary<string, Scoreboard>();

    public void Awake()
    {
        Instance = this;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Scoreboard scoreboard = Instantiate(scoreboardPrefab, holder).GetComponent<Scoreboard>();
            scoreboard.username.text = player.NickName;
            scoreboard.kills.text = "0";
            scoreboard.deaths.text = "0";

            scoreboardItems.Add(player.NickName, scoreboard);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps) {
        Scoreboard scoreboard;
        scoreboardItems.TryGetValue(target.NickName, out scoreboard);

        scoreboard.username.text = target.NickName;
        if (changedProps.ContainsKey("kills"))
        {
            scoreboard.kills.text = "" + (int)changedProps["kills"];
        }
        if (changedProps.ContainsKey("deaths"))
        {
            scoreboard.deaths.text = "" + (int)changedProps["deaths"];
        }

        scoreboardItems.Remove(target.NickName);
        scoreboardItems.Add(target.NickName, scoreboard);

        OrderScoreboard();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Scoreboard scoreboard = Instantiate(scoreboardPrefab, holder).GetComponent<Scoreboard>();
        scoreboard.username.text = newPlayer.NickName;
        scoreboard.kills.text = "0";
        scoreboard.deaths.text = "0";

        scoreboardItems.Add(newPlayer.UserId, scoreboard);

        OrderScoreboard();
    }

    public void OrderScoreboard()
    {
        List<Scoreboard> orderedList = scoreboardItems.Values.ToList().OrderByDescending(o => int.Parse(o.kills.text)).ToList();
        
        for (int i = 0; i < orderedList.Count; i++)
        {
            orderedList[i].gameObject.transform.SetSiblingIndex(i);
        }
    }

}
