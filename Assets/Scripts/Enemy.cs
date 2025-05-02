using UnityEngine;
using System.Collections;

public enum EnemyDestroyType { Kill = 0, Arrive }

public class Enemy : MonoBehaviour
{
    private int wayPointCount;
    private Transform[] wayPoints;
    private int currentIndex = 0;
    private Movement2D movement2D;
    private EnemySpawner enemySpawner;

    public void SetUp(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        movement2D = GetComponent<Movement2D>();
        this.enemySpawner = enemySpawner;

        // set enemy's path WayPoints 
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // set enemy's position to the first WayPoint
        transform.position = wayPoints[currentIndex].position;

        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        NextMoveTo();

        while (true)
        {
            // enemy Object rotation
            transform.Rotate(Vector3.forward * 7);

            // 경로에서 탈주하는 것을 막기 위해 MoveSpeed 곱해줌
            if (Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
                NextMoveTo();

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        // wayPoints가 남아 있다면
        if (currentIndex < wayPointCount - 1)
        {
            // 위치를 정확히 목표위치로 설정
            transform.position = wayPoints[currentIndex].position;

            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            movement2D.MoveTo(direction);
        }
        // wayPoints가 끝났다면
        else
        {
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        enemySpawner.DestroyEnemy(type, this);
    }
}
