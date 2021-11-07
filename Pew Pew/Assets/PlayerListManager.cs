using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class PlayerListManager : MonoBehaviour
{
    [HideInInspector] public static PlayerListManager Instance;

    [Header("Sections")]
    [SerializeField] GameObject freeForAll;
    [SerializeField] GameObject teamDeathMatch;

    [Header("Team Death Match")]
    [SerializeField] Transform teamDeathMatchRed;
    [SerializeField] Transform teamDeathMatchBlue;

    [Header("Prefabs")]
    [SerializeField] GameObject playerListItemPrefab;

    void Start()
    {
        Instance = this;

        GeneralEvents.GamemodeChange += (Gamemode newMode) => { OnGamemodeChange(newMode); };
    }

    void OnDestroy()
    {
        GeneralEvents.GamemodeChange -= OnGamemodeChange;
    }

    public void UpdatePlayerList()
    {
        Gamemode gamemode = GamemodeController.Instance.GetSelectedGamemode();

        if (gamemode.gamemodeID.Equals("ffa"))
        {
            foreach (Transform child in freeForAll.transform)
            {
                Destroy(child.gameObject);
            }

            // Display all of the players already in the lobby to this client. New players that join are handled in #OnPlayerEnteredRoom
            Player[] players = PhotonNetwork.PlayerList;
            for (int i = 0; i < players.Length; i++)
            {
                Instantiate(playerListItemPrefab, freeForAll.transform).GetComponent<PlayerListItem>().Setup(players[i]);
            }
        } else
        {
            foreach (Transform child in teamDeathMatchRed)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in teamDeathMatchBlue)
            {
                Destroy(child.gameObject);
            }

            // Display all of the players already in the lobby to this client. New players that join are handled in #OnPlayerEnteredRoom
            Player[] players = PhotonNetwork.PlayerList;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].GetTeam() == 0)
                {
                    // Red
                    Instantiate(playerListItemPrefab, teamDeathMatchRed.transform).GetComponent<PlayerListItem>().Setup(players[i]);
                }

                if (players[i].GetTeam() == 1)
                {
                    // Blue
                    Instantiate(playerListItemPrefab, teamDeathMatchBlue.transform).GetComponent<PlayerListItem>().Setup(players[i]);
                }

            }
        }
    }

    // TO DO: This needs to become an RPC so that the switch will happen across all clients.
    // OTHER TO DO: Why the heck do you have to click a team twice to join it? weird.
    void OnGamemodeChange(Gamemode mode)
    {
        if (mode.gamemodeID.Equals("ffa"))
        {
            freeForAll.SetActive(true);

            teamDeathMatch.SetActive(false);
        } else if (mode.gamemodeID.Equals("tdm"))
        {
            teamDeathMatch.SetActive(true);
            freeForAll.SetActive(false);
        }

        UpdatePlayerList();
    }
}
