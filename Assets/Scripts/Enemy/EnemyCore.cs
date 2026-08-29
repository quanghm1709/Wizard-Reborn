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
    private bool isElite;
    private RoomController owningRoom;

    private float burnRemaining;
    private float burnTickRemaining;
    private int burnDamagePerTick;
    private float shockRemaining;

    public bool IsDead => isDead;
    public bool IsBurning => burnRemaining > 0f;
    public bool IsShocked => shockRemaining > 0f;

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
        isElite = false;
        ClearElementalStatuses();
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
        ClearElementalStatuses();
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

    public void SetElite(bool elite)
    {
        isElite = elite;
        if (elite)
        {
            maxHp = Mathf.RoundToInt(maxHp * 1.75f);
            maxAtk = Mathf.RoundToInt(maxAtk * 1.35f);
            currentHp = maxHp;
            currentAtk = maxAtk;
            if (hpBar != null)
            {
                hpBar.maxValue = maxHp;
                hpBar.value = currentHp;
            }
        }
    }

    public void TickElementalStatuses()
    {
        if (isDead)
        {
            return;
        }

        if (burnRemaining > 0f)
        {
            burnRemaining -= Time.deltaTime;
            burnTickRemaining -= Time.deltaTime;
            if (burnTickRemaining <= 0f)
            {
                burnTickRemaining += 1f;
                GetComponent<IDamage>()?.TakeDamage(burnDamagePerTick, burnDamagePerTick, 0f);
            }
        }

        if (shockRemaining > 0f)
        {
            shockRemaining -= Time.deltaTime;
        }
    }

    public void ApplyElement(ElementType element, int sourceDamage)
    {
        if (isDead || element == ElementType.None)
        {
            return;
        }

        if ((element == ElementType.Fire && IsShocked) ||
            (element == ElementType.Electro && IsBurning))
        {
            TriggerOverload(sourceDamage);
            return;
        }

        if (element == ElementType.Fire)
        {
            float durationMultiplier = RunManager.Instance != null ? RunManager.Instance.BurnDurationMultiplier : 1f;
            burnRemaining = Mathf.Max(burnRemaining, 3f * durationMultiplier);
            burnTickRemaining = Mathf.Min(burnTickRemaining, 1f);
            burnDamagePerTick = Mathf.Max(burnDamagePerTick, Mathf.Max(1, Mathf.RoundToInt(sourceDamage * .12f)));
        }
        else
        {
            shockRemaining = Mathf.Max(shockRemaining, 4f);
            RunManager.Instance?.RestoreManaFromShock();
        }
    }

    private void TriggerOverload(int sourceDamage)
    {
        ClearElementalStatuses();
        float multiplier = RunManager.Instance != null ? RunManager.Instance.OverloadDamageMultiplier : 1f;
        int reactionDamage = Mathf.Max(1, Mathf.RoundToInt(sourceDamage * .75f * multiplier));

        HashSet<EnemyCore> damagedEnemies = new HashSet<EnemyCore>();
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2.5f);
        foreach (Collider2D hit in hits)
        {
            EnemyCore enemy = hit.GetComponent<EnemyCore>();
            if (enemy == null || enemy.IsDead || !damagedEnemies.Add(enemy))
            {
                continue;
            }

            hit.GetComponent<IDamage>()?.TakeDamage(reactionDamage, reactionDamage, 0f);
        }
    }

    private void ClearElementalStatuses()
    {
        burnRemaining = 0f;
        burnTickRemaining = 0f;
        burnDamagePerTick = 0;
        shockRemaining = 0f;
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
        this.PostEvent(EventID.OnEnemyDead, Mathf.RoundToInt(dropExp * (isElite ? 2f : 1f)));
        DropItem dropItem = GetComponent<DropItem>();
        dropItem?.TryDrop();
        owningRoom = null;
        gameObject.SetActive(false);
    }

    public abstract void ChangeState(CharacterState enemyState);
    public abstract void Flip();
}
