using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static float mouseSensitivity = 150f;
    public static float zoomSensitivity = 80f;

    public static float volume = 0.5f;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static void UpdateSettings(float _mouseSensitivity, float _zoomSensitivty, float _volume)
    {
        GameSettings.mouseSensitivity = _mouseSensitivity;
        GameSettings.zoomSensitivity = _zoomSensitivty;
        GameSettings.volume = _volume;
    }
}
