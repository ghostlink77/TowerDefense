using UnityEngine;
using TMPro;

public enum SystemType { Money = 0, Build }

public class SystemTextViewer : MonoBehaviour
{
    private TextMeshProUGUI textSystem;
    private TMPAlpha tmpAlpha;

     void Awake()
    {
        textSystem = GetComponent<TextMeshProUGUI>();
        tmpAlpha = GetComponent<TMPAlpha>();
    }

    public void PrintText(SystemType type)
    {
        switch (type)
        {
            case SystemType.Money:
                textSystem.text = "System : Not enough money ...";
                break;
            case SystemType.Build:
                textSystem.text = "System : Invalid Build Tower ...";
                break;
        }

        tmpAlpha.FadeOut();
    }
}
