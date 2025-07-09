using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("SFX Clips")]
    public AudioClip gunShotClip;
    public AudioClip gunClickClip;
    public AudioClip enemyDeathClip;

    [Header("Music Clips")]
    public AudioClip madnessMusicClip;

    void Awake()
    {
        // Singleton قوي: إذا فيه نسخة قديمة امسحها وخلك انت الوحيد
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // (1) حدث الأصوات أول ما يبدأ المشهد
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // (2) إذا دخلت مشهد الفوز أو الخسارة... طفي الموسيقى!
        if (scene.name == "WinScene" || scene.name == "LoseScene")
        {
            StopMusic();
        }
        // (3) إذا رجعت للقائمة الرئيسية... طفي الموسيقى
        if (scene.name == "MainMenu")
        {
            StopMusic();
        }
        // (4) لو دخلت مشهد اللعب، أعد تهيئة الـSources إذا احتجت (مثلاً Reset الصوت)
    }

    // تشغيل المؤثرات الصوتية (SFX)
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }

    // تشغيل الموسيقى (مع استبدال المقطع الحالي)
    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        if (clip != null && musicSource != null)
        {
            if (musicSource.clip != clip)
                musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // إيقاف الموسيقى
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PlayGunShot()    { PlaySFX(gunShotClip); }
    public void PlayGunClick()   { PlaySFX(gunClickClip); }
    public void PlayEnemyDeath() { PlaySFX(enemyDeathClip); }
    public void PlayMadnessMusic(float volume = 1f) { PlayMusic(madnessMusicClip, volume); }
}
