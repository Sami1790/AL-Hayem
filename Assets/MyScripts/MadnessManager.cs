using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// مدير الجنون: يتحكم في زيادة الجنون، ربطه بالتأثيرات البصرية،
/// واجهة الجنون، مستوى موسيقى الجنون، وتحميل مشهد الخسارة،
/// ويستدعي التولتيب التوضيحي.
/// </summary>
public class MadnessManager : MonoBehaviour
{
    [Range(0, 100)]
    public float madness = 0f;

    [Header("Madness Settings")]
    public float madnessDuration = 180f;
    public float madnessOnHit = 10f;
    public float madnessOnKill = -5f;

    [Header("UI")]
    public Slider madnessBar;
    public GameObject madnessUI;             // Panel أو Canvas فرعي (اجعله مخفي أولاً)
    public CanvasGroup madnessCanvasGroup;   // حط CanvasGroup على نفس Panel واربطة هنا

    public TooltipPanel tooltipPanel;        // مرجع لسكربت التولتيب (اسحبه في الانسبكتور)

    [Header("Post-Processing")]
    public Volume postProcessVolume;

    // داخلي
    private Vignette vignette;
    private ChromaticAberration chroma;
    private FilmGrain filmGrain;

    private float madnessPerSecond;
    private SoundManager soundManager;
    private bool madnessActive = false;

    void Start()
    {
        madnessPerSecond = 100f / madnessDuration;

        // إخفاء UI الجنون في البداية
        if (madnessUI)
            madnessUI.SetActive(false);
        if (madnessCanvasGroup)
            madnessCanvasGroup.alpha = 0f;

        // شريط الجنون
        if (madnessBar)
        {
            madnessBar.minValue = 0f;
            madnessBar.maxValue = 1f;
            madnessBar.value = madness / 100f;
        }

        // مراجع البوست بروسيسنج
        if (postProcessVolume)
        {
            postProcessVolume.profile.TryGet(out vignette);
            postProcessVolume.profile.TryGet(out chroma);
            postProcessVolume.profile.TryGet(out filmGrain);
        }

        // ربط الساوند مانجر
        soundManager = SoundManager.Instance;
    }

    void Update()
    {
        if (!madnessActive)
            return; // لا تعمل أي شيء حتى يتم التفعيل

        madness += madnessPerSecond * Time.deltaTime;
        madness = Mathf.Clamp(madness, 0, 100);

        float madnessPercent = madness / 100f;

        // التحكم بتأثيرات البيئة حسب الجنون
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
            filmGrain.response.value  = Mathf.Lerp(0.7f, 1.2f, madnessPercent);
        }

        // ربط مستوى الموسيقى مع الجنون
        if (soundManager != null && soundManager.musicSource != null)
        {
            soundManager.musicSource.volume = Mathf.Lerp(0.13f, 0.5f, madnessPercent);
        }

        // إذا امتلأ الجنون ينتقل لمشهد الخسارة
        if (madness >= 100)
        {
            SceneManager.LoadScene("LoseScene");
        }
    }

    // تفعيل الجنون يدوي (نادِها من التريجر أو أي حدث)
    public void ActivateMadness()
    {
        madnessActive = true;

        if (madnessUI)
            madnessUI.SetActive(true);

        // تشغيل فيد واجهة الجنون إذا فيه CanvasGroup
        if (madnessCanvasGroup)
            StartCoroutine(FadeInMadnessUI());

        // يبدأ موسيقى الجنون إذا لم تكن تعمل
        if (soundManager != null)
            soundManager.PlayMadnessMusic(0.13f); // يبدأ منخفض

        // ======= استدعاء التولتيب =======
        if (tooltipPanel != null)
            tooltipPanel.ShowPanel();
    }

    // Fade-In للواجهة
    private IEnumerator FadeInMadnessUI()
    {
        float duration = 0.4f;
        float t = 0;
        madnessCanvasGroup.alpha = 0f;
        madnessCanvasGroup.gameObject.SetActive(true);

        while (t < duration)
        {
            madnessCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        madnessCanvasGroup.alpha = 1f;
    }

    // زيادة الجنون (مثلاً عند لمس العدو)
    public void AddMadness(float amount)
    {
        madness = Mathf.Clamp(madness + amount, 0, 100);
    }

    // تقليل الجنون (مثلاً عند قتل عدو)
    public void ReduceMadness(float amount)
    {
        madness = Mathf.Clamp(madness - amount, 0, 100);
    }
}
