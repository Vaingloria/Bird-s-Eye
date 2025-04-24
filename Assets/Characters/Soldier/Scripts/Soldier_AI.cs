using UnityEngine;
using UnityEngine.AI;

public class Soldier_AI : MonoBehaviour
{
    public enum Team { Red, Blue }
    public Team myTeam;

    [Header("Combat Settings")]
    public float health = 100f;
    public float shootRange = 10f;
    public float shootRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Detection Settings")]
    public float visionRange = 25f;
    public LayerMask enemyLayer;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 2f;

    [Header("Animation")]
    public Animator animator;

    [Header("Drop Settings")]
    public GameObject ammoBoxPrefab;
    public float dropForce = 2f;

    private NavMeshAgent agent;
    private Transform target;
    private bool isDead = false;
    private float shootTimer;
    private int currentPatrolIndex = 0;
    private float patrolWaitTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        shootTimer = shootRate;

        // Auto-assign enemyLayer based on team
        if (myTeam == Team.Red)
            enemyLayer = LayerMask.GetMask("BlueTeam");
        else
            enemyLayer = LayerMask.GetMask("RedTeam");

        // Debug assigned layer
        int layerIndex = Mathf.RoundToInt(Mathf.Log(enemyLayer.value, 2));
        Debug.Log($"[{gameObject.name}] Will detect enemies in layer: {LayerMask.LayerToName(layerIndex)}");

        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogError($"{gameObject.name} is missing NavMeshAgent or not on NavMesh!");
            enabled = false;
        }
    }

    void Update()
    {
        if (isDead) return;

        FindTarget();
        HandleMovementAndCombat();
        UpdateAnimatorMovementParams();

        if (target == null)
        {
            Patrol();
        }
    }

    void FindTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, enemyLayer);
        float closest = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (var hit in hits)
        {
            Soldier_AI other = hit.GetComponent<Soldier_AI>();
            Debug.Log($"{gameObject.name} -> Hit: {hit.name}, Layer: {LayerMask.LayerToName(hit.gameObject.layer)}");

            if (other != null && other.myTeam != this.myTeam && !other.isDead)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < closest)
                {
                    closest = distance;
                    bestTarget = hit.transform;
                    Debug.Log($"✅ Valid enemy: {hit.name}");
                }
            }
        }

        target = bestTarget;

        if (target == null)
            Debug.LogWarning($"{gameObject.name} found NO valid target.");
    }


    // void FindTarget()
    // {
    //     Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, enemyLayer);
    //     float closest = Mathf.Infinity;
    //     Transform bestTarget = null;

    //     foreach (var hit in hits)
    //     {
    //         Soldier_AI other = hit.GetComponent<Soldier_AI>();
    //         if (other != null && other.myTeam != this.myTeam && !other.isDead)
    //         {
    //             float distance = Vector3.Distance(transform.position, other.transform.position);
    //             if (distance < closest)
    //             {
    //                 closest = distance;
    //                 bestTarget = other.transform;
    //             }
    //         }
    //     }

    //     target = bestTarget;
    // }

    void HandleMovementAndCombat()
    {
        // Debug.Log($"{gameObject.name} is FORCIBLY SHOOTING!");
        // Shoot();  // Temporarily bypass timer

        if (target == null)
        {
            Debug.LogWarning($"{gameObject.name} has no target. isShooting = false");
            animator.SetBool("isShooting", false);
            agent.ResetPath();
            return;
        }
        float distance = Vector3.Distance(transform.position, target.position);
        float engageBuffer = 1.5f;
        Debug.Log($"{gameObject.name} distance to target: {distance}");

        if (distance > shootRange + 0.5f)// - engageBuffer)
        {
            Debug.Log($"{gameObject.name} is chasing target...");
            agent.SetDestination(target.position);
            animator.SetBool("isShooting", false);
            //Debug.Log($"{gameObject.name} is moving to target");    
        }
        else
        {
            Debug.Log($"{gameObject.name} is within shooting range!");
            agent.ResetPath();
            FaceTarget();
            shootTimer -= Time.deltaTime;
            Debug.Log($"{gameObject.name} shootTimer: {shootTimer}");
            if (shootTimer <= 0f)
            {
                Debug.Log($"{gameObject.name} is SHOOTING!");
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
        if (bulletPrefab == null)
        {
            Debug.LogWarning($"{gameObject.name} is missing bulletPrefab!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning($"{gameObject.name} is missing firePoint!");
            return;
        }

        if (target == null)
        {
            Debug.LogWarning($"{gameObject.name} has no target.");
            return;
        }

        animator.SetBool("isShooting", true);
        Debug.Log($"{gameObject.name} is SHOOTING at {target.name}");

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<Bullet>().SetTarget(target);
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

    void Patrol()
    {
        if (patrolPoints.Length == 0 || agent == null || !agent.isOnNavMesh) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolWaitTimer += Time.deltaTime;

            if (patrolWaitTimer >= patrolWaitTime)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                patrolWaitTimer = 0f;
            }
        }
        else
        {
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
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
        DropAmmoBox();
        Destroy(gameObject, 4f);
    }

    void DropAmmoBox()
    {
        if (ammoBoxPrefab != null)
        {
            Vector3 dropPos = transform.position + Vector3.up * 1f;
            GameObject drop = Instantiate(ammoBoxPrefab, dropPos, Quaternion.identity);

            Rigidbody rb = drop.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 forceDir = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f)).normalized;
                rb.AddForce(forceDir * dropForce, ForceMode.Impulse);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
