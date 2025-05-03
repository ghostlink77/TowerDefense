using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class EnemySpawner : MonoBehaviour
{
    //[SerializeField] private GameObject enemyPrefab;

    [SerializeField] private PlayerHP playerHP;
    [SerializeField] private PlayerGold playerGold;

    [SerializeField] private GameObject enemyHPSliderPrefab;
    [SerializeField] private Transform canvasTransform; // UI Canvas Transform

    //[SerializeField] private float spawnTime;
    [SerializeField] private Transform[] wayPoints;

    private Wave currentWave;
    private int currentEnemyCount;

    private List<Enemy> enemyList;      // List of currently present enemies
    public List<Enemy> EnemyList => enemyList;
    public int CurrentEnemyCount => currentEnemyCount;
    public int MaxEnemyCount => currentWave.maxEnemyCount;

    private void Awake()
    {
        enemyList = new List<Enemy>();
        //StartCoroutine("SpawnEnemy");
    }

    public void StartWave(Wave wave)
    {
        currentWave = wave;
        currentEnemyCount = currentWave.maxEnemyCount;
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        // 웨이브에서 생성한 적의 숫자
        int spawnEnemyCount = 0;

        // 웨이브에서 생성되어야하는 적의 숫자만큼만 생성
        while (spawnEnemyCount < currentWave.maxEnemyCount)
        {
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.SetUp(this, wayPoints);
            enemyList.Add(enemy);

            SpawnEnemyHPSlider(clone);

            spawnEnemyCount++;

            // wait for spawnTime
            yield return new WaitForSeconds(currentWave.spawnTime);
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

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        if (type == EnemyDestroyType.Arrive)
        {
            playerHP.TakeDamage(1);
        }
        else if(type == EnemyDestroyType.Kill)
        {
            playerGold.CurrentGold += gold;
        }

        currentEnemyCount--;
        enemyList.Remove(enemy);
        Destroy(enemy.gameObject);
    }
}
