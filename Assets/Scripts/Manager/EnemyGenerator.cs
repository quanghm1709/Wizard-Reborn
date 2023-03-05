using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public static EnemyGenerator instance;

    [SerializeField] private ObjectPool enemyPool;
    private int totalWave;
    private void Start()
    {
        instance = this;
    }

    public IEnumerator GenerateEnemy(Vector3 room, int totalWave)
    {
        this.totalWave = totalWave;
        yield return new WaitForSeconds(2f);

        int totalEnemy = Random.Range(3, 10);
        Debug.Log(totalEnemy +"|"+ room);
        for (int i = 0; i < totalEnemy; i++)
        {
            Vector3 spawnPoint = new Vector3(Random.Range(room.x + 5, room.x - 5), Random.Range(room.y + 3, room.y - 3), room.z);
            GameObject enemy = enemyPool.GetObject("Enemy");
            enemy.transform.position = spawnPoint;
        }

        this.totalWave--;
        if (this.totalWave > 1)
        {
            StartCoroutine(GenerateEnemy(room, this.totalWave));
        }
    }
}
