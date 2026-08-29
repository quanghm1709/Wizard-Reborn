#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class GameplayRegressionTests
{
    [Test]
    public void VersionedSave_RoundTripsCriticalProgressionData()
    {
        SaveGameData original = new SaveGameData
        {
            floor = 12,
            specialRoom = 6,
            gold = 345,
            player = new PlayerSaveData
            {
                currentHp = 75,
                maxHp = 100,
                currentMp = 30f,
                maxMp = 50f,
                currentAtk = 14,
                maxAtk = 20,
                currentSpd = 4f,
                maxSpd = 5f,
                attackCooldown = .8f
            },
            progression = new PlayerLevelSaveData
            {
                level = 7,
                currentExp = 25f,
                maxExp = 150f,
                skillPoint = 3
            }
        };
        original.skillTrees.Add(new SkillTreeSaveData
        {
            treePosition = 1,
            treeType = SkillTreeType.Fire.ToString(),
            skillLevels = new List<int> { 3, 2, 1 }
        });
        original.equippedSkills.Add(new EquippedSkillSaveData { slot = 2, skillId = "Ember" });

        SaveGameData restored = JsonUtility.FromJson<SaveGameData>(JsonUtility.ToJson(original));

        Assert.AreEqual(SaveData.CurrentVersion, restored.version);
        Assert.AreEqual(12, restored.floor);
        Assert.AreEqual(345, restored.gold);
        Assert.AreEqual(100, restored.player.maxHp);
        Assert.AreEqual(7, restored.progression.level);
        CollectionAssert.AreEqual(new[] { 3, 2, 1 }, restored.skillTrees[0].skillLevels);
        Assert.AreEqual("Ember", restored.equippedSkills[0].skillId);
    }

    [Test]
    public void LevelUp_UsesFixedPercentageInsteadOfStatAsPercentage()
    {
        GameObject gameObject = new GameObject("Player progression test");
        PlayerController player = gameObject.AddComponent<PlayerController>();
        player.maxHp = 100;
        player.currentHp = 100;
        player.maxMp = 50f;
        player.currentMp = 50f;

        player.LevelUp();

        Assert.AreEqual(102, player.maxHp);
        Assert.AreEqual(51f, player.maxMp, .001f);
        UnityEngine.Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void DungeonGraph_IsConnectedAcrossOneThousandSeeds()
    {
        GameObject gameObject = new GameObject("Dungeon generator test");
        RoomGenerator generator = gameObject.AddComponent<RoomGenerator>();
        MethodInfo generateLayout = typeof(RoomGenerator).GetMethod("GenerateLayout", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(generateLayout);

        for (int seed = 0; seed < 1000; seed++)
        {
            generator.ApplySaveData(seed % 6 + 1);
            IDictionary layout = (IDictionary)generateLayout.Invoke(generator, new object[] { new System.Random(seed) });
            Assert.GreaterOrEqual(layout.Count, 6, $"Seed {seed} generated too few rooms.");
            Assert.AreEqual(layout.Count, CountConnectedRooms(layout), $"Seed {seed} generated an isolated room.");
        }

        UnityEngine.Object.DestroyImmediate(gameObject);
    }

    [TestCase(3, "Shop")]
    [TestCase(6, "Boss")]
    public void DungeonGraph_SpecialFloorsContainExpectedRoom(int cycle, string expectedRoomType)
    {
        GameObject gameObject = new GameObject("Special room test");
        RoomGenerator generator = gameObject.AddComponent<RoomGenerator>();
        generator.ApplySaveData(cycle);
        MethodInfo generateLayout = typeof(RoomGenerator).GetMethod("GenerateLayout", BindingFlags.Instance | BindingFlags.NonPublic);
        IDictionary layout = (IDictionary)generateLayout.Invoke(generator, new object[] { new System.Random(42) });

        bool found = false;
        foreach (DictionaryEntry room in layout)
        {
            found |= room.Value.ToString() == expectedRoomType;
        }

        Assert.IsTrue(found, $"Cycle {cycle} should contain a {expectedRoomType} room.");
        UnityEngine.Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void EnemyScaling_DoesNotCompoundWhenPooledEnemyIsReused()
    {
        int originalFloor = FloorManager.currentFloor;
        FloorManager.currentFloor = 5;
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy/Goblin.prefab");
        Assert.NotNull(prefab);
        GameObject enemyObject = UnityEngine.Object.Instantiate(prefab);
        EnemyCore enemy = enemyObject.GetComponent<EnemyCore>();
        int firstSpawnMaxHp = enemy.maxHp;

        enemyObject.SetActive(false);
        enemyObject.SetActive(true);

        Assert.AreEqual(firstSpawnMaxHp, enemy.maxHp);
        UnityEngine.Object.DestroyImmediate(enemyObject);
        FloorManager.currentFloor = originalFloor;
    }

    [Test]
    public void DisablingPooledEnemy_DoesNotEmitDeathEvent()
    {
        EventDispatcher dispatcher = EventDispatcher.Instance;
        int deathEvents = 0;
        Action<object> listener = _ => deathEvents++;
        dispatcher.RegisterListener(EventID.OnEnemyDead, listener);

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy/Goblin.prefab");
        GameObject enemyObject = UnityEngine.Object.Instantiate(prefab);
        enemyObject.SetActive(false);

        Assert.AreEqual(0, deathEvents);
        dispatcher.RemoveListener(EventID.OnEnemyDead, listener);
        UnityEngine.Object.DestroyImmediate(enemyObject);
        UnityEngine.Object.DestroyImmediate(dispatcher.gameObject);
    }

    private static int CountConnectedRooms(IDictionary layout)
    {
        HashSet<Vector2Int> allRooms = new HashSet<Vector2Int>();
        foreach (DictionaryEntry entry in layout)
        {
            allRooms.Add((Vector2Int)entry.Key);
        }

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        queue.Enqueue(Vector2Int.zero);
        visited.Add(Vector2Int.zero);

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        while (queue.Count > 0)
        {
            Vector2Int room = queue.Dequeue();
            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbour = room + direction;
                if (allRooms.Contains(neighbour) && visited.Add(neighbour))
                {
                    queue.Enqueue(neighbour);
                }
            }
        }
        return visited.Count;
    }
}
#endif
