using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Utilities;
using TMPro;
using ExitGames.Client.Photon;

public class ScoreboardManager : MonoBehaviourPunCallbacks
{
    [HideInInspector] public static ScoreboardManager Instance;

    public bool teamHasWon = false;

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
    [SerializeField] private TMP_Text redScore;
    [SerializeField] private TMP_Text blueScore;

    [Header("Team Death Match Win")]
    [SerializeField] private GameObject winUI;
    [SerializeField] private TMP_Text winnerDisplay;
    [SerializeField] private TMP_Text redTeamScore;
    [SerializeField] private TMP_Text blueTeamScore;
    [SerializeField] private Color redColor;
    [SerializeField] private Color blueColor;
    

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

    public override void OnRoomPropertiesUpdate(Hashtable pr)
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            int pointsToWin = PhotonNetwork.CurrentRoom.GetTDMPointsToWin();

            int redPoints = PhotonNetwork.CurrentRoom.GetTDMRedPoints();
            int bluePoints = PhotonNetwork.CurrentRoom.GetTDMBluePoints();
            redScore.text = "" + redPoints + "/" + pointsToWin;
            blueScore.text = "" + bluePoints + "/" + pointsToWin;

            // Check for a win
            if (redPoints >= pointsToWin)
            {
                Win(0);
            }
            else if (bluePoints >= pointsToWin)
            {
                Win(1);
            }
        }
        
    }

    public void Win(int team)
    {
        teamHasWon = true;
        winUI.SetActive(true);
        if (team == 0) // Red
        {
            winnerDisplay.text = "Red Team Won!";
            winnerDisplay.color = redColor;
        } else if (team == 1) // Red
        {
            winnerDisplay.text = "Blue Team Won!";
            winnerDisplay.color = blueColor;
        }

        redTeamScore.text = "Red Team Score: " + redScore.text;
        blueTeamScore.text = "Blue Team Score: " + blueScore.text;

        Cursor.lockState = CursorLockMode.None;
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
