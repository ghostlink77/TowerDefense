using UnityEngine;
using UnityEngine.UI;

public class EnemyHPViewer : MonoBehaviour
{
    private EnemyHP enemyHP;
    private Slider hpSlider;
    
    public void SetUp(EnemyHP enemyHP)
    {
        this.enemyHP = enemyHP;
        hpSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        hpSlider.value = enemyHP.CurrentHP / enemyHP.MaxHP;
    }
}
