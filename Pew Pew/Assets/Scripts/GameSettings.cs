using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public static float mouseSensitivity = 150f;

    public static float volume = 0.5f;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static void UpdateSettings(float mouseSensitivity, float volume)
    {
        GameSettings.mouseSensitivity = mouseSensitivity;
        GameSettings.volume = volume;
    }
}
