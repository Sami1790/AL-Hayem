using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float walkSpeed = 1.8f;
    public float runSpeed = 4.5f;
    public float startChaseDistance = 7f;
    public float stopChaseDistance = 10f;
    public float vanishDistance = 1.2f;
    public float wanderRadius = 8f;
    public float wanderTimer = 5f;

    Animator animator;
    NavMeshAgent agent;
    bool isChasing = false;
    float timer;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        agent.stoppingDistance = vanishDistance;
        timer = wanderTimer;
    }

    void Update()
    {
        if (!player) return;
        float dist = Vector3.Distance(transform.position, player.position);

        // يبدأ يلاحق إذا قرب اللاعب
        if (!isChasing && dist < startChaseDistance)
        {
            isChasing = true;
            animator.SetBool("isRunning", true);
            agent.speed = runSpeed;
        }
        // يرجع يهيم إذا ابتعد اللاعب
        else if (isChasing && dist > stopChaseDistance)
        {
            isChasing = false;
            animator.SetBool("isRunning", false);
            agent.speed = walkSpeed;
            timer = wanderTimer; // يعطيه بداية جديدة للتايمر
        }

        if (isChasing && dist > vanishDistance)
        {
            agent.SetDestination(player.position);
        }
        else if (!isChasing)
        {
            // يهيم/يمشي عشوائي
            timer += Time.deltaTime;
            // استخدم شرط وصول ذكي
            if (timer >= wanderTimer || 
                (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.25f))
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, NavMesh.AllAreas);
                agent.SetDestination(newPos);
                timer = 0;
            }
        }
        else if (dist <= vanishDistance)
        {
            Destroy(gameObject);
        }
    }

    // يولد نقطة عشوائية ضمن دائرة حول العدو
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int areaMask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection.y = 0; // حافظ على نفس المستوى
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, areaMask);
        return navHit.position;
    }
}
