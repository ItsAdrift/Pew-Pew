using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Utilities;

public class ScoreboardManager : MonoBehaviourPunCallbacks
{
    [HideInInspector] public static ScoreboardManager Instance;

    void Start()
    {
        Instance = this;
    }

    private List<ScoreboardEntry> entries = new List<ScoreboardEntry>();

    [SerializeField] private ScoreboardEntry entryPrefab = null;
    [SerializeField] private Transform holder;
    [SerializeField] private GameObject displayPanel;

    [Header("Team Death Match")]
    [SerializeField] private ScoreboardEntry tdmEntryPrefab = null;
    [SerializeField] private Transform redHolder;
    [SerializeField] private Transform blueHolder;

    //creates and entry for local player and udpates the board
    public override void OnJoinedRoom()
    {
        CreateNewEntry(PhotonNetwork.LocalPlayer);
        UpdateScoreboard();
    }

    //creates entry foreach new player and updates the board
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        CreateNewEntry(newPlayer);
        UpdateScoreboard();
    }

    //removes entry from player that left the room and updates the board
    public override void OnPlayerLeftRoom(Player targetPlayer)
    {
        RemoveEntry(targetPlayer);

        UpdateScoreboard();
    }

    //using this callback to update the scoreboard only if the score property changed
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey(PlayerProperties.Kills) || changedProps.ContainsKey(PlayerProperties.Deaths))
        {
            UpdateScoreboard();
        }
    }

    private ScoreboardEntry CreateNewEntry(Player newPlayer)
    {
        if ((string)PhotonNetwork.CurrentRoom.CustomProperties["gamemode"] == "tdm")
        {
            var newEntry = Instantiate(tdmEntryPrefab, (newPlayer.GetTeam() == 0 ? redHolder : blueHolder), false);
            newEntry.Set(newPlayer);
            entries.Add(newEntry);
            return newEntry;
        }
        else
        {
            var newEntry = Instantiate(entryPrefab, holder, false);
            newEntry.Set(newPlayer);
            entries.Add(newEntry);
            return newEntry;
        }
        
    }

    private void UpdateScoreboard()
    {
        //iterate through all player to update score
        //if no entry exists create one
        foreach (var targetPlayer in PhotonNetwork.CurrentRoom.Players.Values)
        {
            var targetEntry = entries.Find(x => x.Player == targetPlayer);

            if (targetEntry == null)
            {
                targetEntry = CreateNewEntry(targetPlayer);
            }

            targetEntry.UpdateScore();
        }

        SortEntries();
    }

    private void SortEntries()
    {
        //sort entries in list
        entries.Sort((a, b) => b.Kills.CompareTo(a.Kills));

        //sort child order
        for (var i = 0; i < entries.Count; i++)
        {
            entries[i].transform.SetSiblingIndex(i);
        }
    }

    private void RemoveEntry(Player targetPlayer)
    {
        var targetEntry = entries.Find(x => x.Player == targetPlayer);
        entries.Remove(targetEntry);
        Destroy(targetEntry.gameObject);
    }

    public void SetOpen(bool open)
    {
        displayPanel.SetActive(open);
    }
    
    public bool IsOpen()
    {
        return displayPanel.activeSelf;
    }

}
