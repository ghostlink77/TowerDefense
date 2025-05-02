using UnityEngine;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField] private TowerSpawner towerSpawner;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Cast a ray from the camera to the mouse position
            // ray.origin : 카메라의 위치
            // ray.direction : 카메라에서 마우스 포인터 방향
            ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // 2D 모니터를 통해 3D의 오브젝트를 마우스로 선택
            // ray에 부딪히는 오브젝트를 hit에 저장
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // ray에 부딪힌 오브젝트가 "Tile"
                if (hit.transform.CompareTag("Tile"))
                {
                    towerSpawner.SpawnTower(hit.transform);
                }
            }
        }
    }
}
