using UnityEngine;
using UnityEngine.AI;

public class EnemyAI4 : MonoBehaviour
{
    public Transform player;
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 6f;
    public float chaseStartDistance = 8f;
    public float vanishDistance = 1.1f;
    public Transform[] patrolPoints;
    public GameObject vanishEffect;
    public Transform effectSpawnPoint;
    public AudioClip vanishSound;
    public AudioSource audioSource;

    public float madnessReduceOnKill = 5f; // ينقص الجنون عند قتل العدو
    public float madnessAddOnVanish = 10f; // يزيد الجنون عند اقتراب العدو من اللاعب

    Animator animator;
    NavMeshAgent agent;
    int currentPoint = 0;
    bool isChasing = false;
    bool hasVanished = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        agent.stoppingDistance = vanishDistance;
        if (animator) animator.SetBool("isChasing", false);

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
    }

    void Update()
    {
        if (!player || hasVanished) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // يبدأ المطاردة إذا قرب اللاعب
        if (!isChasing && dist < chaseStartDistance)
        {
            isChasing = true;
            if (animator) animator.SetBool("isChasing", true);
            agent.speed = chaseSpeed;
        }

        // مطاردة اللاعب
        if (isChasing && !hasVanished)
        {
            agent.SetDestination(player.position);
            if (dist < vanishDistance)
                Die(false); // هنا: وصل اللاعب (يزيد الجنون)
        }
        // باترول
        else if (!isChasing && patrolPoints.Length > 0)
        {
            // ينتقل لنقطة الباترول التالية فوراً
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
                GoToNextPatrolPoint();
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        currentPoint = (currentPoint + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPoint].position);
    }

    // إذا انقتل من اللاعب ينقص الجنون
    public void TakeHit()
    {
        Die(true);
    }

    // unified death: يفرق إذا قتل اللاعب أو اختفى بنفسه
    void Die(bool killedByPlayer)
    {
        if (hasVanished) return;
        hasVanished = true;

        if (audioSource && vanishSound)
            audioSource.PlayOneShot(vanishSound);

        if (vanishEffect)
        {
            Vector3 fxPos = effectSpawnPoint ? effectSpawnPoint.position : transform.position + Vector3.up * 0.5f;
            GameObject fx = Instantiate(vanishEffect, fxPos, Quaternion.identity);
            Destroy(fx, 2f);
        }

        // التحكم بالجنون
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
