using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField] private TowerSpawner towerSpawner;
    [SerializeField] private TowerDataViewer towerDataViewer;

    private Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    private Transform hitTransform = null;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 마우스가 UI 위에 있을 때는 클릭 이벤트를 무시
        if (EventSystem.current.IsPointerOverGameObject() == true)
        {
            return;
        }
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
                // hitTransform에 오브젝트 저장
                hitTransform = hit.transform;

                // ray에 부딪힌 오브젝트가 "Tile"
                if (hitTransform.CompareTag("Tile"))
                {
                    towerSpawner.SpawnTower(hitTransform);
                }
                else if (hitTransform.CompareTag("Tower"))
                {
                    towerDataViewer.OnPannel(hitTransform);
                }
            }
            
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (hitTransform == null || hitTransform.CompareTag("Tower") == false)
            {
                towerDataViewer.OffPannel();
            }

            hitTransform = null;
        }
    }
}
