using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class DamageEffectController : MonoBehaviour
{
    public static int DAMAGE_EFFECT_SINGLE = 0;
    public static int DAMAGE_EFFECT_PULSE = 1;

    [Header("Effect")]
    [SerializeField] Color colour1;
    [SerializeField] Color colour2;
    [SerializeField] float startLerpTime = 5f;
    [SerializeField] float endLerpTime = 1f;
    [SerializeField] float timeToEffectClear = 7f;

    PostProcessVolume volume;
    Vignette vignette;
    Color targetColour;

    bool isPlaying = false;
    public int damageEffect = DAMAGE_EFFECT_SINGLE;

    float timeToClear;

    void Start()
    {
        volume = FindObjectOfType<PostProcessVolume>();
        volume.profile.TryGetSettings(out vignette);
    }

    void Update()
    {
        if (isPlaying)
        {
           
            if (damageEffect == DAMAGE_EFFECT_PULSE)
            {
                vignette.intensity.Override(0.45f);
                vignette.smoothness.Override(0.18f);
                vignette.color.Override(Color.Lerp(colour1, colour2, Mathf.PingPong(Time.time, 1)));
            } else
            {

                float t = startLerpTime * Time.deltaTime;

                if (Time.time > timeToClear)
                {
                    t = endLerpTime * Time.deltaTime;
                    // We need to clear the effect
                    if (vignette.color == colour1)
                    {
                        // has returned to normal, clear
                        Clear();
                        return;
                    }

                    // Normal Settings
                    vignette.intensity.Override(Mathf.Lerp(vignette.intensity, 0.315f, t));
                    vignette.smoothness.Override(Mathf.Lerp(vignette.smoothness, 0f, t));
                    vignette.color.Override(Color.Lerp(vignette.color, colour1, t));

                    return;
                }

                // Damaged Settings - May want to make these a bit more obvious
                vignette.intensity.Override(Mathf.Lerp(vignette.intensity, 0.45f, t));
                vignette.smoothness.Override(Mathf.Lerp(vignette.smoothness, 0.18f, t));
                vignette.color.Override(Color.Lerp(vignette.color, colour2, t));
            }
        } else
        {
            // Normal Settings
            vignette.intensity.Override(0.315f);
            vignette.smoothness.Override(0f);
            vignette.color.Override(colour1);
        }

    }

    public void SetPlaying(bool playing)
    {
        isPlaying = playing;
    }

    public void SetDamageEffect(int damageEffect)
    {
        this.damageEffect = damageEffect;
    }

    public void AddHit()
    {
        timeToClear = Time.time + timeToEffectClear;
    }

    public void Clear()
    {
        vignette.intensity.Override(0.315f);
        vignette.smoothness.Override(0f);
        vignette.color.Override(colour1);
        SetPlaying(false);
    }
}
