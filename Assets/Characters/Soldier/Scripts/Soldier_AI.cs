using UnityEngine;
using UnityEngine.AI;

public class Soldier_AI : MonoBehaviour
{
    public enum Team { Red, Blue }
    public Team myTeam;

    [Header("Combat Settings")]
    public float health = 100f;
    public float shootRange = 25f;
    public float shootRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Animation & Targeting")]
    public LayerMask enemyLayer;
    public Animator animator;

    private float shootTimer;
    private Transform target;
    private NavMeshAgent agent;
    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        shootTimer = shootRate;

        if (agent == null)
        {
            Debug.LogError($"{gameObject.name} is missing a NavMeshAgent!");
            enabled = false;
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"{gameObject.name} is not placed on the NavMesh!");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (isDead) return;

        FindTarget();
        HandleMovementAndCombat();
        UpdateAnimatorMovementParams();
    }

    void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, shootRange, enemyLayer);
        float closest = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (var hit in hits)
        {
            Soldier_AI other = hit.GetComponent<Soldier_AI>();
            if (other != null && other.myTeam != this.myTeam && !other.isDead)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closest)
                {
                    closest = distance;
                    bestTarget = hit.transform;
                }
            }
        }

        target = bestTarget;
    }

    void HandleMovementAndCombat()
    {
        if (target == null)
        {
            agent.ResetPath();
            animator.SetBool("isShooting", false);
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > shootRange)
        {
            agent.SetDestination(target.position);
            animator.SetBool("isShooting", false);
        }
        else
        {
            agent.ResetPath();
            FaceTarget();

            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootRate;
            }
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    void Shoot()
    {
        if (bulletPrefab && firePoint && target != null)
        {
            animator.SetBool("isShooting", true);

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Bullet>().SetTarget(target);
        }
    }

    void UpdateAnimatorMovementParams()
    {
        if (agent == null || animator == null) return;

        Vector3 velocity = agent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        animator.SetFloat("moveX", localVelocity.x);
        animator.SetFloat("moveZ", localVelocity.z);

        if (velocity.magnitude > 0.1f)
        {
            animator.SetBool("isShooting", false);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        agent.enabled = false;
        animator.SetTrigger("die");
        Destroy(gameObject, 4f);
    }
}
