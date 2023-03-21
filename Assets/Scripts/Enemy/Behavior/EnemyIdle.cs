using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behaviour/Idle", fileName = "Idle")]
public class EnemyIdle : State
{
    private float waitAttack = 2f;
    public override CharacterState GetState()
    {
        return CharacterState.Idle;
    }
  
    public override void Action()
    {
            _agent.anim.SetBool("isMove", false);
            _agent.anim.SetBool("isAttack", false);
            waitAttack -= Time.deltaTime;
            if (waitAttack <= 0)
            {
                _agent.anim.SetBool("isMove", true);
                _agent.anim.SetBool("isAttack", false);
                waitAttack = 2f;
                _agent.ChangeState(CharacterState.Moving);
            }
    }
}
