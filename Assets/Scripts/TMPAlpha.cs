using UnityEngine;
using System.Collections;
using TMPro;

public class TMPAlpha : MonoBehaviour
{
    [SerializeField] private float lerpTime = 0.5f;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void FadeOut()
    {
        StartCoroutine(AlphaLerp(1, 0));
    }

    private IEnumerator AlphaLerp(float start, float end)
    {
        float currentTime = 0.0f;
        float percentage = 0.0f;

        while (percentage < 1.0f)
        {
            currentTime += Time.deltaTime;
            percentage = currentTime / lerpTime;

            Color color = text.color;
            color.a = Mathf.Lerp(start, end, percentage);
            text.color = color;

            yield return null;
        }
    }
}
