using UnityEngine;
using System.Collections;

public enum WeaponType { Cannon = 0, Laser, Slow, Buff, }
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, }

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]    
    [SerializeField] private TowerTemplate towertemplate; // Tower Stats
    [SerializeField] private Transform firePoint; // Point from where the projectile will be fired
    [SerializeField] private WeaponType weaponType;

    [Header("Cannon")]
    [SerializeField] private GameObject projectilePrefab; // Prefab for the projectile

    [Header("Laser")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform hitEffect;
    [SerializeField] private LayerMask targetLayer;

    private int level = 0;

    private Transform attackTarget = null;
    private EnemySpawner enemySpawner;
    private WeaponState weaponState = WeaponState.SearchTarget;

    private SpriteRenderer spriteRenderer;
    private PlayerGold playerGold;
    private Tile ownerTile;

    private TowerSpawner towerSpawner;
    private float addedDamage;
    private int buffLevel;

    public Sprite TowerSprite => towertemplate.weapon[level].sprite;
    public float Damage => towertemplate.weapon[level].damage;
    public float Slow => towertemplate.weapon[level].slow;
    public float Buff => towertemplate.weapon[level].buff;
    public float Rate => towertemplate.weapon[level].rate;
    public float Range => towertemplate.weapon[level].range;
    public int Level => level + 1;
    public int MaxLevel => towertemplate.weapon.Length;
    public int UpgradeCost => Level < MaxLevel ? towertemplate.weapon[level + 1].cost : 0;
    public int SellingPrice => towertemplate.weapon[level].sellingPrice;
    public float AddedDamage
    {
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
    }
    public int BuffLevel
    {
        set => buffLevel = Mathf.Max(0, value);
        get => buffLevel;
    }
    public WeaponType WeaponType => weaponType;

    public void SetUP(TowerSpawner towerSpawner, EnemySpawner enemySpawner, PlayerGold playerGold, Tile ownerTile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.towerSpawner = towerSpawner;
        this.enemySpawner = enemySpawner;
        this.playerGold = playerGold;
        this.ownerTile = ownerTile;

        if (weaponType == WeaponType.Laser || weaponType == WeaponType.Cannon)
        {
            // Start the weapon in the SearchTarget state
            ChangeState(WeaponState.SearchTarget);
        }
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
            attackTarget = FindClosestAttackTarget();

            // 상태 변경
            if (attackTarget != null)
            {
                if (weaponType == WeaponType.Cannon)
                {
                    ChangeState(WeaponState.TryAttackCannon);
                }
                else if (weaponType == WeaponType.Laser)
                {
                    ChangeState(WeaponState.TryAttackLaser);
                }
            }

            yield return null;
        }
    }

    private Transform FindClosestAttackTarget()
    {
        float closestDistSqr = Mathf.Infinity;

        for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);

            // 적과의 거리와 공격 범위 비교
            // 공격 범위보다 가까운 적이면서 가장 가까운 적을 찾는다
            if (distance < towertemplate.weapon[level].range && distance <= closestDistSqr)
            {
                closestDistSqr = distance;
                attackTarget = enemySpawner.EnemyList[i].transform;
            }
        }

        return attackTarget;
    }

    private IEnumerator TryAttackCannon()
    {
        while (true)
        {
            // target을 공격하는게 가능한지 검사
            if (IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            SpawnProjectile();
            // fireRate 만큼 대기
            yield return new WaitForSeconds(towertemplate.weapon[level].rate);
        }
    }

    private IEnumerator TryAttackLaser()
    {
        EnableLaser();

        while (true)
        {
            if (IsPossibleToAttackTarget() == false)
            {
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            SpawnLaser();
            yield return null;
        }
    }

    private bool IsPossibleToAttackTarget()
    {
        // 공격할 적이 없거나 공격 범위를 벗어나면 SearchTarget 상태로 변경
        if (attackTarget == null)
        {
            return false;
        }
        float distance = Vector3.Distance(attackTarget.position, transform.position);
        if (distance > towertemplate.weapon[level].range)
        {
            attackTarget = null;
            return false;
        }

        return true;
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        float damage = towertemplate.weapon[level].damage + AddedDamage;

        clone.GetComponent<Projectile>().SetUp(attackTarget, damage);
    }

    private void SpawnLaser()
    {
        Vector3 direction = attackTarget.position - firePoint.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(firePoint.position, direction, 
                                                    towertemplate.weapon[level].range, targetLayer);
        // raycastAll로 발사한 ray와 충돌한 모든 적을 검사
        // 그 중 attackTarget과 동일 오브젝트 검출
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].transform == attackTarget)
            {
                // 레이저 시작지점
                lineRenderer.SetPosition(0, firePoint.position);
                // 레이저 목표지점
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                // 레이저 이펙트 위치
                hitEffect.position = hit[i].point;

                float damage = towertemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHP>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }

    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }

    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }

    public bool Upgrade()
    {
        if (playerGold.CurrentGold < towertemplate.weapon[level + 1].cost)
        {
            return false;
        }

        if (weaponType == WeaponType.Laser)
        {
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }
        if (weaponType == WeaponType.Slow)
        {
            CircleCollider2D circleCollider2D = GetComponentInChildren<CircleCollider2D>();
            circleCollider2D.radius = towertemplate.weapon[level + 1].range;
        }

        level++;
        spriteRenderer.sprite = towertemplate.weapon[level].sprite;
        playerGold.CurrentGold -= towertemplate.weapon[level].cost;

        towerSpawner.OnBuffAllBuffTowers();

        return true;
    }

    public void Sell()
    {
        playerGold.CurrentGold += towertemplate.weapon[level].sellingPrice;
        ownerTile.IsBuildTower = false;
        Destroy(gameObject);
    }

    public void OnBuffAroundTower()
    {
        // 모든 "Tower" 검색
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; i++)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if (weapon.BuffLevel > Level)
            {
                continue;
            }

            // 현재 버프타워와 다른 타워와의 거리를 계산해서 범위 안에 있다면
            if (Vector3.Distance(weapon.transform.position, transform.position) <= towertemplate.weapon[level].range)
            {
                if (weapon.WeaponType == WeaponType.Laser || weapon.WeaponType == WeaponType.Cannon)
                {
                    weapon.AddedDamage = weapon.Damage * towertemplate.weapon[level].buff;
                    weapon.BuffLevel = level;
                }
            }
        }
    }
}
