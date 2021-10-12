using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsLink : MonoBehaviour
{
    bool updatingSliders = false;

    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] Slider volumeSlider;

    public void OnSliderUpdate()
    {
        if (updatingSliders)
            return;

        GameSettings.UpdateSettings(mouseSensitivitySlider.value, volumeSlider.value);
    }

    public void UpdateSliders()
    {
        updatingSliders = true;
        mouseSensitivitySlider.value = GameSettings.mouseSensitivity;
        volumeSlider.value = GameSettings.volume;
        updatingSliders = false;
    }

}
