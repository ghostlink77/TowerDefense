using UnityEngine;

public class SliderPositionAutoSetter : MonoBehaviour
{
    [SerializeField] private Vector3 distance = Vector3.down * 20.0f;
    private Transform targetTransform;
    private RectTransform rectTransform;

    public void SetUp(Transform target)
    {
        targetTransform = target;
        rectTransform = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        // 적이 파괴되면 Slider도 삭제
        if (targetTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        // 오브젝트의 좌표를 기준으로 화면에서의 좌표를 구함
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        // UI 위치 설정
        rectTransform.position = screenPosition + distance;
    }
}
