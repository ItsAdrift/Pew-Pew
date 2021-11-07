using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GamemodeController : MonoBehaviourPunCallbacks
{
    public Gamemode[] modes;

    [SerializeField] Slider slider;
    [SerializeField] TMP_Text gamemodeName;
    [SerializeField] TMP_Text gamemodeDescription;

    private Gamemode selectedGamemode;

    [HideInInspector] public static GamemodeController Instance;

    public void Start()
    {
        Instance = this;

        slider.maxValue = modes.Length - 1;
        slider.value = 0;

        SelectGameMode(modes[0]);
    }

    public void SelectGameMode(Gamemode mode)
    {
        UpdateGamemodeDisplay(mode);

        if (PhotonNetwork.CurrentRoom != null)
        {
            Hashtable hash = new Hashtable();
            hash.Add("gamemode", mode.gamemodeID);

            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }

        GeneralEvents.GamemodeChange(mode);
    }

    public void UpdateGamemodeDisplay(Gamemode mode)
    {
        selectedGamemode = mode;
        gamemodeName.text = mode.gamemodeName;
        gamemodeDescription.text = mode.gamemodeDescription;
    }

    public override void OnRoomPropertiesUpdate(Hashtable pr)
    {
        if (pr.ContainsKey("gamemode"))
        {
            Gamemode m = FindGamemode((string)pr["gamemode"]);
            UpdateGamemodeDisplay(m);
        }
    }

    /*
     * Gamemode changes will need to sync across clients
     */
    public void OnSliderUpdate()
    {
        SelectGameMode(modes[(int)slider.value]);
    }

    public Gamemode GetSelectedGamemode()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("gamemode"))
        {
            return FindGamemode((string)PhotonNetwork.CurrentRoom.CustomProperties["gamemode"]);
        }
        else
        {
            return modes[0];
        }
    }

    public Gamemode FindGamemode(string id)
    {
        foreach (Gamemode m in modes)
        {
            if (m.gamemodeID.Equals(id))
            {
                return m;
            }
        }

        return modes[0];
    }

}
