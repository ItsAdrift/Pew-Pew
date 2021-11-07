using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEvents : MonoBehaviour
{
    public static Action<Map> MapChange = delegate { };
    public static Action<Gamemode> GamemodeChange = delegate { };
}
