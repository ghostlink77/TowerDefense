using UnityEngine;
using System.Collections;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField] private TowerTemplate[] towerTemplate;   // Tower Stats
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private PlayerGold playerGold;
    [SerializeField] private SystemTextViewer systemTextViewer;

    private bool isOnTowerButton = false;       // 타워 건설 버튼 클릭 여부
    private GameObject followTowerClone = null;
    private int towerType;


    public void ReadyToSpawnTower(int type)
    {
        towerType = type;

        if (isOnTowerButton == true)
        {
            return;
        }

        if (towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold)
        {
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        isOnTowerButton = true;
        // 마우스를 따라다니는 임시 타워 생성
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);
        // 타워 건설 취소 코루틴 시작
        StartCoroutine("OnTowerCancelSystem");
    }
    public void SpawnTower(Transform tileTransform)
    {
        if (isOnTowerButton == false)
        {
            return;
        }

        // check if the tile is already occupied
        Tile tile = tileTransform.GetComponent<Tile>();
        if (tile.IsBuildTower == true)
        {
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }

        isOnTowerButton = false;
        tile.IsBuildTower = true;
        playerGold.CurrentGold -= towerTemplate[towerType].weapon[0].cost;

        Vector3 position = tileTransform.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefab, position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().SetUP(this, enemySpawner, playerGold, tile);

        OnBuffAllBuffTowers();
        // 임시 타워 삭제
        Destroy(followTowerClone);
        // 타워 건설 취소 코루틴 종료
        StopCoroutine("OnTowerCancelSystem");
    }

    private IEnumerator OnTowerCancelSystem()
    {
        while (true)
        {
            if( Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButton(1) )
            {
                isOnTowerButton = false;
                Destroy(followTowerClone);
                break;
            }
            yield return null;
        }
    }

    public void OnBuffAllBuffTowers()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for(int i = 0; i < towers.Length; i++)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();
            
            if (weapon.WeaponType == WeaponType.Buff)
            {
                weapon.OnBuffAroundTower();
            }
        }
    }
}
