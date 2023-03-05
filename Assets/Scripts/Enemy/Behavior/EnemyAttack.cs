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
            _agent.rb.velocity = Vector2.zero;
            if (_agent.canAttack)
            {
                _agent.anim.SetBool("isAttack", true);
                _agent.anim.SetBool("isMove", false);
                Collider2D[] hit = Physics2D.OverlapCircleAll(_agent.dmgPoint.position, _agent.detectRange, _agent.detectLayer);
                Debug.Log(hit.Length);
                if (hit != null)
                {
                    hit[0].GetComponent<IDamage>().TakeDamage(_agent.currentAtk, _agent.maxAtk, 0);
                }
                _agent.LoadHit();
                _agent.ChangeState(CharacterState.Idle);
            }
        }
        else
        {
            _agent.ChangeState(CharacterState.Moving);
        }
    }
}
