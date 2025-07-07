using UnityEngine;
using UnityEngine.AI;

public class EnemyAI3 : MonoBehaviour
{
    public Transform player;
    public float crawlSpeed = 2.5f;
    public float chaseStartDistance = 25f;
    public float vanishDistance = 1.1f;
    public GameObject vanishEffect;
    public Transform effectSpawnPoint;
    public AudioClip vanishSound;
    public AudioSource audioSource;

    public float madnessReduceOnKill = 5f; // ينقص الجنون إذا قتله اللاعب
    public float madnessAddOnVanish = 10f; // يزيد الجنون إذا وصل قريب

    Animator animator;
    NavMeshAgent agent;
    bool hasVanished = false;
    bool isChasing = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = crawlSpeed;
        agent.stoppingDistance = vanishDistance;
        agent.enabled = false;

        // يبدأ على Idle
        if (animator) animator.SetBool("isCrawling", false);
    }

    void Update()
    {
        if (!player || hasVanished) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // يبدأ يزحف إذا قرب اللاعب
        if (!isChasing && dist <= chaseStartDistance)
        {
            isChasing = true;
            if (animator) animator.SetBool("isCrawling", true);
            agent.enabled = true;
            agent.Warp(transform.position);
            agent.SetDestination(player.position);
        }

        // إذا صار يلاحق
        if (isChasing && !hasVanished)
        {
            agent.SetDestination(player.position);

            // إذا اقترب يختفي ويزيد الجنون
            if (dist < vanishDistance)
            {
                Die(false);
            }
        }
    }

    // إذا أُصيب بطلقة ينقص الجنون
    public void TakeHit()
    {
        Die(true);
    }

    // دالة موحدة للموت
    void Die(bool killedByPlayer)
    {
        if (hasVanished) return;
        hasVanished = true;

        // شغل الصوت
        if (audioSource && vanishSound)
            audioSource.PlayOneShot(vanishSound);

        // الدخان
        if (vanishEffect)
        {
            Vector3 fxPos = effectSpawnPoint ? effectSpawnPoint.position : transform.position + Vector3.up * 0.5f;
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

        Destroy(gameObject);
    }
}
