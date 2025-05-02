using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private EnemySpawner enemySpawner;

    public void SpawnTower(Transform tileTransform)
    {
        // check if the tile is already occupied
        Tile tile = tileTransform.GetComponent<Tile>();
        if (tile.IsBuildTower == true)
        {
            return;
        }
        tile.IsBuildTower = true;

        GameObject clone = Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);

        clone.GetComponent<TowerWeapon>().SetUP(enemySpawner);
    }
}
