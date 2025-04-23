using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    private Transform target;

    public void SetTarget(Transform enemy)
    {
        target = enemy;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance < 0.5f)
        {
            target.GetComponent<Soldier_AI>()?.TakeDamage(25f);
            Destroy(gameObject);
        }
    }
}
