using UnityEngine;

public class EnemyAI2 : MonoBehaviour
{
    public Transform player;
    public float vanishDistance = 4f;
    public GameObject vanishEffect;
    public Transform effectSpawnPoint;
    public AudioClip vanishSound;
    public AudioSource audioSource;

    public float madnessReduceOnKill = 5f; // ينقص جنون إذا قُتل
    public float madnessAddOnVanish = 10f; // يزيد جنون إذا قرب اللاعب

    bool hasVanished = false;

    void Update()
    {
        if (hasVanished || !player) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < vanishDistance)
        {
            Die(false); // اختفاء بسبب القرب من اللاعب
        }
    }

    // تناديها رصاصة اللاعب
    public void TakeHit()
    {
        Die(true); // قُتل بالسلاح
    }

    // دالة موحدة للموت: لو قُتل بالسلاح أو قرب اللاعب
    void Die(bool killedByPlayer)
    {
        if (hasVanished) return;
        hasVanished = true;

        // شغل الصوت
        if (audioSource && vanishSound)
            audioSource.PlayOneShot(vanishSound);

        // مؤثر الدخان
        if (vanishEffect)
        {
            Vector3 fxPos = effectSpawnPoint ? effectSpawnPoint.position : transform.position + Vector3.up * 0.7f;
            GameObject fx = Instantiate(vanishEffect, fxPos, Quaternion.identity);
            Destroy(fx, 2f);
        }

        // تحكم بالجنون
        MadnessManager madness = FindFirstObjectByType<MadnessManager>();
        if (madness)
        {
            if (killedByPlayer)
                madness.ReduceMadness(madnessReduceOnKill);
            else
                madness.AddMadness(madnessAddOnVanish);
        }

        // حذف العدو
        Destroy(gameObject);
    }
}
