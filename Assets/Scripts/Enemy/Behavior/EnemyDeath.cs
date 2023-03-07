using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : State
{
    private float deadTime = 2;
    public override CharacterState GetState()
    {
        return CharacterState.Death;
    }

    public override void Action()
    {
        _agent.rb.velocity = Vector2.zero;
        _agent.anim.SetBool("isDead", true);
        deadTime -= Time.deltaTime;
        if (deadTime <= 0)
        {
            EnemyGenerator.instance.activeEnemy.Remove(_agent.gameObject);
            //_agent.PostEvent(EventID.OnEnemyDead);
            _agent.gameObject.SetActive(false);
        }

    }
}
