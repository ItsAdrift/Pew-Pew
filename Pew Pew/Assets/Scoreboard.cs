using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    public TMP_Text username;
    public TMP_Text kills;
    public TMP_Text deaths;

    [HideInInspector] public string userID;
}
