using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossSkillState
{
    None,
    Skill1,
    Skill2,
    Skill3,
}
public class BossController : MonoBehaviour
{
    public BossSkillState currentSkill;

    [Header("Skill1")]
    [SerializeField] private GameObject summonMonster;
    [SerializeField] private int total;

    [Header("Skill2")]
    [SerializeField] private GameObject projectile;

    [Header("Skill3")]
    [SerializeField] private Transform attackpoint;
    [SerializeField] private float skill3TelegraphTime = .5f;

    private readonly List<GameObject> summons = new List<GameObject>();
    private EnemyCore enemyCore;

    private void Awake()
    {
        enemyCore = GetComponent<EnemyCore>();
    }

    private void OnEnable()
    {
        currentSkill = BossSkillState.Skill1;
    }

    private void OnDisable()
    {
        CleanupSummons();
    }

    public void Skill1() {
        summons.RemoveAll(item => item == null || !item.activeInHierarchy);
        if (summonMonster == null || EnemyGenerator.instance == null)
        {
            currentSkill = BossSkillState.Skill2;
            return;
        }
        for (int i = 0; i < total; i++)
        {
            Vector3 randpos = transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f);
            GameObject g = EnemyGenerator.instance.enemyPool.GetObject(summonMonster.name);
            if (g == null)
            {
                continue;
            }
            g.transform.position = randpos;
            EnemyCore summon = g.GetComponent<EnemyCore>();
            if (summon != null)
            {
                summon.SetOwningRoom(null);
                summon.ResetData();
            }
            summons.Add(g);
        }
        currentSkill = BossSkillState.Skill2;
    }
    public void Skill2() {
        if (enemyCore != null && enemyCore.tar != null && projectile != null)
        {
            Vector2 lookDirection = enemyCore.tar.transform.position - transform.position;
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            GameObject shot = Instantiate(projectile, enemyCore.dmgPoint.position, Quaternion.Euler(0f, 0f, angle));
            EnemyProjectile enemyProjectile = shot.GetComponent<EnemyProjectile>();
            if (enemyProjectile != null)
            {
                enemyProjectile.damage = enemyCore.currentAtk;
            }
        }
        currentSkill = BossSkillState.Skill3;
    }
    public void Skill3() {
        currentSkill = BossSkillState.Skill1;
        StartCoroutine(ExecuteAreaAttack());
    }

    private IEnumerator ExecuteAreaAttack()
    {
        yield return new WaitForSeconds(skill3TelegraphTime);
        if (enemyCore == null || enemyCore.IsDead)
        {
            yield break;
        }

        Vector3 center = attackpoint != null ? attackpoint.position : transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, enemyCore.dmgRange, enemyCore.detectLayer);
        foreach (Collider2D hit in hits)
        {
            IDamage damageable = hit.GetComponent<IDamage>();
            damageable?.TakeDamage(enemyCore.currentAtk, enemyCore.maxAtk, .25f);
        }
    }

    public void CleanupSummons()
    {
        foreach (GameObject summon in summons)
        {
            if (summon != null && summon.activeSelf)
            {
                summon.SetActive(false);
            }
        }
        summons.Clear();
    }
}
