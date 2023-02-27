using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : State
{
    public override CharacterState GetState()
    {
        return CharacterState.Attack;
    }
    public override void Action()
    {
        Attack();
    }

    private void Attack()
    {
        if(_agent.Detect())
        {
            if (_agent.canAttack)
            {
                _agent.anim.SetBool("isAttack", true);
                _agent.anim.SetBool("isMove", false);
                Collider2D[] hit = Physics2D.OverlapCircleAll(_agent.dmgPoint.position, _agent.dmgRange, _agent.detectLayer);
                if (hit != null)
                {
                    StartCoroutine(hit[0].GetComponent<IDamage>().TakeDamage(_agent.currentAtk, _agent.maxAtk, 0));
                }
                _agent.LoadHit();
           }
        }
        else
        {
            _agent.ChangeState(CharacterState.Moving);
        }
    }
}
