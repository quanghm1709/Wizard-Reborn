using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : EnemyCore, IDamage
{
    [Header("Behavior")]
    [SerializeField] public List<State> states;
     public CharacterState _characterState;

    private void Update()
    {
        TickElementalStatuses();
        var curState = GetState(_characterState);
        if (curState == null)
        {
            return;
        }
        curState.Init(this);
        curState.Action();

        Flip();
        ReloadHit();

        
    }

    private State GetState(CharacterState characterState)
    {
        foreach (var state in states)
        {
            if (state.GetState() == characterState)
                return state;
        }
        return null;
    }

    public override void ChangeState(CharacterState enemyState)
    {
        _characterState = enemyState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(dmgPoint.position, dmgRange);
    }

    public override void Flip()
    {
        if (tar == null)
        {
            return;
        }

        if(tar.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void TakeDamage(int atk, int maxAtk, float bonusDmg)
    {
        if (IsDead)
        {
            return;
        }

        float shockMultiplier = IsShocked ? 1.2f : 1f;
        int damage = Mathf.Max(0, Mathf.RoundToInt((atk + maxAtk * bonusDmg) * shockMultiplier));
        if (damage == 0)
        {
            return;
        }
        currentHp -= damage;
        Color hitColor = IsBurning
            ? new Color(1f, .32f, .08f, 1f)
            : IsShocked
                ? new Color(.2f, .68f, 1f, 1f)
                : new Color(1f, .78f, .2f, 1f);
        CombatFeedback.ShowHit(transform.position, damage, hitColor);
        if (hpBar != null)
        {
            hpBar.value = currentHp;
        }
        if (currentHp <= 0)
        {
            BeginDeath();
        }
    }

    public void TakeSusDamage(int totalDmg, float time)
    {
        if (!IsDead)
        {
            StartCoroutine(DamageOverTime(totalDmg, time));
        }
    }

    private IEnumerator DamageOverTime(int totalDmg, float duration)
    {
        int ticks = Mathf.Max(1, Mathf.CeilToInt(duration));
        int damagePerTick = Mathf.Max(1, Mathf.CeilToInt((float)totalDmg / ticks));
        for (int i = 0; i < ticks && !IsDead; i++)
        {
            TakeDamage(damagePerTick, damagePerTick, 0f);
            yield return new WaitForSeconds(1f);
        }
    }
}
