using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Notification : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    public void Setup(string _text)
    {
        text.text = _text;
    }
}
