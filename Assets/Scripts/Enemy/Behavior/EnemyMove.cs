using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : State
{
    public override CharacterState GetState()
    {
        return CharacterState.Moving;
    }

    public override void Action() {
        Move();
    }

    private void Move()
    {
        if (!_agent.Detect() && _agent.canMove)
        {
            _agent.transform.position = Vector2.MoveTowards(_agent.transform.position, _agent.tar.transform.position, _agent.currentSpd * Time.deltaTime);
            _agent.anim.SetBool("isMove", true);
            _agent.anim.SetBool("isAttack", false);
        }
        else
        {
            _agent.ChangeState(CharacterState.Attack);
        }
        
    }
}
