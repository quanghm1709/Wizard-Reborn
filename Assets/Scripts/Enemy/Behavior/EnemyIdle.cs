using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behaviour/Idle", fileName = "Idle")]
public class EnemyIdle : State
{
    private const float WaitAttack = 2f;
    public override CharacterState GetState()
    {
        return CharacterState.Idle;
    }
  
    public override void Action()
    {
            _agent.anim.SetBool("isMove", false);
            _agent.anim.SetBool("isAttack", false);
            if (_agent.TickIdle(WaitAttack))
            {
                _agent.anim.SetBool("isMove", true);
                _agent.anim.SetBool("isAttack", false);
                _agent.ChangeState(CharacterState.Moving);
            }
    }
}
