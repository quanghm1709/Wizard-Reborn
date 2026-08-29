using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomCategory
{
    Start,
    Combat,
    Elite,
    Healing,
    Treasure,
    Shop,
    Exit,
    Boss
}

public enum EncounterObjective
{
    None,
    Elimination,
    Survival,
    Ritual
}

public class RoomController : MonoBehaviour
{
    public int roomId;

    [SerializeField] private bool isEnemyRoom;
    [SerializeField] private int totalWave;
    public bool isClear;

    [SerializeField] private Transform[] detectRoom;
    [SerializeField] private GameObject[] teleportPoint;

    private bool playerIn = false;
    private EnemyCore boss;
    private bool configured;
    private float enteredAt;
    private TextMesh previewLabel;

    public RoomCategory Category { get; private set; } = RoomCategory.Combat;
    public EncounterObjective Objective { get; private set; } = EncounterObjective.Elimination;
    public bool IsBonusObjectiveComplete { get; private set; }
    public Vector3 ObjectiveCenter => transform.position + new Vector3(4.75f, 4.25f, 0f);

    private void OnEnable()
    {
        this.RegisterListener(EventID.OnRoomClear, HandleRoomClear);
    }

    private void OnDisable()
    {
        this.RemoveListener(EventID.OnRoomClear, HandleRoomClear);
    }

    private void Start()
    {
        boss = GetComponentInChildren<EnemyCore>(true);
        if (boss != null && boss.type == EnemyType.Boss)
        {
            Category = RoomCategory.Boss;
            Objective = EncounterObjective.Elimination;
            isEnemyRoom = true;
            totalWave = 0;
            boss.SetOwningRoom(this);
            boss.gameObject.SetActive(false);
        }

        if (!configured && boss == null)
        {
            Category = isEnemyRoom ? RoomCategory.Combat : RoomCategory.Start;
            Objective = isEnemyRoom ? EncounterObjective.Elimination : EncounterObjective.None;
        }

        if (isEnemyRoom)
        {
            if (boss == null && Category != RoomCategory.Elite)
            {
                totalWave = Random.Range(1, 4);
            }
        }
        else if (Category != RoomCategory.Healing && Category != RoomCategory.Treasure)
        {
            OnRoomClear(roomId);
        }
    }

    public void Configure(RoomCategory category, EncounterObjective objective)
    {
        configured = true;
        Category = category;
        Objective = objective;
        isEnemyRoom = category == RoomCategory.Combat || category == RoomCategory.Elite || category == RoomCategory.Boss;
        if (category == RoomCategory.Elite)
        {
            totalWave = 1;
        }
        CreatePreviewLabel();
    }

    private void HandleRoomClear(object param)
    {
        if (param is int clearedRoomId)
        {
            OnRoomClear(clearedRoomId);
        }
    }

    public void OnRoomClear(int param)
    {
        if(param == roomId)
        {
            for (int i = 0; i < detectRoom.Length; i++)
            {
                Collider2D[] hit = Physics2D.OverlapCircleAll(detectRoom[i].position, 1);
                if (hit.Length > 0)
                {
                    //Debug.Log("direct " + hit[0].name + " " +i + " " +roomId);
                    teleportPoint[i].SetActive(true);
                }
            }
        }
    }

    internal void ResetRoom()
    {
        foreach(GameObject g in teleportPoint)
        {
            g.SetActive(false);
        }
        isClear = false;
        playerIn = false;
    }

    public void CompleteCombatRoom()
    {
        if (isClear)
        {
            return;
        }

        isClear = true;
        IsBonusObjectiveComplete = Category == RoomCategory.Elite || Category == RoomCategory.Boss ||
                                   (enteredAt > 0f && Time.time - enteredAt <= 25f);
        this.PostEvent(EventID.OnRoomClear, roomId);
        if (Category == RoomCategory.Combat || Category == RoomCategory.Elite || Category == RoomCategory.Healing ||
            Category == RoomCategory.Treasure || Category == RoomCategory.Boss)
        {
            this.PostEvent(EventID.OnRoomReward, this);
        }
    }

    public void OnBossDefeated()
    {
        CompleteCombatRoom();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.name);
        if(collision.CompareTag("Player") && !playerIn)
        {
            playerIn = true;
            enteredAt = Time.time;
            RunManager.Instance?.OnRoomEntered();

            if (Category == RoomCategory.Healing || Category == RoomCategory.Treasure)
            {
                CompleteCombatRoom();
                return;
            }

            if (!isEnemyRoom)
            {
                return;
            }

            if (boss != null)
            {
                boss.gameObject.SetActive(true);
            }
            else if (totalWave > 0 && EnemyGenerator.instance != null)
            {
                Vector3 spawnPoint = new Vector3(transform.position.x + 4.75f, transform.position.y + 4.25f, transform.position.z);
                bool elite = Category == RoomCategory.Elite;
                if (Objective == EncounterObjective.Survival || Objective == EncounterObjective.Ritual)
                {
                    StartCoroutine(EnemyGenerator.instance.GenerateTimedEncounter(spawnPoint, 25f, Objective == EncounterObjective.Ritual, this, elite));
                }
                else
                {
                    StartCoroutine(EnemyGenerator.instance.GenerateEnemy(spawnPoint, totalWave, this, elite));
                }
            }
        }
    }

    public void UpdateObjectiveProgress(float progress)
    {
        if (previewLabel != null && (Objective == EncounterObjective.Survival || Objective == EncounterObjective.Ritual))
        {
            previewLabel.text = $"{GetRoomLabel()} {Mathf.RoundToInt(progress * 100f)}%";
        }
    }

    private void CreatePreviewLabel()
    {
        if (previewLabel != null || Category == RoomCategory.Start)
        {
            return;
        }

        GameObject labelObject = new GameObject("Room Preview");
        labelObject.transform.SetParent(transform, false);
        labelObject.transform.localPosition = new Vector3(4.75f, 7.7f, -1f);
        previewLabel = labelObject.AddComponent<TextMesh>();
        previewLabel.anchor = TextAnchor.MiddleCenter;
        previewLabel.alignment = TextAlignment.Center;
        previewLabel.fontSize = 40;
        previewLabel.characterSize = .08f;
        previewLabel.text = GetRoomLabel();
        previewLabel.color = GetRoomColor();
        previewLabel.GetComponent<MeshRenderer>().sortingOrder = 30;
    }

    private string GetRoomLabel()
    {
        switch (Category)
        {
            case RoomCategory.Elite: return "ELITE";
            case RoomCategory.Healing: return "HEAL";
            case RoomCategory.Treasure: return "TREASURE";
            case RoomCategory.Shop: return "SHOP";
            case RoomCategory.Exit: return "EXIT";
            case RoomCategory.Boss: return "BOSS";
            default: return Objective == EncounterObjective.Survival ? "SURVIVE" : Objective == EncounterObjective.Ritual ? "RITUAL" : "COMBAT";
        }
    }

    private Color GetRoomColor()
    {
        switch (Category)
        {
            case RoomCategory.Elite:
            case RoomCategory.Boss: return new Color(1f, .2f, .16f);
            case RoomCategory.Healing: return new Color(.2f, 1f, .4f);
            case RoomCategory.Treasure: return new Color(1f, .78f, .15f);
            case RoomCategory.Shop: return new Color(.25f, .75f, 1f);
            default: return Color.white;
        }
    }
}
