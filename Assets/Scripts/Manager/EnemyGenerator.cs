using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public static EnemyGenerator instance;

    public ObjectPool enemyPool;

    public List<GameObject> activeEnemy;

    private int totalWave;
    private RoomController currentRoom;

    private void Start()
    {
        instance = this;
        RegisterEvent();
    }

    private void RegisterEvent()
    {
        this.RegisterListener(EventID.OnEnemyDead, (param) => OnEnemyDead());
    }

    private void OnEnemyDead()
    {
        if (activeEnemy.Count <= 0)
        {
            this.PostEvent(EventID.OnRoomClear, currentRoom.roomId);
            Debug.Log("Wave end");
            currentRoom.isClear = true;
        }
    }

    public IEnumerator GenerateEnemy(Vector3 room, int totalWave, RoomController roomController)
    {
        currentRoom = roomController;
        this.totalWave = totalWave;
        yield return new WaitForSeconds(2f);

        int totalEnemy = Random.Range(3, 10);

        for (int i = 0; i < totalEnemy; i++)
        {
            Vector3 spawnPoint = new Vector3(Random.Range(room.x + 5, room.x - 5), Random.Range(room.y + 3, room.y - 3), room.z);
            GameObject enemy = enemyPool.GetObject("Enemy");
            enemy.transform.position = spawnPoint;
            enemy.GetComponent<EnemyCore>().ResetData();

            activeEnemy.Add(enemy);
        }

        this.totalWave--;
        if (this.totalWave > 1)
        {
            StartCoroutine(GenerateEnemy(room, this.totalWave, currentRoom));
        }
    }
}
