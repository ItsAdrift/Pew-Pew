using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Utilities;

public class ModifierManager : MonoBehaviour
{

    [SerializeField] Toggle oneShot;
    [SerializeField] TMP_InputField tdm_pointsToWin;
    [SerializeField] TMP_InputField tdm_pointsKill;
    [SerializeField] TMP_InputField tdm_pointsHeadshot;

    public void Save()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            Room room = PhotonNetwork.CurrentRoom;

            room.SetOneShot(oneShot.isOn);
            room.SetTDMPointsToWin(int.Parse(tdm_pointsToWin.text));
            room.SetTDMPointsKill(int.Parse(tdm_pointsKill.text));
            room.SetTDMPointsHeadshot(int.Parse(tdm_pointsHeadshot.text));
        }
    }
}
