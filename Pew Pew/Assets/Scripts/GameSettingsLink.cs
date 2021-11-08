using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettingsLink : MonoBehaviour
{
    bool updatingSliders = false;

    [SerializeField] Slider mouseSensitivitySlider;
    [SerializeField] Slider zoomSensitivitySlider;
    [SerializeField] Slider volumeSlider;

    public void OnSliderUpdate()
    {
        if (updatingSliders)
            return;

        GameSettings.UpdateSettings(mouseSensitivitySlider.value, zoomSensitivitySlider.value, volumeSlider.value);
    }

    public void UpdateSliders()
    {
        updatingSliders = true;
        mouseSensitivitySlider.value = GameSettings.mouseSensitivity;
        zoomSensitivitySlider.value = GameSettings.zoomSensitivity;
        volumeSlider.value = GameSettings.volume;
        updatingSliders = false;
    }

}
