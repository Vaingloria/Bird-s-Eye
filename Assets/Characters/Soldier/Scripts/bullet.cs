using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float damage = 25f;
    public float lifeTime = 5f;

    private Transform target;

    void Start()
    {
        // Auto-destroy in case it doesn't hit anything
        Destroy(gameObject, lifeTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move toward the target
        Vector3 direction = (target.position - transform.position).normalized;
        float step = speed * Time.deltaTime;

        transform.position += direction * step;
        transform.LookAt(target);

        // Optional: check if very close and register hit
        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        Soldier_AI enemy = target.GetComponent<Soldier_AI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Optional: add hit effects here
        Destroy(gameObject);
    }
}
