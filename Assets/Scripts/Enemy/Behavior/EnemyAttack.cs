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
        if(_agent.Detect() && _agent.canAttack)
        {
            _agent.anim.SetTrigger("isAttack");
            _agent.anim.SetBool("isMove", false);
        }
        else
        {
            _agent.ChangeState(CharacterState.Moving);
        }
    }
}
