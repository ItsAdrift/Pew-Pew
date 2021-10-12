using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameQuitHandler : MonoBehaviour
{
   
    public void HandleQuitButtonClick()
    {
        Destroy(RoomManager.Instance.gameObject);
        StartCoroutine(DisconnectAndSendToTitle());
    }

    IEnumerator DisconnectAndSendToTitle()
    {
        PhotonNetwork.LeaveRoom();
        while (PhotonNetwork.InRoom)
            yield return null;
        SceneManager.LoadScene(0);
    }

}
