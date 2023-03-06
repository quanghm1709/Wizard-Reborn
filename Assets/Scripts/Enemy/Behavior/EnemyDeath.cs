using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : State
{
    public override CharacterState GetState()
    {
        return CharacterState.Death;
    }

    public override void Action()
    {
        EnemyGenerator.instance.activeEnemy.Remove(_agent.gameObject);
        //_agent.PostEvent(EventID.OnEnemyDead);
        _agent.gameObject.SetActive(false);
    }
}
