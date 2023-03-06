using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdle : State
{
    private float waitAttack = 2f;
    public override CharacterState GetState()
    {
        return CharacterState.Idle;
    }
  
    public override void Action()
    {
        //if (!_agent.Detect())
        {
            
            waitAttack -= Time.deltaTime;
            if (waitAttack <= 0)
            {
                _agent.anim.SetBool("isMove", true);
                _agent.anim.SetBool("isAttack", false);
                waitAttack = 2f;
                _agent.ChangeState(CharacterState.Moving);
            }
            
           
        }
        //else
        //{
        //    _agent.ChangeState(CharacterState.Attack);
        //}
    }
}
