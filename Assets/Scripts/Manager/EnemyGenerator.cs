using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public static EnemyGenerator instance;

    public ObjectPool enemyPool;

    public List<GameObject> activeEnemy;

    [SerializeField] private List<string> enemyName;

    private RoomController currentRoom;
    private bool isGenerating;

    private void Start()
    {
        instance = this;
    }

    public IEnumerator GenerateEnemy(Vector3 room, int totalWave, RoomController roomController, bool elite = false)
    {
        if (isGenerating)
        {
            yield break;
        }

        isGenerating = true;
        currentRoom = roomController;
        int waveCount = Mathf.Max(1, totalWave);

        for (int waveIndex = 0; waveIndex < waveCount; waveIndex++)
        {
            yield return new WaitForSeconds(waveIndex == 0 ? 1f : 1.5f);
            SpawnWave(room, roomController, elite);
            yield return new WaitUntil(() => activeEnemy.Count == 0);
        }

        isGenerating = false;
        if (currentRoom == roomController && roomController != null)
        {
            roomController.CompleteCombatRoom();
        }
    }

    public IEnumerator GenerateTimedEncounter(Vector3 room, float duration, bool ritual, RoomController roomController, bool elite = false)
    {
        if (isGenerating)
        {
            yield break;
        }

        isGenerating = true;
        currentRoom = roomController;
        float progress = 0f;
        float spawnCooldown = 0f;
        Transform player = GameObject.Find("Player")?.transform;

        while (progress < duration && currentRoom == roomController)
        {
            bool canProgress = !ritual || (player != null && Vector2.Distance(player.position, roomController.ObjectiveCenter) <= 3.5f);
            if (canProgress)
            {
                progress += Time.deltaTime;
                roomController.UpdateObjectiveProgress(progress / duration);
            }

            spawnCooldown -= Time.deltaTime;
            if (spawnCooldown <= 0f && activeEnemy.Count < 8)
            {
                SpawnEnemies(room, roomController, elite, elite ? 2 : 3);
                spawnCooldown = 5f;
            }
            yield return null;
        }

        DeactivateActiveEnemies();
        isGenerating = false;
        if (currentRoom == roomController && roomController != null)
        {
            roomController.CompleteCombatRoom();
        }
    }

    private void SpawnWave(Vector3 room, RoomController roomController, bool elite)
    {
        int totalEnemy = Random.Range(3, 5) + FloorManager.currentFloor / 3;
        if (elite)
        {
            totalEnemy = Mathf.Max(1, totalEnemy / 2);
        }
        SpawnEnemies(room, roomController, elite, totalEnemy);
    }

    private void SpawnEnemies(Vector3 room, RoomController roomController, bool elite, int totalEnemy)
    {
        for (int i = 0; i < totalEnemy; i++)
        {
            float rand = Random.Range(0f, 1f);
            Vector3 spawnPoint = new Vector3(Random.Range(room.x - 5f, room.x + 5f), Random.Range(room.y - 3f, room.y + 3f), room.z);

            string selectedEnemy;
            if (rand < .3f && enemyName.Count > 1)
            {
                selectedEnemy = rand < .1f && enemyName.Count > 2 ? enemyName[2] : enemyName[1];
            }
            else
            {
                selectedEnemy = enemyName[0];
            }

            GameObject enemy = enemyPool.GetObject(selectedEnemy);
            if (enemy == null)
            {
                Debug.LogError($"Enemy pool cannot provide '{selectedEnemy}'.");
                continue;
            }

            enemy.transform.position = spawnPoint;
            EnemyCore enemyCore = enemy.GetComponent<EnemyCore>();
            enemyCore.SetOwningRoom(roomController);
            enemyCore.ResetData();
            enemyCore.SetElite(elite);

            activeEnemy.Add(enemy);
        }
    }

    public void NotifyEnemyDefeated(EnemyCore enemy)
    {
        if (enemy != null)
        {
            activeEnemy.Remove(enemy.gameObject);
        }
    }

    public void ResetFloorCombat()
    {
        StopAllCoroutines();
        isGenerating = false;
        currentRoom = null;

        DeactivateActiveEnemies();
    }

    private void DeactivateActiveEnemies()
    {
        for (int i = activeEnemy.Count - 1; i >= 0; i--)
        {
            GameObject enemy = activeEnemy[i];
            if (enemy != null)
            {
                enemy.SetActive(false);
            }
        }
        activeEnemy.Clear();
    }
}
