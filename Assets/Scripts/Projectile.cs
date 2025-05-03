using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Movement2D movement2D;
    private Transform target;
    private float damage;

    public void SetUp(Transform target, float damage)
    {
        movement2D = GetComponent<Movement2D>();
        this.target = target;
        this.damage = damage;

    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            // 발사체 이동
            Vector3 direction = (target.position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
            return;
        if(collision.transform != target)
            return;

        //collision.GetComponent<Enemy>().OnDie();
        collision.GetComponent<EnemyHP>().TakeDamage(damage);
        Destroy(gameObject);
    }
}
