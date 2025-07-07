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
    public Transform effectSpawnPoint;

    Animator animator;
    NavMeshAgent agent;
    int currentPoint = 0;
    bool isChasing = false;
    bool hasVanished = false;
    float waitTimer = 0f;
    public float patrolWait = 2.5f;

    public float madnessReduceOnKill = 5f; // من السلاح
    public float madnessAddOnVanish = 10f; // إذا وصل اللاعب

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
                // اختفى بسبب القرب من اللاعب (يزيد الجنون)
                Die(false);
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

    // دالة موحدة للموت
    // killedByPlayer = true إذا قتله السلاح، false إذا قرب منه اللاعب
    public void Die(bool killedByPlayer)
    {
        if (hasVanished) return;
        hasVanished = true;

        // تأثير الدخان
        if (vanishEffect)
        {
            Vector3 spawnPos = effectSpawnPoint ? effectSpawnPoint.position : transform.position;
            GameObject fx = Instantiate(vanishEffect, spawnPos, Quaternion.identity);
            Destroy(fx, 2f);
        }

        // التحكم في الجنون حسب نوع الموت
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

    // تناديها رصاصة اللاعب
    public void TakeHit()
    {
        Die(true);
    }
}
