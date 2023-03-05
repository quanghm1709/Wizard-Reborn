using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdle : State
{
    public override CharacterState GetState()
    {
        return CharacterState.Idle;
    }

    public override void Action()
    {
        //if (!_agent.Detect())
        {
            _agent.anim.SetBool("isMove", true);
            _agent.anim.SetBool("isAttack", false);
            _agent.ChangeState(CharacterState.Moving);
        }
        //else
        //{
        //    _agent.ChangeState(CharacterState.Attack);
        //}
    }
}
