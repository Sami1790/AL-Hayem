using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MadnessManager : MonoBehaviour
{
    [Range(0, 100)]
    public float madness = 0f;

    public float madnessDuration = 180f;
    public float madnessPerSecond;
    public float madnessOnHit = 10f;
    public float madnessOnKill = -5f;

    public Slider madnessBar;

    public Volume postProcessVolume; // اسحب الـ Global Volume هنا

    Vignette vignette;
    ChromaticAberration chroma;
    FilmGrain filmGrain; // هنا التغيير

    void Start()
    {
        madnessPerSecond = 100f / madnessDuration;

        if (madnessBar)
        {
            madnessBar.minValue = 0f;
            madnessBar.maxValue = 1f;
            madnessBar.value = madness / 100f;
        }

        if (postProcessVolume)
        {
            postProcessVolume.profile.TryGet(out vignette);
            postProcessVolume.profile.TryGet(out chroma);
            postProcessVolume.profile.TryGet(out filmGrain);
        }
    }

    void Update()
    {
        madness += madnessPerSecond * Time.deltaTime;
        madness = Mathf.Clamp(madness, 0, 100);

        float madnessPercent = madness / 100f;

        RenderSettings.fogDensity = Mathf.Lerp(0.03f, 0.07f, madnessPercent);

        if (madnessBar)
            madnessBar.value = madnessPercent;

        if (vignette != null)
            vignette.intensity.value = Mathf.Lerp(0.18f, 0.52f, madnessPercent);

        if (chroma != null)
            chroma.intensity.value = Mathf.Lerp(0f, 0.85f, madnessPercent);

        if (filmGrain != null)
        {
            filmGrain.intensity.value = Mathf.Lerp(0.05f, 0.38f, madnessPercent);
            filmGrain.response.value  = Mathf.Lerp(0.7f, 1.2f, madnessPercent); // response بدلاً من size
        }

        if (madness >= 100)
        {
            Debug.Log("جنون كامل!");
            // هنا تقدر تضيف End Screen أو غيره
        }
    }

    public void AddMadness(float amount)
    {
        madness = Mathf.Clamp(madness + amount, 0, 100);
    }

    public void ReduceMadness(float amount)
    {
        madness = Mathf.Clamp(madness - amount, 0, 100);
    }
}
