using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleIntroCamera : MonoBehaviour
{
    public AudioSource introAudio;
    public string nextSceneName = "Main_Scene";
    public float rotationSpeed = 10f;
    public float rotationAmount = 40f;    // درجة الميلان يمين/يسار
    public float delayBeforeSound = 2f;   // تأخير قبل يبدأ الصوت

    private float timer = 0f;
    private float phase = 0f;
    private bool audioStarted = false;
    private Quaternion startRot;

    void Start()
    {
        startRot = transform.rotation;
        if (introAudio)
        {
            introAudio.Stop(); // يتأكد انه ما يشتغل بالبداية
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 1. الكاميرا تتحرك يمين أول 10 ثواني
        if (timer < 10f)
        {
            float t = timer / 10f;
            float angle = Mathf.Lerp(0f, rotationAmount, t);
            transform.rotation = startRot * Quaternion.Euler(0, angle, 0);
        }
        // 2. بعدين تتحرك شوي يسار (من 10 إلى 14 ثانية مثلاً)
        else if (timer < 14f)
        {
            float t = (timer - 10f) / 4f;
            float angle = Mathf.Lerp(rotationAmount, -rotationAmount * 0.45f, t); // 45% يسار
            transform.rotation = startRot * Quaternion.Euler(0, angle, 0);
        }
        // 3. بعدين تثبت
        else
        {
            transform.rotation = startRot * Quaternion.Euler(0, -rotationAmount * 0.45f, 0);
        }

        // تشغيل الصوت بعد التأخير (delay)
        if (!audioStarted && timer >= delayBeforeSound)
        {
            if (introAudio)
                introAudio.Play();
            audioStarted = true;
        }

        // ما ينتقل إلا إذا الصوت فعلاً انتهى
        if (audioStarted && introAudio && !introAudio.isPlaying)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
