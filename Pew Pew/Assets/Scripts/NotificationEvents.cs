using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;

public class NotificationEvents : MonoBehaviour
{
    public static Action<string, RpcTarget> SendNotificiation = delegate {};
}
