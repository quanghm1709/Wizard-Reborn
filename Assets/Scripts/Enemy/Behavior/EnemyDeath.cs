using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Behaviour/Dead", fileName = "Dead")]
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
        //_agent.PostEvent(EventID.OnEnemyDead, (int) _agent.dropExp);
        //PlayerLevelManager.instance.OnEnemyDead((int)_agent.dropExp);
        if (deadTime <= 0)
        {
            if(_agent.type != EnemyType.Boss)
            {
                EnemyGenerator.instance.activeEnemy.Remove(_agent.gameObject);
            }
            
            
            deadTime = 2;
            _agent.gameObject.SetActive(false);
        }

    }
}
