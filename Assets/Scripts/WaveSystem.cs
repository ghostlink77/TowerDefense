using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private EnemySpawner enemySpawner;

    private int currentWaveIndex = -1;

    public int CurrnetWave => currentWaveIndex + 1;
    public int MaxWave => waves.Length;

    public void StartWave()
    {
        if (enemySpawner.EnemyList.Count == 0 && currentWaveIndex < waves.Length-1)
        {
            currentWaveIndex++;
            enemySpawner.StartWave(waves[currentWaveIndex]);
        }
    }
}

[System.Serializable]
public struct Wave
{
    public float spawnTime;
    public int maxEnemyCount;
    public GameObject[] enemyPrefabs;   // 웨이브 적 등장 종류
}