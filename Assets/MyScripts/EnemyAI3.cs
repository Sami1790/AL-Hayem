using UnityEngine;
using UnityEngine.AI;

public class EnemyAI3 : MonoBehaviour
{
    public Transform player;               // اللاعب
    public float crawlSpeed = 2.5f;        // سرعة الزحف
    public float chaseStartDistance = 25f; // يبدأ يطارد إذا قرب اللاعب
    public float vanishDistance = 1.1f;    // يختفي أو يهجم إذا قرب
    public GameObject vanishEffect;        // افكت الدخان
    public Transform effectSpawnPoint;     // مكان ظهور الدخان
    public AudioClip vanishSound;          // الصوت
    public AudioSource audioSource;        // مصدر الصوت

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
        agent.enabled = false; // يبدأ غير مفعل، يتحرك فقط بعد اقتراب اللاعب

        // يبدأ الأنميشن على Idle (تأكد أن Animator Default هو Idle)
        animator.SetBool("isCrawling", false);
    }

    void Update()
    {
        if (!player || hasVanished) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // يبدأ المطاردة إذا اقترب اللاعب من المسافة المطلوبة
        if (!isChasing && dist <= chaseStartDistance)
        {
            isChasing = true;
            animator.SetBool("isCrawling", true); // يحول من Idle إلى زحف
            agent.enabled = true;
            agent.Warp(transform.position); // يثبت مكانه قبل التحرك
            agent.SetDestination(player.position);
        }

        if (isChasing && !hasVanished)
        {
            agent.SetDestination(player.position);

            if (dist < vanishDistance)
            {
                Vanish();
            }
        }
    }

    void Vanish()
    {
        hasVanished = true;

        // شغل الصوت
        if (audioSource && vanishSound)
            audioSource.PlayOneShot(vanishSound);

        // طلع الدخان
        if (vanishEffect)
        {
            Vector3 fxPos = effectSpawnPoint ? effectSpawnPoint.position : transform.position + Vector3.up * 0.5f;
            GameObject fx = Instantiate(vanishEffect, fxPos, Quaternion.identity);
            Destroy(fx, 2f);
        }

        // احذف الزاحف
        Destroy(gameObject);
    }
}
