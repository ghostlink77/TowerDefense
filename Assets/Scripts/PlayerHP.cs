using UnityEngine;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] private float maxHP = 20;
    private float currentHP;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {

        }
    }
}
