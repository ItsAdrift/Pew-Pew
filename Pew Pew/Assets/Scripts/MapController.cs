using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MapController : MonoBehaviourPunCallbacks
{
    public Map[] maps;

    [SerializeField] Slider slider;
    [SerializeField] TMP_Text mapName;
    [SerializeField] Image mapImage;

    private Map selectedMap;

    [HideInInspector] public static MapController Instance;

    public void Start()
    {
        Instance = this;

        slider.maxValue = maps.Length - 1;
        slider.value = 0;

        SelectMap(maps[0]);
    }

    public void SelectMap(Map map)
    {
        UpdateMapDisplay(map);

        if (PhotonNetwork.CurrentRoom != null)
        {
            Hashtable hash = new Hashtable();
            hash.Add("map", map.mapID);

            PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
        }

        GeneralEvents.MapChange(map);
    }

    public void UpdateMapDisplay(Map map)
    {
        selectedMap = map;
        mapName.text = map.mapName;
        mapImage.sprite = map.mapImage;
    }

    public override void OnRoomPropertiesUpdate(Hashtable pr)
    {
        if (pr.ContainsKey("map"))
        {
            Map m = FindMap((string) pr["map"]);
            UpdateMapDisplay(m);
        }
    }

    /*
     * Map changes will need to sync across clients
     */
    public void OnSliderUpdate()
    {
        SelectMap(maps[(int) slider.value]);
    }

    public Map GetSelectedMap()
    {
        return FindMap((string)PhotonNetwork.CurrentRoom.CustomProperties["map"]);
    }

    public Map FindMap(string id)
    {
        foreach(Map m in maps)
        {
            if (m.mapID.Equals(id))
            {
                return m;
            }
        }

        return maps[0];
    }

}
