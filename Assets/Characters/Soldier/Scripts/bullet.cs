using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float damage = 25f;

    private Transform target;
    private float lifeTime = 5f; // Prevents bullets from lingering forever

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        //Debug.Log($"Bullet assigned target: {target?.name ?? "null"}");
    }

    void Update()
    {
        // Safety: Destroy if target disappears
        if (target == null)
        {
            //Debug.LogWarning("Bullet target lost. Destroying bullet.");
            Destroy(gameObject);
            return;
        }

        // Move toward target
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Optional: Rotate bullet to face target direction
        transform.forward = direction;

        // Impact check
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < 0.5f)
        {
            Soldier_AI enemy = target.GetComponent<Soldier_AI>();
            if (enemy != null)
            {
                //Debug.Log($"Bullet hit {target.name}, applying {damage} damage.");
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        // Lifetime timeout
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            //Debug.Log("Bullet lifetime expired. Destroying.");
            Destroy(gameObject);
        }
    }
}
