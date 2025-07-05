using UnityEngine;

public class EnemyAI2 : MonoBehaviour
{
    public Transform player;              // اللاعب
    public float vanishDistance = 4f;     // المسافة المطلوبة للاختفاء
    public GameObject vanishEffect;       // Prefab الدخان
    public Transform effectSpawnPoint;    // مكان ظهور الدخان
    public AudioClip vanishSound;         // ملف الصوت
    public AudioSource audioSource;       // AudioSource لتشغيل الصوت

    bool hasVanished = false;

    void Update()
    {
        if (hasVanished || !player) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist < vanishDistance)
        {
            Vanish();
        }
    }

    void Vanish()
    {
        hasVanished = true;

        // تشغيل صوت
        if (audioSource && vanishSound)
            audioSource.PlayOneShot(vanishSound);

        // مؤثر الدخان
        if (vanishEffect)
        {
            Vector3 fxPos = effectSpawnPoint ? effectSpawnPoint.position : transform.position + Vector3.up * 0.7f;
            GameObject fx = Instantiate(vanishEffect, fxPos, Quaternion.identity);
            Destroy(fx, 2f);
        }

        // حذف العدو
        Destroy(gameObject);
    }
}
