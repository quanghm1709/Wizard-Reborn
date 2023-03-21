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
        var curState = GetState(_characterState);
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
        float damage = atk + maxAtk * bonusDmg;
        currentHp -= (int)damage;
        hpBar.value = currentHp;
        if (currentHp <= 0)
        {
            ChangeState(CharacterState.Death);
        }
    }

    public void TakeSusDamage(int totalDmg, float time)
    {
        throw new System.NotImplementedException();
    }
}
