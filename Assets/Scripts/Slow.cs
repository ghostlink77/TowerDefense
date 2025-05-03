using UnityEngine;

public class Slow : MonoBehaviour
{
    private TowerWeapon towerWeapon;

    void Awake()
    {
        towerWeapon = GetComponentInParent<TowerWeapon>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
        {
            return;
        }

        Movement2D movement2D = collision.GetComponent<Movement2D>();
        movement2D.MoveSpeed *= 1 - towerWeapon.Slow;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy"))
        {
            return;
        }

        Movement2D movement2D = collision.GetComponent<Movement2D>();
        movement2D.MoveSpeed /= 1 - towerWeapon.Slow;
        //collision.GetComponent<Movement2D>().ResetMoveSpeed();
    }
}
