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
        animator.SetBool("isChasing", false);

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
    }

    void Update()
    {
        if (!player || hasVanished) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // يبدأ الهجوم إذا قرب اللاعب
        if (!isChasing && dist < chaseStartDistance)
        {
            isChasing = true;
            animator.SetBool("isChasing", true);
            agent.speed = chaseSpeed;
        }

        // مطاردة اللاعب
        if (isChasing && !hasVanished)
        {
            agent.SetDestination(player.position);
            if (dist < vanishDistance)
                Vanish();
        }
        // باترول
        else if (!isChasing && patrolPoints.Length > 0)
        {
            // إذا وصل نقطة الباترول ينتقل للي بعدها فوراً بدون لف ولا تأخير
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
            {
                GoToNextPatrolPoint();
            }
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        currentPoint = (currentPoint + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPoint].position);
    }

    void Vanish()
    {
        hasVanished = true;

        if (audioSource && vanishSound)
            audioSource.PlayOneShot(vanishSound);

        if (vanishEffect)
        {
            Vector3 fxPos = effectSpawnPoint ? effectSpawnPoint.position : transform.position + Vector3.up * 0.5f;
            GameObject fx = Instantiate(vanishEffect, fxPos, Quaternion.identity);
            Destroy(fx, 2f);
        }
        Destroy(gameObject);
    }
}
