using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private PlayerHP playerHP;

    [SerializeField] private GameObject enemyHPSliderPrefab;
    [SerializeField] private Transform canvasTransform; // UI Canvas Transform

    [SerializeField] private float spawnTime;
    [SerializeField] private Transform[] wayPoints;

    private List<Enemy> enemyList;      // List of currently present enemies
    public List<Enemy> EnemyList => enemyList;

    private void Awake()
    {
        enemyList = new List<Enemy>();
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        while (true)
        {
            GameObject clone = Instantiate(enemyPrefab);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.SetUp(this, wayPoints);
            enemyList.Add(enemy);

            SpawnEnemyHPSlider(clone);

            // wait for spawnTime
            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void SpawnEnemyHPSlider(GameObject enemy)
    {
        GameObject sliderClone = Instantiate(enemyHPSliderPrefab);

        // Slider UI is a child of the Canvas
        sliderClone.transform.SetParent(canvasTransform);
        // 크기 재설정
        sliderClone.transform.localScale = Vector3.one;

        sliderClone.GetComponent<SliderPositionAutoSetter>().SetUp(enemy.transform);
        sliderClone.GetComponent<EnemyHPViewer>().SetUp(enemy.GetComponent<EnemyHP>());
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy)
    {
        if (type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);
        }

        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}
