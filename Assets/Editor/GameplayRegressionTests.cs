#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        original.relics.Add(RelicType.EchoCrystal.ToString());

        SaveGameData restored = JsonUtility.FromJson<SaveGameData>(JsonUtility.ToJson(original));

        Assert.AreEqual(SaveData.CurrentVersion, restored.version);
        Assert.AreEqual(12, restored.floor);
        Assert.AreEqual(345, restored.gold);
        Assert.AreEqual(100, restored.player.maxHp);
        Assert.AreEqual(7, restored.progression.level);
        CollectionAssert.AreEqual(new[] { 3, 2, 1 }, restored.skillTrees[0].skillLevels);
        Assert.AreEqual("Ember", restored.equippedSkills[0].skillId);
        CollectionAssert.Contains(restored.relics, RelicType.EchoCrystal.ToString());
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
    public void DungeonGraph_ContainsEliteRecoveryAndTreasureChoices()
    {
        GameObject gameObject = new GameObject("Room variety test");
        RoomGenerator generator = gameObject.AddComponent<RoomGenerator>();
        generator.ApplySaveData(1);
        MethodInfo generateLayout = typeof(RoomGenerator).GetMethod("GenerateLayout", BindingFlags.Instance | BindingFlags.NonPublic);
        IDictionary layout = (IDictionary)generateLayout.Invoke(generator, new object[] { new System.Random(42) });
        HashSet<string> roomTypes = new HashSet<string>();

        foreach (DictionaryEntry room in layout)
        {
            roomTypes.Add(room.Value.ToString());
        }

        CollectionAssert.IsSubsetOf(new[] { "Elite", "Healing", "Treasure" }, roomTypes);
        UnityEngine.Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void LevelUpChoices_UseExistingSkillsAndUnlockTheNextTier()
    {
        GameObject gameObject = new GameObject("Skill choice test");
        SkillTree tree = gameObject.AddComponent<SkillTree>();
        List<GSkillCore> skills = new List<GSkillCore>();
        List<SkillUI> skillUis = new List<SkillUI>();

        for (int i = 0; i < 3; i++)
        {
            Ember skill = ScriptableObject.CreateInstance<Ember>();
            skill.skillName = $"Existing Fire Skill {i}";
            skill.skillType = SkillCore.SkillType.Active;
            skills.Add(new GSkillCore { skillCore = skill });
            skillUis.Add(ScriptableObject.CreateInstance<SkillUI>());
        }

        SetPrivateField(tree, "listSkill", skills);
        SetPrivateField(tree, "listSkillUI", skillUis);
        List<SkillLevelUpCandidate> candidates = new List<SkillLevelUpCandidate>();
        tree.GetLevelUpCandidates(candidates);
        Assert.AreEqual(1, candidates.Count);
        Assert.AreSame(skills[0], candidates[0].skill);

        Assert.IsTrue(tree.ApplyLevelUpChoice(0));
        Assert.IsTrue(tree.ApplyLevelUpChoice(0));
        Assert.IsTrue(tree.ApplyLevelUpChoice(0));
        candidates.Clear();
        tree.GetLevelUpCandidates(candidates);
        Assert.AreEqual(1, candidates.Count);
        Assert.AreSame(skills[1], candidates[0].skill);

        foreach (GSkillCore skill in skills)
        {
            UnityEngine.Object.DestroyImmediate(skill.skillCore);
        }
        foreach (SkillUI skillUi in skillUis)
        {
            UnityEngine.Object.DestroyImmediate(skillUi);
        }
        UnityEngine.Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void FireAndElectro_TriggerOverloadAndConsumeBothStatuses()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Enemy/Goblin.prefab");
        Assert.NotNull(prefab);
        GameObject enemyObject = UnityEngine.Object.Instantiate(prefab);
        EnemyCore enemy = enemyObject.GetComponent<EnemyCore>();
        Assert.NotNull(enemy);
        if (enemyObject.GetComponent<Collider2D>() == null)
        {
            enemyObject.AddComponent<CircleCollider2D>();
        }

        enemy.maxHp = 1000;
        enemy.currentHp = 1000;
        Physics2D.SyncTransforms();
        enemy.ApplyElement(ElementType.Fire, 100);
        Assert.IsTrue(enemy.IsBurning);
        enemy.ApplyElement(ElementType.Electro, 100);

        Assert.IsFalse(enemy.IsBurning);
        Assert.IsFalse(enemy.IsShocked);
        Assert.Less(enemy.currentHp, 1000);
        UnityEngine.Object.DestroyImmediate(enemyObject);
    }

    [Test]
    public void ChoicePopup_BuildsThreeLandscapeCards()
    {
        GameplayChoiceUI ui = GameplayChoiceUI.EnsureExists();
        CanvasScaler scaler = ui.GetComponent<CanvasScaler>();
        Assert.NotNull(scaler);
        Assert.AreEqual(new Vector2(1920f, 1080f), scaler.referenceResolution);

        ChoiceCardHover[] cards = ui.GetComponentsInChildren<ChoiceCardHover>(true);
        Assert.AreEqual(3, cards.Length);
        foreach (ChoiceCardHover card in cards)
        {
            RectTransform rect = card.GetComponent<RectTransform>();
            RectTransform panel = rect.parent as RectTransform;
            float pixelWidth = (rect.anchorMax.x - rect.anchorMin.x) *
                               (panel.anchorMax.x - panel.anchorMin.x) * scaler.referenceResolution.x;
            float pixelHeight = (rect.anchorMax.y - rect.anchorMin.y) *
                                (panel.anchorMax.y - panel.anchorMin.y) * scaler.referenceResolution.y;
            Assert.Greater(pixelWidth, pixelHeight, "Choice cards should be landscape-oriented on a 16:9 screen.");
        }

        Button[] buttons = ui.GetComponentsInChildren<Button>(true);
        Assert.AreEqual(3, buttons.Length);

        UnityEngine.Object.DestroyImmediate(ui.gameObject);
    }

    [Test]
    public void BootScene_IsFirstAndContainsFallbackLoadingUI()
    {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
        Assert.GreaterOrEqual(buildScenes.Length, 3);
        Assert.IsTrue(buildScenes[0].enabled);
        Assert.AreEqual(BootSceneBuilder.BootScenePath, buildScenes[0].path);
        Assert.AreEqual("Assets/Scenes/HomeScene.unity", buildScenes[1].path);

        Scene bootScene = EditorSceneManager.OpenScene(BootSceneBuilder.BootScenePath, OpenSceneMode.Additive);
        try
        {
            BootLoader loader = null;
            foreach (GameObject root in bootScene.GetRootGameObjects())
            {
                loader = root.GetComponent<BootLoader>();
                if (loader != null)
                {
                    break;
                }
            }

            Assert.NotNull(loader);
            Assert.AreEqual("HomeScene", loader.NextSceneName);
            Assert.Greater(loader.FirebaseTimeoutSeconds, 0f);
            Assert.LessOrEqual(loader.FirebaseTimeoutSeconds, 10f, "Firebase must never block boot indefinitely.");
            Assert.NotNull(loader.BackgroundImage);
            Assert.AreEqual("Background Placeholder", loader.BackgroundImage.gameObject.name);
            Assert.NotNull(loader.BackgroundImage.GetComponent<BootBackgroundFitter>());
        }
        finally
        {
            EditorSceneManager.CloseScene(bootScene, true);
        }
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

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.NotNull(field, $"Missing field {fieldName} on {target.GetType().Name}.");
        field.SetValue(target, value);
    }
}
#endif
