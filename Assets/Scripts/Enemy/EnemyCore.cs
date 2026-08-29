using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EnemyType
{
    Melee,
    Range,
    Boss,
}

public abstract class EnemyCore : Core
{
    public EnemyType type;
    public GameObject tar;
    public int id;
    public float dropExp;

    [Header("Radar")]
    public float detectRange;
    public LayerMask detectLayer;
    public Detect detect;

    [Header("Attack")]
    public float dmgRange;
    public Transform dmgPoint;
    public GameObject enemyProjectile;
    public Vector2 movement;

    [Header("UI")]
    public Slider hpBar;

    [Header("Scaling")]
    [SerializeField, Min(0f)] private float floorScaling = .2f;

    private int baseHp;
    private int baseAtk;
    private float idleTime;
    private float deathTime;
    private bool isDead;
    private RoomController owningRoom;

    public bool IsDead => isDead;

    protected override void Awake()
    {
        base.Awake();
        baseHp = maxHp;
        baseAtk = maxAtk;
    }

    private void OnEnable()
    {
        tar = GameObject.Find("Player");

        float floorMultiplier = 1f + floorScaling * Mathf.Max(0, FloorManager.currentFloor - 1);
        maxHp = Mathf.Max(1, Mathf.RoundToInt(baseHp * floorMultiplier));
        maxAtk = Mathf.Max(1, Mathf.RoundToInt(baseAtk * floorMultiplier));

        currentHp = maxHp;
        currentAtk = maxAtk;

        isDead = false;
        idleTime = 0f;
        deathTime = 0f;
        canMove = true;

        if (hpBar != null)
        {
            hpBar.maxValue = maxHp;
            hpBar.value = currentHp;
        }
        ChangeState(CharacterState.Idle);
    }
   
    private void OnDisable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    public virtual bool Detect()
    {
        return detect.Detecting(detectRange, detectLayer);
    }
    public void LoadHit()
    {
        timeBtwHitCD = timeBtwHit;
    }

    public void ResetData()
    {
        currentHp = maxHp;
        currentAtk = maxAtk;
        isDead = false;
        idleTime = 0f;
        deathTime = 0f;
        canMove = true;
        if (hpBar != null)
        {
            hpBar.maxValue = maxHp;
            hpBar.value = currentHp;
        }
    }

    public void SetOwningRoom(RoomController room)
    {
        owningRoom = room;
    }

    public bool TickIdle(float duration)
    {
        idleTime += Time.deltaTime;
        if (idleTime < duration)
        {
            return false;
        }

        idleTime = 0f;
        return true;
    }

    public void BeginDeath()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        deathTime = 0f;
        canMove = false;
        ChangeState(CharacterState.Death);
    }

    public bool TickDeath(float duration)
    {
        deathTime += Time.deltaTime;
        return deathTime >= duration;
    }

    public void CompleteDeath()
    {
        if (!isDead || !gameObject.activeSelf)
        {
            return;
        }

        if (type == EnemyType.Boss)
        {
            BossController boss = GetComponent<BossController>();
            if (boss != null)
            {
                boss.CleanupSummons();
            }
            owningRoom?.OnBossDefeated();
        }

        EnemyGenerator.instance?.NotifyEnemyDefeated(this);
        this.PostEvent(EventID.OnEnemyDead, (int)dropExp);
        DropItem dropItem = GetComponent<DropItem>();
        dropItem?.TryDrop();
        owningRoom = null;
        gameObject.SetActive(false);
    }

    public abstract void ChangeState(CharacterState enemyState);
    public abstract void Flip();
}
