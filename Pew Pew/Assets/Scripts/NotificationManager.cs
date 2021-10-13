using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    void Awake()
    {
        Instance = this;
    }

    [SerializeField] PhotonView PV;

    [SerializeField] Transform notificationManager;
    [SerializeField] GameObject notificationPrefab;

    public void SendNotification(string message)
    {
        GameObject obj = Instantiate(notificationPrefab, notificationManager);
        //Debug.Log(obj.transform.position);
        //Debug.Log(obj.transform.parent.name); // The object is not spawning as a child of the notificationManager (Vertical Layout Group) object
        obj.GetComponent<Notification>().Setup(message);
        Destroy(obj, 8f);
    }

    public void SendGlobalNotification(string message, bool toSelf)
    {
        RpcTarget target = RpcTarget.All;
        if (!toSelf)
        {
           target = RpcTarget.Others;
        }
        
        NotificationEvents.SendNotificiation(message, target);
    }

    public void SendDelayedGlobalNotification(string message, bool toSelf, float delay)
    {
        StartCoroutine(_SendDelayedGlobalNotification(message, toSelf, delay));
    }

    private IEnumerator _SendDelayedGlobalNotification(string message, bool toSelf, float delay)
    {
        yield return new WaitForSeconds(delay);

        SendGlobalNotification(message, toSelf);
    }

    public void SendPlayerDiedMessage(string message, string killed, string killer)
    {
        NotificationEvents.PlayerDied(message, killed, killer);
    }

}
