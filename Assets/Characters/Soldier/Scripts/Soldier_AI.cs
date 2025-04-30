using UnityEngine;
using UnityEngine.AI;

public class Soldier_AI : MonoBehaviour
{
    public enum Team { RedTeam, BlueTeam }
    public Team myTeam;

    public float health = 100f;
    public float shootRange = 50f;
    public float shootRate = 1f;
    private float shootTimer;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public float visionRange = 150f;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip deathClip;

    private NavMeshAgent agent;
    private bool isDead = false;
    public bool IsDead => isDead;
    private Transform target;

    public bool edible = true;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        shootTimer = shootRate;
    }

    void Update()
    {
        if (agent != null && animator != null)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
            animator.SetFloat("moveX", localVelocity.x);
            animator.SetFloat("moveZ", localVelocity.z);
        }

        if (isDead) return;

        FindTarget();

        if (target != null && !isDead)
        {
            agent.SetDestination(target.position);

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= shootRange)
            {
                transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                shootTimer -= Time.deltaTime;

                if (shootTimer <= 0f)
                {
                    Shoot(target);
                    shootTimer = shootRate;
                }
            }
        }
    }

    void FindTarget()
    {
        Soldier_AI[] allSoldiers = FindObjectsOfType<Soldier_AI>();
        float minDist = Mathf.Infinity;
        Transform closest = null;

        foreach (Soldier_AI soldier in allSoldiers)
        {
            if (soldier == null || soldier == this || soldier.isDead || soldier.myTeam == this.myTeam)
                continue;

            float dist = Vector3.Distance(transform.position, soldier.transform.position);
            if (dist <= visionRange && dist < minDist)
            {
                closest = soldier.transform;
                minDist = dist;
            }
        }

        target = closest;
    }

    void Shoot(Transform currentTarget)
    {
        if (bulletPrefab == null || firePoint == null || currentTarget == null) return;
        if (bulletPrefab == null)
        {
            //Debug.LogWarning($"{name} cannot shoot. Bullet prefab is not assigned!");
            return;
        }

        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Debug.DrawRay(firePoint.position, firePoint.forward * 5f, Color.red, 2f);
        //Debug.Log($"Instantiating bullet from {name} at {firePoint.position}");

        Bullet bulletScript = bulletGO.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            //Debug.Log($"Trying to assign target {currentTarget?.name ?? "null"} to bullet.");
            bulletScript.SetTarget(currentTarget);
            Debug.Log($"{name} shot at {currentTarget.name}");
        }

        if (audioSource && shootClip)
            audioSource.PlayOneShot(shootClip);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        //Debug.Log($"{name} took {damage} damage. Remaining: {health}");

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (animator != null)
        {
            animator.SetBool("isShooting", false);
            animator.SetTrigger("die");
        }

        if (audioSource && deathClip)
            audioSource.PlayOneShot(deathClip);

        this.enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}
