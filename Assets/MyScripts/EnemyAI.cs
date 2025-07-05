using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float patrolSpeed = 0.78f;
    public float runSpeed = 4.5f;
    public float startChaseDistance = 7f;
    public float vanishDistance = 1.2f;
    public Transform[] patrolPoints;
    public GameObject vanishEffect;
    public Transform effectSpawnPoint; // أضفه هنا

    Animator animator;
    NavMeshAgent agent;
    int currentPoint = 0;
    bool isChasing = false;
    bool hasVanished = false;
    float waitTimer = 0f;
    public float patrolWait = 2.5f;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = patrolSpeed;
        agent.stoppingDistance = vanishDistance;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
    }

    void Update()
    {
        if (!player || hasVanished) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!isChasing && dist < startChaseDistance)
        {
            isChasing = true;
            animator.SetBool("isRunning", true);
            agent.speed = runSpeed;
        }

        if (isChasing)
        {
            if (dist > vanishDistance)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                Vanish();
            }
        }
        else
        {
            if (!agent.pathPending && agent.remainingDistance < 0.3f)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= patrolWait)
                {
                    GoToNextPatrolPoint();
                    waitTimer = 0f;
                }
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
        if (vanishEffect)
        {
            Vector3 spawnPos = effectSpawnPoint ? effectSpawnPoint.position : transform.position;
            GameObject fx = Instantiate(vanishEffect, spawnPos, Quaternion.identity);
            Destroy(fx, 2f);
        }
        Destroy(gameObject);
    }
}
