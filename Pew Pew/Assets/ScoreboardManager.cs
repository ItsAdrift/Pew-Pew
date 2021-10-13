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
    }

    public void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Scoreboard scoreboard = Instantiate(scoreboardPrefab, holder).GetComponent<Scoreboard>();
            scoreboard.username.text = player.NickName;
            scoreboard.kills.text = "0";
            scoreboard.deaths.text = "0";

            scoreboard.userID = player.UserId;

            scoreboardItems.Add(player.UserId, scoreboard);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player target, Hashtable changedProps) {
        Scoreboard scoreboard;
        scoreboardItems.TryGetValue(target.UserId, out scoreboard);

        scoreboard.username.text = target.NickName;
        if (changedProps.ContainsKey("kills"))
        {
            scoreboard.kills.text = "" + (int)changedProps["kills"];
        }
        if (changedProps.ContainsKey("deaths"))
        {
            scoreboard.deaths.text = "" + (int)changedProps["deaths"];
        }

        OrderScoreboard();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Scoreboard scoreboard = Instantiate(scoreboardPrefab, holder).GetComponent<Scoreboard>();
        scoreboard.username.text = newPlayer.NickName;
        scoreboard.kills.text = "0";
        scoreboard.deaths.text = "0";

        scoreboard.userID = newPlayer.UserId;

        scoreboardItems.Add(newPlayer.UserId, scoreboard);

        OrderScoreboard();
    }

    public void OrderScoreboard()
    {
        List<Scoreboard> orderedList = scoreboardItems.Values.ToList().OrderByDescending(o => o.kills).ToList();
        foreach (Scoreboard sb in scoreboardItems.Values.ToList())
        {
            Destroy(sb.gameObject);
        }
        scoreboardItems.Clear();
        foreach (Scoreboard sb in orderedList)
        {
            Scoreboard scoreboard = Instantiate(scoreboardPrefab, holder).GetComponent<Scoreboard>();
            scoreboard.username.text = sb.username.text;
            scoreboard.kills.text = sb.kills.text;
            scoreboard.deaths.text = sb.deaths.text;

            scoreboardItems.Add(scoreboard.userID, scoreboard);
        }
    }

}
