using UnityEngine;
using System.Collections;

public enum WeaponState { SearchTarget = 0, AttackToTarget }

public class TowerWeapon : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab; // Prefab for the projectile
    [SerializeField] private Transform firePoint; // Point from where the projectile will be fired
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private int attackDamage = 2;

    private Transform attackTarget = null;
    private EnemySpawner enemySpawner;
    private WeaponState weaponState = WeaponState.SearchTarget;

    public void SetUP(EnemySpawner enemySpawner)
    {
        this.enemySpawner = enemySpawner;

        // Start the weapon in the SearchTarget state
        ChangeState(WeaponState.SearchTarget);
    }

    public void ChangeState(WeaponState newState)
    {
        // Stop the current state coroutine
        StopCoroutine(weaponState.ToString());

        weaponState = newState;
        // Start the new state coroutine
        StartCoroutine(weaponState.ToString());
    }

    void Update()
    {
        if (attackTarget != null)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        // 원점으로부터의 거리와 수평죽으로부터의 각도를 이용해 위치를 구하는 극 좌표게 이용
        // 각도 = arctan(y/x)

        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;

        // Mathf.Rad2Deg : Radians to Degrees
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            // 제일 가까운 적의 거리 - 최초값은 무한대
            float closestDistSqr = Mathf.Infinity;

            for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
            {
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);

                // 적과의 거리와 공격 범위 비교
                // 공격 범위보다 가까운 적이면서 가장 가까운 적을 찾는다
                if (distance < attackRange && distance <= closestDistSqr)
                {
                    closestDistSqr = distance;
                    attackTarget = enemySpawner.EnemyList[i].transform;
                }
            }

            // 상태 변경
            if (attackTarget != null)
            {
                ChangeState(WeaponState.AttackToTarget);
            }

            yield return null;
        }
    }

    private IEnumerator AttackToTarget()
    {
        while (true)
        {
            if (attackTarget == null)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if (distance > attackRange)
            {
                attackTarget = null;
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            yield return new WaitForSeconds(fireRate);

            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        clone.GetComponent<Projectile>().SetUp(attackTarget, attackDamage);
    }
}
