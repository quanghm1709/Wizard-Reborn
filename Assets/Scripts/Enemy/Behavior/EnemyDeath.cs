using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behaviour/Dead", fileName = "Dead")]
public class EnemyDeath : State
{
    private const float DeadTime = 2f;
    public override CharacterState GetState()
    {
        return CharacterState.Death;
    }

    public override void Action()
    {
        _agent.rb.linearVelocity = Vector2.zero;
        _agent.anim.SetBool("isDead", true);
        if (_agent.TickDeath(DeadTime))
        {
            _agent.CompleteDeath();
        }

    }
}
