using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FloorManager floorManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerLevelManager playerLevelManager;
    [SerializeField] private RoomGenerator roomGenerator;
    [SerializeField] private SkillUIManager skillUIManager;
    [SerializeField] private SkillHolder skillHolder;

    private void OnEnable()
    {
        this.RegisterListener(EventID.OnPlayerEnterGate, HandlePlayerEnterGate);
    }

    private void OnDisable()
    {
        this.RemoveListener(EventID.OnPlayerEnterGate, HandlePlayerEnterGate);
    }

    private void Start()
    {
        RunManager.Create(playerController);
        RoomRewardController.Create(playerController);
        LevelUpChoiceController.Create(skillUIManager.skillTrees, skillHolder, playerController);
        if (SaveData.TryLoadGame(out SaveGameData save))
        {
            ApplySaveData(save);
            Debug.Log("Versioned save loaded successfully.");
        }
        else if (SaveData.HasKey("Player"))
        {
            try
            {
                floorManager.Load();
                playerController.Load();
                playerLevelManager.Load();
                roomGenerator.Load();
                foreach(SkillTree s in skillUIManager.skillTrees)
                {
                    s.LoadSkill();
                    skillUIManager.treeIndex++;
                }
                skillHolder.LoadData();

                SaveCurrentGame();
                Debug.Log("Legacy save migrated successfully.");
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

        }
        else
        {
            FloorManager.currentFloor = 1;
            GoldManager.playerGold = 0;
            FloorManager.readyGenerate = true;
        }
    }

    private void HandlePlayerEnterGate(object param)
    {
        StartCoroutine(SaveAfterGate());
    }

    private IEnumerator SaveAfterGate()
    {
        yield return null;
        SaveCurrentGame();
    }

    private void ApplySaveData(SaveGameData save)
    {
        floorManager.ApplySaveData(save.floor);
        roomGenerator.ApplySaveData(save.specialRoom);
        playerController.ApplySaveData(save.player);
        playerLevelManager.ApplySaveData(save.progression);
        GoldManager.playerGold = Mathf.Max(0, save.gold);
        RunManager.Instance.ApplyRelics(save.relics);

        PassiveSkillHolder.instance.ClearSkills();
        if (save.skillTrees == null)
        {
            save.skillTrees = new List<SkillTreeSaveData>();
        }
        foreach (SkillTree tree in skillUIManager.skillTrees)
        {
            SkillTreeSaveData treeData = save.skillTrees.Find(item => item.treePosition == tree.TreePosition);
            tree.ApplySaveData(treeData);
        }
        skillHolder.ApplySaveData(save.equippedSkills ?? new List<EquippedSkillSaveData>(), skillUIManager.skillTrees);
    }

    private void SaveCurrentGame()
    {
        SaveGameData save = new SaveGameData
        {
            floor = FloorManager.currentFloor,
            specialRoom = roomGenerator.SpecialRoom,
            gold = GoldManager.playerGold,
            player = playerController.CaptureSaveData(),
            progression = playerLevelManager.CaptureSaveData(),
            equippedSkills = skillHolder.CaptureSaveData()
        };
        save.relics = RunManager.Instance != null ? RunManager.Instance.CaptureRelics() : new List<string>();

        foreach (SkillTree tree in skillUIManager.skillTrees)
        {
            save.skillTrees.Add(tree.CaptureSaveData());
        }
        SaveData.SaveGame(save);
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused && floorManager != null && playerController != null)
        {
            SaveCurrentGame();
        }
    }

    private void OnApplicationQuit()
    {
        if (floorManager != null && playerController != null)
        {
            SaveCurrentGame();
        }
    }
}
