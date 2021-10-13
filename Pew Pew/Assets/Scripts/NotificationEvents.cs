using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class NotificationEvents : MonoBehaviour
{
    public static Action<string, RpcTarget> SendNotificiation = delegate {};
    // message (using placeholders <KILLED> <KILLER>) killed, killer
    public static Action<string, string, string> PlayerDied = delegate {};
}
